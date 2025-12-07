/**
 * Reviews API
 * Functions for managing customer reviews and ratings
 */

import { supabase } from '../lib/supabase'

export interface CreateReviewData {
  appointment_id: string
  provider_id: string
  rating: number
  comment?: string
}

/**
 * Create a review for a completed appointment
 */
export async function createReview(data: CreateReviewData) {
  const { data: userData, error: userError } = await supabase.auth.getUser()

  if (userError || !userData.user) {
    throw new Error('Not authenticated')
  }

  // Verify the appointment exists and is completed
  const { data: appointment, error: aptError } = await supabase
    .from('appointments')
    .select('status, customer_id')
    .eq('id', data.appointment_id)
    .single()

  if (aptError) throw aptError

  if (appointment.customer_id !== userData.user.id) {
    throw new Error('You can only review your own appointments')
  }

  if (appointment.status !== 'completed') {
    throw new Error('You can only review completed appointments')
  }

  // Check if review already exists
  const { data: existing } = await supabase
    .from('reviews')
    .select('id')
    .eq('appointment_id', data.appointment_id)
    .single()

  if (existing) {
    throw new Error('You have already reviewed this appointment')
  }

  const { data: review, error } = await supabase
    .from('reviews')
    .insert({
      ...data,
      customer_id: userData.user.id,
      is_verified: true, // Mark as verified since it's from a real booking
    })
    .select()
    .single()

  if (error) throw error

  return review
}

/**
 * Get reviews for a provider
 */
export async function getProviderReviews(providerId: string) {
  const { data, error } = await supabase
    .from('reviews')
    .select(`
      *,
      profiles (
        full_name,
        avatar_url
      )
    `)
    .eq('provider_id', providerId)
    .order('created_at', { ascending: false })

  if (error) throw error

  return data
}

/**
 * Get review for a specific appointment
 */
export async function getAppointmentReview(appointmentId: string) {
  const { data, error } = await supabase
    .from('reviews')
    .select('*')
    .eq('appointment_id', appointmentId)
    .single()

  if (error) {
    if (error.code === 'PGRST116') {
      // No review found
      return null
    }
    throw error
  }

  return data
}

/**
 * Update provider's response to a review
 */
export async function respondToReview(reviewId: string, response: string) {
  const { data: userData, error: userError } = await supabase.auth.getUser()

  if (userError || !userData.user) {
    throw new Error('Not authenticated')
  }

  // Get the review to verify ownership
  const { data: review, error: reviewError } = await supabase
    .from('reviews')
    .select('provider_id')
    .eq('id', reviewId)
    .single()

  if (reviewError) throw reviewError

  // Verify the user owns this provider
  const { data: provider, error: providerError } = await supabase
    .from('providers')
    .select('id')
    .eq('id', review.provider_id)
    .eq('user_id', userData.user.id)
    .single()

  if (providerError || !provider) {
    throw new Error('You can only respond to reviews for your own business')
  }

  const { data, error } = await supabase
    .from('reviews')
    .update({
      response,
      response_at: new Date().toISOString(),
    })
    .eq('id', reviewId)
    .select()
    .single()

  if (error) throw error

  return data
}

/**
 * Get all reviews for current user's provider
 */
export async function getMyProviderReviews() {
  const { data: userData, error: userError } = await supabase.auth.getUser()

  if (userError || !userData.user) {
    throw new Error('Not authenticated')
  }

  // Get the provider ID for the current user
  const { data: provider, error: providerError } = await supabase
    .from('providers')
    .select('id')
    .eq('user_id', userData.user.id)
    .single()

  if (providerError) throw providerError

  const { data, error} = await supabase
    .from('reviews')
    .select(`
      *,
      profiles (
        full_name,
        avatar_url
      ),
      appointments (
        appointment_date,
        services (
          name
        )
      )
    `)
    .eq('provider_id', provider.id)
    .order('created_at', { ascending: false })

  if (error) throw error

  return data
}
