/**
 * Authentication API
 * Handles user authentication, registration, and session management
 */

import { supabase } from '../lib/supabase'
import type { UserType } from '../lib/supabase'

export interface SignUpData {
  email: string
  password: string
  fullName: string
  userType?: UserType
}

export interface SignInData {
  email: string
  password: string
}

/**
 * Sign up a new user
 */
export async function signUp(data: SignUpData) {
  const { email, password, fullName, userType = 'customer' } = data

  const { data: authData, error } = await supabase.auth.signUp({
    email,
    password,
    options: {
      data: {
        full_name: fullName,
        user_type: userType,
      },
    },
  })

  if (error) throw error

  return authData
}

/**
 * Sign in an existing user
 */
export async function signIn(data: SignInData) {
  const { email, password } = data

  const { data: authData, error } = await supabase.auth.signInWithPassword({
    email,
    password,
  })

  if (error) throw error

  return authData
}

/**
 * Sign out the current user
 */
export async function signOut() {
  const { error } = await supabase.auth.signOut()

  if (error) throw error
}

/**
 * Get the current user session
 */
export async function getSession() {
  const { data, error } = await supabase.auth.getSession()

  if (error) throw error

  return data.session
}

/**
 * Get the current user
 */
export async function getUser() {
  const { data, error } = await supabase.auth.getUser()

  if (error) throw error

  return data.user
}

/**
 * Get the current user's profile
 */
export async function getProfile() {
  const user = await getUser()

  if (!user) return null

  const { data, error } = await supabase
    .from('profiles')
    .select('*')
    .eq('id', user.id)
    .single()

  if (error) throw error

  return data
}

/**
 * Update the current user's profile
 */
export async function updateProfile(updates: {
  full_name?: string
  phone?: string
  avatar_url?: string
}) {
  const user = await getUser()

  if (!user) throw new Error('Not authenticated')

  const { data, error } = await supabase
    .from('profiles')
    .update(updates)
    .eq('id', user.id)
    .select()
    .single()

  if (error) throw error

  return data
}

/**
 * Send password reset email
 */
export async function resetPassword(email: string) {
  const { error } = await supabase.auth.resetPasswordForEmail(email, {
    redirectTo: `${window.location.origin}/auth/reset-password`,
  })

  if (error) throw error
}

/**
 * Update password
 */
export async function updatePassword(newPassword: string) {
  const { error } = await supabase.auth.updateUser({
    password: newPassword,
  })

  if (error) throw error
}

/**
 * Sign in with OAuth provider
 */
export async function signInWithProvider(provider: 'google' | 'apple') {
  const { error } = await supabase.auth.signInWithOAuth({
    provider,
    options: {
      redirectTo: `${window.location.origin}/auth/callback`,
    },
  })

  if (error) throw error
}
