/**
 * Appointments API
 * Functions for managing appointments and bookings
 */

import { get, post, put, authApi } from '../lib/apiClient'

export interface CreateAppointmentData {
  providerId: string
  locationId: string
  serviceId: string
  staffId?: string
  appointmentDate: string // YYYY-MM-DD
  startTime: string // HH:MM:SS
  endTime: string // HH:MM:SS
  price: number
  customerNotes?: string
}

export interface Appointment {
  id: string
  customerId: string
  providerId: string
  locationId: string
  serviceId: string
  staffId?: string | null
  appointmentDate: string
  startTime: string
  endTime: string
  price: number
  status: 'pending' | 'confirmed' | 'completed' | 'cancelled' | 'no_show'
  paymentStatus: 'pending' | 'paid' | 'refunded'
  customerNotes?: string | null
  cancellationReason?: string | null
  cancelledAt?: string | null
  cancelledBy?: string | null
  createdAt: string
  updatedAt: string
  provider?: {
    businessName: string
    phone: string | null
    email: string | null
  }
  location?: {
    name: string
    addressLine1: string
    addressLine2?: string | null
    city: string
    stateProvince: string
    postalCode: string
  }
  service?: {
    name: string
    description?: string | null
    durationMinutes: number
    price: number
  }
  customer?: {
    fullName: string | null
    phone: string | null
    email: string | null
  }
}

export interface TimeSlot {
  startTime: string
  endTime: string
  available: boolean
}

/**
 * Create a new appointment
 */
export async function createAppointment(data: CreateAppointmentData): Promise<Appointment> {
  return authApi.post<Appointment>('/appointments', data)
}

/**
 * Get appointments for current user (customer view)
 */
export async function getMyAppointments(): Promise<Appointment[]> {
  return authApi.get<Appointment[]>('/appointments/my')
}

/**
 * Get appointments for provider
 */
export async function getProviderAppointments(providerId: string): Promise<Appointment[]> {
  return authApi.get<Appointment[]>(`/appointments/provider/${providerId}`)
}

/**
 * Get a single appointment by ID
 */
export async function getAppointment(id: string): Promise<Appointment> {
  return authApi.get<Appointment>(`/appointments/${id}`)
}

/**
 * Cancel an appointment
 */
export async function cancelAppointment(
  id: string,
  reason?: string
): Promise<Appointment> {
  return authApi.put<Appointment>(`/appointments/${id}/cancel`, { reason })
}

/**
 * Update appointment status (provider only)
 */
export async function updateAppointmentStatus(
  id: string,
  status: 'pending' | 'confirmed' | 'completed' | 'cancelled' | 'no_show'
): Promise<Appointment> {
  return authApi.put<Appointment>(`/appointments/${id}/status`, { status })
}

/**
 * Get available time slots for a service on a specific date
 * Optionally filter by employee/staff member
 */
export async function getAvailableTimeSlots(
  providerId: string,
  serviceId: string,
  date: string,
  employeeId?: string
): Promise<TimeSlot[]> {
  const params = new URLSearchParams({
    serviceId,
    date,
  })

  if (employeeId) {
    params.append('employeeId', employeeId)
  }

  return get<TimeSlot[]>(`/providers/${providerId}/available-slots?${params.toString()}`)
}

/**
 * Get available slots for TODAY - OpenTable style
 * Shows next 3 hours of availability with both taken and available slots
 */
export async function getQuickBookingSlots(
  providerId: string,
  serviceId: string
): Promise<TimeSlot[]> {
  const params = new URLSearchParams({
    serviceId,
  })

  return get<TimeSlot[]>(`/providers/${providerId}/quick-slots?${params.toString()}`)
}
