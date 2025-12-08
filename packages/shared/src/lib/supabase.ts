/**
 * Supabase client configuration
 * This file provides Supabase clients for both browser and server environments
 */

import { createClient } from '@supabase/supabase-js'
import type { Database } from '../types/database.types'
import { env } from '../config/env'

// Validate environment variables
if (!env.supabase.url || env.supabase.url === 'your-project-url-here' || !env.supabase.url.includes('supabase.co')) {
  throw new Error(
    '❌ SUPABASE URL NOT CONFIGURED!\n\n' +
    'Please update your .env.local file with your actual Supabase credentials:\n' +
    '1. Go to https://app.supabase.com/project/_/settings/api\n' +
    '2. Copy your Project URL and anon/public key\n' +
    '3. Update apps/web/.env.local\n\n' +
    `Current value: "${env.supabase.url}"`
  )
}

if (!env.supabase.anonKey || env.supabase.anonKey === 'your-anon-key-here' || env.supabase.anonKey.length < 100) {
  throw new Error(
    '❌ SUPABASE ANON KEY NOT CONFIGURED!\n\n' +
    'Please update your .env.local file with your actual Supabase anon key.\n' +
    `Current value length: ${env.supabase.anonKey.length} characters`
  )
}

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
