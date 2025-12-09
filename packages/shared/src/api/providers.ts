/**
 * Providers API
 * Functions for searching and managing service providers
 */

import { supabase } from '../lib/supabase'
import type { Provider } from '../lib/supabase'

export interface SearchProvidersParams {
  latitude?: number
  longitude?: number
  radiusKm?: number
  categoryId?: string
  minRating?: number
  verified?: boolean
  limit?: number
  offset?: number
}

export interface CreateProviderData {
  business_name: string
  description?: string
  phone?: string
  email?: string
  website?: string
  cancellation_policy?: string
}

/**
 * Search for providers with geospatial filtering
 * Note: For MVP, we'll use simple filtering. PostGIS queries will be added via Edge Functions
 */
export async function searchProviders(params: SearchProvidersParams) {
  // Optimized query - only select fields we need for search results
  let query = supabase
    .from('providers')
    .select(`
      id,
      business_name,
      description,
      phone,
      email,
      verified,
      rating_average,
      rating_count,
      provider_locations!inner (
        id,
        city,
        state_province,
        latitude,
        longitude,
        is_primary
      ),
      services (
        id,
        name,
        price,
        duration_minutes,
        category_id,
        service_categories (
          id,
          name,
          slug
        )
      )
    `)

  // Filter by verified status
  if (params.verified !== undefined) {
    query = query.eq('verified', params.verified)
  }

  // Filter by minimum rating
  if (params.minRating) {
    query = query.gte('rating_average', params.minRating)
  }

  // Pagination - get more to account for category filtering
  const limit = params.limit || 20
  const offset = params.offset || 0
  // Fetch more records to ensure we have enough after filtering
  query = query.range(offset, offset + (limit * 3) - 1)

  // Order by rating for better results
  query = query.order('rating_average', { ascending: false, nullsFirst: false })

  const { data, error } = await query

  if (error) throw error

  let results = data || []

  // Filter by category if specified (client-side since Supabase doesn't support nested filtering)
  if (params.categoryId) {
    results = results.filter((provider) => {
      return provider.services?.some(
        (service: any) => service.category_id === params.categoryId
      )
    })
  }

  // Filter by distance if location provided
  if (params.latitude && params.longitude && params.radiusKm) {
    results = results.filter((provider) => {
      if (!provider.provider_locations || provider.provider_locations.length === 0) {
        return false
      }

      return provider.provider_locations.some((location: any) => {
        const distance = calculateDistance(
          params.latitude!,
          params.longitude!,
          location.latitude,
          location.longitude
        )
        return distance <= params.radiusKm!
      })
    })
  }

  // Limit results after filtering
  results = results.slice(0, limit)

  return results
}

/**
 * Get a single provider by ID with all related data (optimized - no reviews)
 * Reviews should be loaded separately on-demand
 */
export async function getProvider(id: string) {
  const { data, error} = await supabase
    .from('providers')
    .select(`
      id,
      business_name,
      description,
      phone,
      email,
      website,
      verified,
      rating_average,
      rating_count,
      cancellation_policy,
      provider_locations (*),
      services (
        id,
        name,
        description,
        price,
        duration_minutes,
        category_id,
        service_categories (
          id,
          name,
          slug
        )
      )
    `)
    .eq('id', id)
    .single()

  if (error) throw error

  return data
}

/**
 * Get provider reviews separately for better performance
 */
export async function getProviderReviews(providerId: string, limit: number = 10) {
  const { data, error } = await supabase
    .from('reviews')
    .select(`
      id,
      rating,
      comment,
      response,
      created_at,
      profiles (
        full_name,
        avatar_url
      )
    `)
    .eq('provider_id', providerId)
    .order('created_at', { ascending: false })
    .limit(limit)

  if (error) throw error

  return data || []
}

/**
 * Create a new provider profile (for current user)
 */
export async function createProvider(providerData: CreateProviderData) {
  const { data: userData, error: userError } = await supabase.auth.getUser()

  if (userError || !userData.user) {
    throw new Error('Not authenticated')
  }

  // First, update user type to provider
  await supabase
    .from('profiles')
    .update({ user_type: 'provider' })
    .eq('id', userData.user.id)

  // Then create provider profile
  const { data, error } = await supabase
    .from('providers')
    .insert({
      user_id: userData.user.id,
      ...providerData,
    })
    .select()
    .single()

  if (error) throw error

  return data
}

/**
 * Update provider profile
 */
export async function updateProvider(
  id: string,
  updates: Partial<CreateProviderData>
) {
  const { data, error } = await supabase
    .from('providers')
    .update(updates)
    .eq('id', id)
    .select()
    .single()

  if (error) throw error

  return data
}

/**
 * Get provider for current user
 */
export async function getMyProvider() {
  const { data: userData, error: userError } = await supabase.auth.getUser()

  if (userError || !userData.user) {
    return null
  }

  const { data, error } = await supabase
    .from('providers')
    .select(`
      *,
      provider_locations (*),
      services (
        *,
        service_categories (*)
      ),
      staff_members (*)
    `)
    .eq('user_id', userData.user.id)
    .single()

  if (error) {
    if (error.code === 'PGRST116') {
      // No provider found
      return null
    }
    throw error
  }

  return data
}

/**
 * Calculate distance between two points using Haversine formula
 * Returns distance in kilometers
 */
function calculateDistance(
  lat1: number,
  lon1: number,
  lat2: number,
  lon2: number
): number {
  const R = 6371 // Earth's radius in kilometers
  const dLat = toRad(lat2 - lat1)
  const dLon = toRad(lon2 - lon1)

  const a =
    Math.sin(dLat / 2) * Math.sin(dLat / 2) +
    Math.cos(toRad(lat1)) *
      Math.cos(toRad(lat2)) *
      Math.sin(dLon / 2) *
      Math.sin(dLon / 2)

  const c = 2 * Math.atan2(Math.sqrt(a), Math.sqrt(1 - a))
  const distance = R * c

  return distance
}

function toRad(degrees: number): number {
  return degrees * (Math.PI / 180)
}
