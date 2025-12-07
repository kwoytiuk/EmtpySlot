/**
 * Appointments API
 * Functions for managing appointments and bookings
 */

import { supabase } from '../lib/supabase'
import type { Appointment } from '../lib/supabase'

export interface CreateAppointmentData {
  provider_id: string
  location_id: string
  service_id: string
  staff_id?: string
  appointment_date: string // YYYY-MM-DD
  start_time: string // HH:MM:SS
  end_time: string // HH:MM:SS
  price: number
  customer_notes?: string
}

export interface TimeSlot {
  start_time: string
  end_time: string
  available: boolean
}

/**
 * Create a new appointment
 */
export async function createAppointment(data: CreateAppointmentData) {
  const { data: userData, error: userError } = await supabase.auth.getUser()

  if (userError || !userData.user) {
    throw new Error('Not authenticated')
  }

  const { data: appointment, error } = await supabase
    .from('appointments')
    .insert({
      customer_id: userData.user.id,
      ...data,
      status: 'pending',
      payment_status: 'pending',
    })
    .select()
    .single()

  if (error) throw error

  return appointment
}

/**
 * Get appointments for current user (customer view)
 */
export async function getMyAppointments() {
  const { data: userData, error: userError } = await supabase.auth.getUser()

  if (userError || !userData.user) {
    throw new Error('Not authenticated')
  }

  const { data, error } = await supabase
    .from('appointments')
    .select(`
      *,
      providers (
        business_name,
        phone,
        email
      ),
      provider_locations (
        name,
        address_line1,
        city,
        state_province
      ),
      services (
        name,
        duration_minutes,
        price
      )
    `)
    .eq('customer_id', userData.user.id)
    .order('appointment_date', { ascending: true })
    .order('start_time', { ascending: true })

  if (error) throw error

  return data
}

/**
 * Get appointments for provider
 */
export async function getProviderAppointments(providerId: string) {
  const { data, error } = await supabase
    .from('appointments')
    .select(`
      *,
      profiles (
        full_name,
        phone
      ),
      services (
        name,
        duration_minutes
      )
    `)
    .eq('provider_id', providerId)
    .order('appointment_date', { ascending: true })
    .order('start_time', { ascending: true })

  if (error) throw error

  return data
}

/**
 * Get a single appointment by ID
 */
export async function getAppointment(id: string) {
  const { data, error } = await supabase
    .from('appointments')
    .select(`
      *,
      providers (
        business_name,
        phone,
        email
      ),
      provider_locations (
        name,
        address_line1,
        address_line2,
        city,
        state_province,
        postal_code
      ),
      services (
        name,
        description,
        duration_minutes,
        price
      ),
      profiles (
        full_name,
        phone,
        email
      )
    `)
    .eq('id', id)
    .single()

  if (error) throw error

  return data
}

/**
 * Cancel an appointment
 */
export async function cancelAppointment(
  id: string,
  reason?: string
) {
  const { data: userData, error: userError } = await supabase.auth.getUser()

  if (userError || !userData.user) {
    throw new Error('Not authenticated')
  }

  const { data, error } = await supabase
    .from('appointments')
    .update({
      status: 'cancelled',
      cancelled_by: userData.user.id,
      cancelled_at: new Date().toISOString(),
      cancellation_reason: reason,
    })
    .eq('id', id)
    .select()
    .single()

  if (error) throw error

  return data
}

/**
 * Update appointment status (provider only)
 */
export async function updateAppointmentStatus(
  id: string,
  status: 'pending' | 'confirmed' | 'completed' | 'cancelled' | 'no_show'
) {
  const { data, error } = await supabase
    .from('appointments')
    .update({ status })
    .eq('id', id)
    .select()
    .single()

  if (error) throw error

  return data
}

/**
 * Get available time slots for a service on a specific date
 * This is a simplified version - in production, use an Edge Function
 */
export async function getAvailableTimeSlots(
  providerId: string,
  serviceId: string,
  date: string
): Promise<TimeSlot[]> {
  // Get the service to know duration
  const { data: service, error: serviceError } = await supabase
    .from('services')
    .select('duration_minutes')
    .eq('id', serviceId)
    .single()

  if (serviceError) throw serviceError

  // Get existing appointments for that day
  const { data: appointments, error: appointmentsError } = await supabase
    .from('appointments')
    .select('start_time, end_time')
    .eq('provider_id', providerId)
    .eq('appointment_date', date)
    .in('status', ['pending', 'confirmed'])

  if (appointmentsError) throw appointmentsError

  // Generate time slots (9 AM to 6 PM, every 30 minutes)
  // In production, this would be based on provider's actual schedule
  const slots: TimeSlot[] = []
  const startHour = 9
  const endHour = 18

  for (let hour = startHour; hour < endHour; hour++) {
    for (let minute = 0; minute < 60; minute += 30) {
      const startTime = `${hour.toString().padStart(2, '0')}:${minute
        .toString()
        .padStart(2, '0')}:00`

      // Calculate end time based on service duration
      const endMinutes = hour * 60 + minute + service.duration_minutes
      const endHour = Math.floor(endMinutes / 60)
      const endMinute = endMinutes % 60

      if (endHour >= 18) continue // Don't create slots that end after closing

      const endTime = `${endHour.toString().padStart(2, '0')}:${endMinute
        .toString()
        .padStart(2, '0')}:00`

      // Check if slot conflicts with existing appointments
      const isAvailable = !appointments?.some((apt) => {
        return (
          (startTime >= apt.start_time && startTime < apt.end_time) ||
          (endTime > apt.start_time && endTime <= apt.end_time) ||
          (startTime <= apt.start_time && endTime >= apt.end_time)
        )
      })

      slots.push({
        start_time: startTime,
        end_time: endTime,
        available: isAvailable,
      })
    }
  }

  return slots
}
