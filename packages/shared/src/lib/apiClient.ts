/**
 * .NET API Client
 * Centralized HTTP client for making requests to the .NET backend
 */

import { env } from '../config/env'

export interface ApiError {
  message: string
  status: number
  errors?: Record<string, string[]>
}

export class ApiClientError extends Error {
  constructor(
    message: string,
    public status: number,
    public errors?: Record<string, string[]>
  ) {
    super(message)
    this.name = 'ApiClientError'
  }
}

/**
 * Generic API request function
 */
export async function apiRequest<T>(
  endpoint: string,
  options?: RequestInit & { token?: string }
): Promise<T> {
  const { token, ...fetchOptions } = options || {}

  const headers: HeadersInit = {
    'Content-Type': 'application/json',
    ...fetchOptions.headers,
  }

  // Add authorization token if provided
  if (token) {
    headers['Authorization'] = `Bearer ${token}`
  }

  const url = `${env.api.url}${endpoint}`

  try {
    const response = await fetch(url, {
      ...fetchOptions,
      headers,
    })

    // Handle non-JSON responses (like 204 No Content)
    if (response.status === 204) {
      return undefined as T
    }

    const data = await response.json()

    if (!response.ok) {
      throw new ApiClientError(
        data.message || data.title || 'An error occurred',
        response.status,
        data.errors
      )
    }

    return data as T
  } catch (error) {
    if (error instanceof ApiClientError) {
      throw error
    }

    // Network or other errors
    if (error instanceof Error) {
      throw new ApiClientError(error.message, 0)
    }

    throw new ApiClientError('An unknown error occurred', 0)
  }
}

/**
 * GET request
 */
export async function get<T>(
  endpoint: string,
  options?: RequestInit & { token?: string }
): Promise<T> {
  return apiRequest<T>(endpoint, {
    ...options,
    method: 'GET',
  })
}

/**
 * POST request
 */
export async function post<T>(
  endpoint: string,
  body?: unknown,
  options?: RequestInit & { token?: string }
): Promise<T> {
  return apiRequest<T>(endpoint, {
    ...options,
    method: 'POST',
    body: body ? JSON.stringify(body) : undefined,
  })
}

/**
 * PUT request
 */
export async function put<T>(
  endpoint: string,
  body?: unknown,
  options?: RequestInit & { token?: string }
): Promise<T> {
  return apiRequest<T>(endpoint, {
    ...options,
    method: 'PUT',
    body: body ? JSON.stringify(body) : undefined,
  })
}

/**
 * PATCH request
 */
export async function patch<T>(
  endpoint: string,
  body?: unknown,
  options?: RequestInit & { token?: string }
): Promise<T> {
  return apiRequest<T>(endpoint, {
    ...options,
    method: 'PATCH',
    body: body ? JSON.stringify(body) : undefined,
  })
}

/**
 * DELETE request
 */
export async function del<T>(
  endpoint: string,
  options?: RequestInit & { token?: string }
): Promise<T> {
  return apiRequest<T>(endpoint, {
    ...options,
    method: 'DELETE',
  })
}

/**
 * Token storage helper
 * Store and retrieve auth tokens
 */
class TokenStorage {
  private token: string | null = null

  set(token: string) {
    this.token = token
    if (typeof localStorage !== 'undefined') {
      localStorage.setItem('auth_token', token)
    }
  }

  get(): string | null {
    if (this.token) return this.token

    if (typeof localStorage !== 'undefined') {
      this.token = localStorage.getItem('auth_token')
    }

    return this.token
  }

  clear() {
    this.token = null
    if (typeof localStorage !== 'undefined') {
      localStorage.removeItem('auth_token')
    }
  }
}

export const tokenStorage = new TokenStorage()

/**
 * Authenticated request helpers
 * Automatically include the stored auth token
 */
export const authApi = {
  get: <T>(endpoint: string, options?: RequestInit) =>
    get<T>(endpoint, { ...options, token: tokenStorage.get() || undefined }),

  post: <T>(endpoint: string, body?: unknown, options?: RequestInit) =>
    post<T>(endpoint, body, { ...options, token: tokenStorage.get() || undefined }),

  put: <T>(endpoint: string, body?: unknown, options?: RequestInit) =>
    put<T>(endpoint, body, { ...options, token: tokenStorage.get() || undefined }),

  patch: <T>(endpoint: string, body?: unknown, options?: RequestInit) =>
    patch<T>(endpoint, body, { ...options, token: tokenStorage.get() || undefined }),

  delete: <T>(endpoint: string, options?: RequestInit) =>
    del<T>(endpoint, { ...options, token: tokenStorage.get() || undefined }),
}
