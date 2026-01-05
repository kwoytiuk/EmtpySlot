/**
 * Supabase client configuration (LEGACY - Not used, kept for migration reference)
 * This file provides Supabase clients for both browser and server environments
 *
 * NOTE: This project now uses .NET API backend. See packages/shared/src/lib/apiClient.ts
 */

import { createClient } from '@supabase/supabase-js'
import type { Database } from '../types/database.types'
import { env } from '../config/env'

/**
 * Browser-side Supabase client (LEGACY - DO NOT USE)
 * Use the .NET API client from apiClient.ts instead
 */
export const supabase = env.supabase.url && env.supabase.anonKey
  ? createClient<Database>(
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
  : null as any // Return null if Supabase not configured (using .NET API instead)

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
