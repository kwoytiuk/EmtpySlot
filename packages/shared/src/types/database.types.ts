/**
 * Database types generated from Supabase
 *
 * To regenerate these types after schema changes:
 * 1. Make sure Supabase is running locally OR
 * 2. Have your Supabase project URL configured
 * 3. Run: npm run generate-types (in packages/shared directory)
 *
 * For now, this is a placeholder. Once you set up Supabase, run:
 * npx supabase gen types typescript --project-id YOUR_PROJECT_ID > packages/shared/src/types/database.types.ts
 */

export type Json =
  | string
  | number
  | boolean
  | null
  | { [key: string]: Json | undefined }
  | Json[]

export interface Database {
  public: {
    Tables: {
      profiles: {
        Row: {
          id: string
          user_type: 'customer' | 'provider' | 'admin'
          full_name: string | null
          phone: string | null
          avatar_url: string | null
          created_at: string
          updated_at: string
        }
        Insert: {
          id: string
          user_type?: 'customer' | 'provider' | 'admin'
          full_name?: string | null
          phone?: string | null
          avatar_url?: string | null
          created_at?: string
          updated_at?: string
        }
        Update: {
          id?: string
          user_type?: 'customer' | 'provider' | 'admin'
          full_name?: string | null
          phone?: string | null
          avatar_url?: string | null
          created_at?: string
          updated_at?: string
        }
      }
      service_categories: {
        Row: {
          id: string
          name: string
          slug: string
          icon: string | null
          description: string | null
          parent_id: string | null
          display_order: number
          created_at: string
        }
        Insert: {
          id?: string
          name: string
          slug: string
          icon?: string | null
          description?: string | null
          parent_id?: string | null
          display_order?: number
          created_at?: string
        }
        Update: {
          id?: string
          name?: string
          slug?: string
          icon?: string | null
          description?: string | null
          parent_id?: string | null
          display_order?: number
          created_at?: string
        }
      }
      providers: {
        Row: {
          id: string
          user_id: string
          business_name: string
          description: string | null
          phone: string | null
          email: string | null
          website: string | null
          logo_url: string | null
          cover_image_url: string | null
          verified: boolean
          rating_average: number
          rating_count: number
          cancellation_policy: string | null
          created_at: string
          updated_at: string
        }
        Insert: {
          id?: string
          user_id: string
          business_name: string
          description?: string | null
          phone?: string | null
          email?: string | null
          website?: string | null
          logo_url?: string | null
          cover_image_url?: string | null
          verified?: boolean
          rating_average?: number
          rating_count?: number
          cancellation_policy?: string | null
          created_at?: string
          updated_at?: string
        }
        Update: {
          id?: string
          user_id?: string
          business_name?: string
          description?: string | null
          phone?: string | null
          email?: string | null
          website?: string | null
          logo_url?: string | null
          cover_image_url?: string | null
          verified?: boolean
          rating_average?: number
          rating_count?: number
          cancellation_policy?: string | null
          created_at?: string
          updated_at?: string
        }
      }
      provider_locations: {
        Row: {
          id: string
          provider_id: string
          name: string
          address_line1: string
          address_line2: string | null
          city: string
          state_province: string
          postal_code: string
          country: string
          latitude: number
          longitude: number
          location: unknown
          phone: string | null
          is_primary: boolean
          created_at: string
          updated_at: string
        }
        Insert: {
          id?: string
          provider_id: string
          name: string
          address_line1: string
          address_line2?: string | null
          city: string
          state_province: string
          postal_code: string
          country?: string
          latitude: number
          longitude: number
          location?: unknown
          phone?: string | null
          is_primary?: boolean
          created_at?: string
          updated_at?: string
        }
        Update: {
          id?: string
          provider_id?: string
          name?: string
          address_line1?: string
          address_line2?: string | null
          city?: string
          state_province?: string
          postal_code?: string
          country?: string
          latitude?: number
          longitude?: number
          location?: unknown
          phone?: string | null
          is_primary?: boolean
          created_at?: string
          updated_at?: string
        }
      }
      services: {
        Row: {
          id: string
          provider_id: string
          category_id: string
          name: string
          description: string | null
          duration_minutes: number
          price: number
          deposit_required: number | null
          image_url: string | null
          is_active: boolean
          created_at: string
          updated_at: string
        }
        Insert: {
          id?: string
          provider_id: string
          category_id: string
          name: string
          description?: string | null
          duration_minutes: number
          price: number
          deposit_required?: number | null
          image_url?: string | null
          is_active?: boolean
          created_at?: string
          updated_at?: string
        }
        Update: {
          id?: string
          provider_id?: string
          category_id?: string
          name?: string
          description?: string | null
          duration_minutes?: number
          price?: number
          deposit_required?: number | null
          image_url?: string | null
          is_active?: boolean
          created_at?: string
          updated_at?: string
        }
      }
      staff_members: {
        Row: {
          id: string
          provider_id: string
          name: string
          email: string | null
          phone: string | null
          avatar_url: string | null
          bio: string | null
          is_active: boolean
          created_at: string
          updated_at: string
        }
        Insert: {
          id?: string
          provider_id: string
          name: string
          email?: string | null
          phone?: string | null
          avatar_url?: string | null
          bio?: string | null
          is_active?: boolean
          created_at?: string
          updated_at?: string
        }
        Update: {
          id?: string
          provider_id?: string
          name?: string
          email?: string | null
          phone?: string | null
          avatar_url?: string | null
          bio?: string | null
          is_active?: boolean
          created_at?: string
          updated_at?: string
        }
      }
      appointments: {
        Row: {
          id: string
          booking_reference: string
          customer_id: string
          provider_id: string
          location_id: string
          service_id: string
          staff_id: string | null
          appointment_date: string
          start_time: string
          end_time: string
          status: 'pending' | 'confirmed' | 'completed' | 'cancelled' | 'no_show'
          price: number
          deposit_paid: number | null
          payment_status: 'pending' | 'paid' | 'refunded'
          payment_intent_id: string | null
          customer_notes: string | null
          provider_notes: string | null
          cancelled_by: string | null
          cancelled_at: string | null
          cancellation_reason: string | null
          created_at: string
          updated_at: string
        }
        Insert: {
          id?: string
          booking_reference?: string
          customer_id: string
          provider_id: string
          location_id: string
          service_id: string
          staff_id?: string | null
          appointment_date: string
          start_time: string
          end_time: string
          status?: 'pending' | 'confirmed' | 'completed' | 'cancelled' | 'no_show'
          price: number
          deposit_paid?: number | null
          payment_status?: 'pending' | 'paid' | 'refunded'
          payment_intent_id?: string | null
          customer_notes?: string | null
          provider_notes?: string | null
          cancelled_by?: string | null
          cancelled_at?: string | null
          cancellation_reason?: string | null
          created_at?: string
          updated_at?: string
        }
        Update: {
          id?: string
          booking_reference?: string
          customer_id?: string
          provider_id?: string
          location_id?: string
          service_id?: string
          staff_id?: string | null
          appointment_date?: string
          start_time?: string
          end_time?: string
          status?: 'pending' | 'confirmed' | 'completed' | 'cancelled' | 'no_show'
          price?: number
          deposit_paid?: number | null
          payment_status?: 'pending' | 'paid' | 'refunded'
          payment_intent_id?: string | null
          customer_notes?: string | null
          provider_notes?: string | null
          cancelled_by?: string | null
          cancelled_at?: string | null
          cancellation_reason?: string | null
          created_at?: string
          updated_at?: string
        }
      }
      reviews: {
        Row: {
          id: string
          appointment_id: string
          customer_id: string
          provider_id: string
          rating: number
          comment: string | null
          response: string | null
          response_at: string | null
          is_verified: boolean
          created_at: string
          updated_at: string
        }
        Insert: {
          id?: string
          appointment_id: string
          customer_id: string
          provider_id: string
          rating: number
          comment?: string | null
          response?: string | null
          response_at?: string | null
          is_verified?: boolean
          created_at?: string
          updated_at?: string
        }
        Update: {
          id?: string
          appointment_id?: string
          customer_id?: string
          provider_id?: string
          rating?: number
          comment?: string | null
          response?: string | null
          response_at?: string | null
          is_verified?: boolean
          created_at?: string
          updated_at?: string
        }
      }
      promotions: {
        Row: {
          id: string
          provider_id: string
          service_id: string | null
          title: string
          description: string | null
          discount_type: 'percentage' | 'fixed_amount'
          discount_value: number
          valid_from: string
          valid_until: string
          max_uses: number | null
          current_uses: number
          min_notice_hours: number
          is_active: boolean
          created_at: string
          updated_at: string
        }
        Insert: {
          id?: string
          provider_id: string
          service_id?: string | null
          title: string
          description?: string | null
          discount_type: 'percentage' | 'fixed_amount'
          discount_value: number
          valid_from: string
          valid_until: string
          max_uses?: number | null
          current_uses?: number
          min_notice_hours?: number
          is_active?: boolean
          created_at?: string
          updated_at?: string
        }
        Update: {
          id?: string
          provider_id?: string
          service_id?: string | null
          title?: string
          description?: string | null
          discount_type?: 'percentage' | 'fixed_amount'
          discount_value?: number
          valid_from?: string
          valid_until?: string
          max_uses?: number | null
          current_uses?: number
          min_notice_hours?: number
          is_active?: boolean
          created_at?: string
          updated_at?: string
        }
      }
      favorites: {
        Row: {
          id: string
          customer_id: string
          provider_id: string
          created_at: string
        }
        Insert: {
          id?: string
          customer_id: string
          provider_id: string
          created_at?: string
        }
        Update: {
          id?: string
          customer_id?: string
          provider_id?: string
          created_at?: string
        }
      }
      notifications: {
        Row: {
          id: string
          user_id: string
          type: 'booking_confirmed' | 'booking_reminder' | 'booking_cancelled' | 'review_request' | 'promotion'
          title: string
          message: string
          data: Json | null
          read: boolean
          created_at: string
        }
        Insert: {
          id?: string
          user_id: string
          type: 'booking_confirmed' | 'booking_reminder' | 'booking_cancelled' | 'review_request' | 'promotion'
          title: string
          message: string
          data?: Json | null
          read?: boolean
          created_at?: string
        }
        Update: {
          id?: string
          user_id?: string
          type?: 'booking_confirmed' | 'booking_reminder' | 'booking_cancelled' | 'review_request' | 'promotion'
          title?: string
          message?: string
          data?: Json | null
          read?: boolean
          created_at?: string
        }
      }
      staff_services: {
        Row: {
          staff_id: string
          service_id: string
        }
        Insert: {
          staff_id: string
          service_id: string
        }
        Update: {
          staff_id?: string
          service_id?: string
        }
      }
      availability_schedules: {
        Row: {
          id: string
          provider_id: string
          staff_id: string | null
          location_id: string
          day_of_week: number
          start_time: string
          end_time: string
          is_active: boolean
          created_at: string
        }
        Insert: {
          id?: string
          provider_id: string
          staff_id?: string | null
          location_id: string
          day_of_week: number
          start_time: string
          end_time: string
          is_active?: boolean
          created_at?: string
        }
        Update: {
          id?: string
          provider_id?: string
          staff_id?: string | null
          location_id?: string
          day_of_week?: number
          start_time?: string
          end_time?: string
          is_active?: boolean
          created_at?: string
        }
      }
      availability_exceptions: {
        Row: {
          id: string
          provider_id: string
          staff_id: string | null
          location_id: string
          date: string
          start_time: string | null
          end_time: string | null
          reason: string | null
          created_at: string
        }
        Insert: {
          id?: string
          provider_id: string
          staff_id?: string | null
          location_id: string
          date: string
          start_time?: string | null
          end_time?: string | null
          reason?: string | null
          created_at?: string
        }
        Update: {
          id?: string
          provider_id?: string
          staff_id?: string | null
          location_id?: string
          date?: string
          start_time?: string | null
          end_time?: string | null
          reason?: string | null
          created_at?: string
        }
      }
    }
    Enums: {
      user_type: 'customer' | 'provider' | 'admin'
      appointment_status: 'pending' | 'confirmed' | 'completed' | 'cancelled' | 'no_show'
      payment_status: 'pending' | 'paid' | 'refunded'
      discount_type: 'percentage' | 'fixed_amount'
    }
  }
}
