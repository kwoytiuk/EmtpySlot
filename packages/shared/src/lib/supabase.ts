/**
 * Supabase client configuration
 * This file provides Supabase clients for both browser and server environments
 */

import { createClient } from '@supabase/supabase-js'
import type { Database } from '../types/database.types'
import { env } from '../config/env'

/**
 * Browser-side Supabase client
 * Use this in React components and client-side code
 */
export const supabase = createClient<Database>(
  env.supabase.url,
  env.supabase.anonKey,
  {
    auth: {
      persistSession: true,
      autoRefreshToken: true,
      detectSessionInUrl: true,
    },
  }
)

/**
 * Type-safe database helpers
 */
export type Tables<T extends keyof Database['public']['Tables']> =
  Database['public']['Tables'][T]['Row']

export type Enums<T extends keyof Database['public']['Enums']> =
  Database['public']['Enums'][T]

/**
 * Useful type exports
 */
export type Profile = Tables<'profiles'>
export type Provider = Tables<'providers'>
export type ProviderLocation = Tables<'provider_locations'>
export type Service = Tables<'services'>
export type ServiceCategory = Tables<'service_categories'>
export type Appointment = Tables<'appointments'>
export type Review = Tables<'reviews'>
export type StaffMember = Tables<'staff_members'>
export type Promotion = Tables<'promotions'>
export type Favorite = Tables<'favorites'>
export type Notification = Tables<'notifications'>

export type UserType = Enums<'user_type'>
export type AppointmentStatus = Enums<'appointment_status'>
export type PaymentStatus = Enums<'payment_status'>
export type DiscountType = Enums<'discount_type'>
