/**
 * Authentication API
 * Handles user authentication, registration, and session management
 */

import { post, authApi, tokenStorage } from '../lib/apiClient'

export type UserType = 'customer' | 'provider' | 'admin'

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

export interface AuthResponse {
  token: string
  user: User
}

export interface User {
  id: string
  email: string
  fullName: string | null
  phone: string | null
  avatarUrl: string | null
  userType: UserType
  createdAt: string
  updatedAt: string
}

export interface Profile {
  id: string
  userType: UserType
  fullName: string | null
  phone: string | null
  avatarUrl: string | null
  createdAt: string
  updatedAt: string
}

/**
 * Sign up a new user
 */
export async function signUp(data: SignUpData): Promise<AuthResponse> {
  const response = await post<AuthResponse>('/auth/register', {
    email: data.email,
    password: data.password,
    fullName: data.fullName,
    userType: data.userType || 'customer',
  })

  // Store the token
  tokenStorage.set(response.token)

  return response
}

/**
 * Sign in an existing user
 */
export async function signIn(data: SignInData): Promise<AuthResponse> {
  const response = await post<AuthResponse>('/auth/login', {
    email: data.email,
    password: data.password,
  })

  // Store the token
  tokenStorage.set(response.token)

  return response
}

/**
 * Sign out the current user
 */
export async function signOut(): Promise<void> {
  tokenStorage.clear()
}

/**
 * Get the current user
 */
export async function getUser(): Promise<User | null> {
  try {
    return await authApi.get<User>('/auth/me')
  } catch (error: any) {
    if (error.status === 401) {
      return null
    }
    throw error
  }
}

/**
 * Get the current user's profile
 */
export async function getProfile(): Promise<Profile | null> {
  try {
    return await authApi.get<Profile>('/auth/profile')
  } catch (error: any) {
    if (error.status === 401 || error.status === 404) {
      return null
    }
    throw error
  }
}

/**
 * Update the current user's profile
 */
export async function updateProfile(updates: {
  fullName?: string
  phone?: string
  avatarUrl?: string
}): Promise<Profile> {
  return authApi.put<Profile>('/auth/profile', {
    fullName: updates.fullName,
    phone: updates.phone,
    avatarUrl: updates.avatarUrl,
  })
}

/**
 * Send password reset email
 */
export async function resetPassword(email: string): Promise<void> {
  await post('/auth/forgot-password', { email })
}

/**
 * Update password
 */
export async function updatePassword(newPassword: string): Promise<void> {
  await authApi.put('/auth/password', { newPassword })
}

/**
 * Sign in with OAuth provider
 */
export async function signInWithProvider(provider: 'google' | 'apple'): Promise<{ redirectUrl: string }> {
  return post<{ redirectUrl: string }>('/auth/oauth/url', { provider })
}

/**
 * Get the current session (check if token is valid)
 */
export async function getSession(): Promise<{ user: User } | null> {
  const user = await getUser()
  return user ? { user } : null
}
