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
      ),
      customer:profiles!customer_id (
        full_name,
        phone
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
      customer:profiles!customer_id (
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
      customer:profiles!customer_id (
        full_name,
        phone
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
 * Optionally filter by employee/staff member
 * This is a simplified version - in production, use an Edge Function
 */
export async function getAvailableTimeSlots(
  providerId: string,
  serviceId: string,
  date: string,
  employeeId?: string
): Promise<TimeSlot[]> {
  // Get the service to know duration
  const { data: service, error: serviceError } = await supabase
    .from('services')
    .select('duration_minutes')
    .eq('id', serviceId)
    .single()

  if (serviceError) throw serviceError

  // If employee is specified, check their schedule for this day
  let employeeSchedule = null
  if (employeeId) {
    const dayOfWeek = new Date(date + 'T00:00:00').getDay()

    const { data: schedules } = await supabase
      .from('employee_schedules')
      .select('*')
      .eq('employee_id', employeeId)
      .eq('day_of_week', dayOfWeek)
      .eq('is_available', true)

    employeeSchedule = schedules && schedules.length > 0 ? schedules : null

    // Check for time off
    const { data: timeOff } = await supabase
      .from('employee_time_off')
      .select('*')
      .eq('employee_id', employeeId)
      .lte('start_date', date)
      .gte('end_date', date)

    // If employee has time off on this day, return no slots
    if (timeOff && timeOff.length > 0) {
      return []
    }

    // If no schedule found for this day, return no slots
    if (!employeeSchedule || employeeSchedule.length === 0) {
      return []
    }
  }

  // Get existing appointments for that day
  let appointmentsQuery = supabase
    .from('appointments')
    .select('start_time, end_time, staff_id')
    .eq('provider_id', providerId)
    .eq('appointment_date', date)
    .in('status', ['pending', 'confirmed'])

  // If employee specified, only check their appointments
  if (employeeId) {
    appointmentsQuery = appointmentsQuery.eq('staff_id', employeeId)
  }

  const { data: appointments, error: appointmentsError } = await appointmentsQuery

  if (appointmentsError) throw appointmentsError

  // Generate time slots based on employee schedule or default (9 AM to 6 PM)
  const slots: TimeSlot[] = []
  let startHour = 9
  let endHour = 18

  // If employee schedule exists, use their hours
  if (employeeSchedule && employeeSchedule.length > 0) {
    for (const schedule of employeeSchedule) {
      const scheduleStartHour = parseInt(schedule.start_time.split(':')[0])
      const scheduleStartMinute = parseInt(schedule.start_time.split(':')[1])
      const scheduleEndHour = parseInt(schedule.end_time.split(':')[0])
      const scheduleEndMinute = parseInt(schedule.end_time.split(':')[1])

      // Generate slots for this schedule block
      let currentMinutes = scheduleStartHour * 60 + scheduleStartMinute
      const endMinutes = scheduleEndHour * 60 + scheduleEndMinute

      while (currentMinutes < endMinutes) {
        const hour = Math.floor(currentMinutes / 60)
        const minute = currentMinutes % 60

        const startTime = `${hour.toString().padStart(2, '0')}:${minute
          .toString()
          .padStart(2, '0')}:00`

        // Calculate end time based on service duration
        const slotEndMinutes = currentMinutes + service.duration_minutes
        const slotEndHour = Math.floor(slotEndMinutes / 60)
        const slotEndMinute = slotEndMinutes % 60

        // Skip if slot would end after employee's schedule ends
        if (slotEndMinutes > endMinutes) break

        const endTime = `${slotEndHour.toString().padStart(2, '0')}:${slotEndMinute
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

        currentMinutes += 30 // 30 minute intervals
      }
    }
  } else {
    // Default schedule (9 AM to 6 PM, every 30 minutes)
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
  }

  return slots
}

/**
 * Get available slots for TODAY - OpenTable style
 * Shows next 3 hours of availability with both taken and available slots
 */
export async function getQuickBookingSlots(
  providerId: string,
  serviceId: string
): Promise<TimeSlot[]> {
  const now = new Date()
  const today = now.toISOString().split('T')[0]

  // Get current time in minutes since midnight
  const currentHour = now.getHours()
  const currentMinute = now.getMinutes()
  const currentTimeMinutes = currentHour * 60 + currentMinute

  // Round up to next 30-minute interval
  const roundedMinutes = Math.ceil(currentTimeMinutes / 30) * 30

  // Calculate end time (3 hours from now)
  const endTimeMinutes = roundedMinutes + (3 * 60)

  // Get the service to know duration
  const { data: service, error: serviceError } = await supabase
    .from('services')
    .select('duration_minutes')
    .eq('id', serviceId)
    .single()

  if (serviceError) throw serviceError

  // Get existing appointments for today
  const { data: appointments, error: appointmentsError } = await supabase
    .from('appointments')
    .select('start_time, end_time, staff_id')
    .eq('provider_id', providerId)
    .eq('appointment_date', today)
    .in('status', ['pending', 'confirmed'])

  if (appointmentsError) throw appointmentsError

  // Generate slots from now until 3 hours from now
  const slots: TimeSlot[] = []
  let currentSlotMinutes = roundedMinutes

  while (currentSlotMinutes < endTimeMinutes && currentSlotMinutes < 22 * 60) {
    const hour = Math.floor(currentSlotMinutes / 60)
    const minute = currentSlotMinutes % 60

    const startTime = `${hour.toString().padStart(2, '0')}:${minute
      .toString()
      .padStart(2, '0')}:00`

    // Calculate end time based on service duration
    const slotEndMinutes = currentSlotMinutes + service.duration_minutes
    const slotEndHour = Math.floor(slotEndMinutes / 60)
    const slotEndMinute = slotEndMinutes % 60

    const endTime = `${slotEndHour.toString().padStart(2, '0')}:${slotEndMinute
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

    currentSlotMinutes += 30 // 30 minute intervals
  }

  return slots
}
