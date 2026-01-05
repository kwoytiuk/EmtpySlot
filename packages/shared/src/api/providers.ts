/**
 * Providers API
 * Functions for searching and managing service providers
 */

import { get, post, put, authApi } from '../lib/apiClient'

export interface ProviderLocation {
  id: string
  city: string
  stateProvince: string
  latitude: number
  longitude: number
  isPrimary: boolean
}

export interface Service {
  id: string
  name: string
  description?: string
  price: number
  durationMinutes: number
  categoryId: string
  category?: {
    id: string
    name: string
    slug: string
  }
}

export interface Provider {
  id: string
  userId: string
  businessName: string
  description: string | null
  phone: string | null
  email: string | null
  website: string | null
  logoUrl: string | null
  coverImageUrl: string | null
  verified: boolean
  ratingAverage: number
  ratingCount: number
  cancellationPolicy: string | null
  createdAt: string
  updatedAt: string
  locations?: ProviderLocation[]
  services?: Service[]
}

export interface SearchProvidersParams {
  latitude?: number
  longitude?: number
  radiusKm?: number
  categoryId?: string
  minRating?: number
  verified?: boolean
  limit?: number
  offset?: number
}

export interface SearchProvidersResponse {
  providers: Provider[]
  total: number
}

export interface CreateProviderData {
  businessName: string
  description?: string
  phone?: string
  email?: string
  website?: string
  cancellationPolicy?: string
}

/**
 * Search for providers with geospatial filtering
 */
export async function searchProviders(params: SearchProvidersParams): Promise<SearchProvidersResponse> {
  const queryParams = new URLSearchParams()

  if (params.latitude !== undefined) queryParams.append('latitude', params.latitude.toString())
  if (params.longitude !== undefined) queryParams.append('longitude', params.longitude.toString())
  if (params.radiusKm !== undefined) queryParams.append('radiusKm', params.radiusKm.toString())
  if (params.categoryId) queryParams.append('categoryId', params.categoryId)
  if (params.minRating !== undefined) queryParams.append('minRating', params.minRating.toString())
  if (params.verified !== undefined) queryParams.append('verified', params.verified.toString())
  if (params.limit !== undefined) queryParams.append('limit', params.limit.toString())
  if (params.offset !== undefined) queryParams.append('offset', params.offset.toString())

  const queryString = queryParams.toString()
  const endpoint = queryString ? `/providers/search?${queryString}` : '/providers/search'

  return get<SearchProvidersResponse>(endpoint)
}

/**
 * Get a single provider by ID with all related data
 */
export async function getProvider(id: string): Promise<Provider> {
  return get<Provider>(`/providers/${id}`)
}

export interface Review {
  id: string
  rating: number
  comment: string | null
  response: string | null
  createdAt: string
  customer?: {
    fullName: string | null
    avatarUrl: string | null
  }
}

/**
 * Get provider reviews separately for better performance
 */
export async function getProviderReviews(providerId: string, limit: number = 10): Promise<Review[]> {
  return get<Review[]>(`/providers/${providerId}/reviews?limit=${limit}`)
}

/**
 * Create a new provider profile (for current user)
 */
export async function createProvider(providerData: CreateProviderData): Promise<Provider> {
  return authApi.post<Provider>('/providers', providerData)
}

/**
 * Update provider profile
 */
export async function updateProvider(
  id: string,
  updates: Partial<CreateProviderData>
): Promise<Provider> {
  return authApi.put<Provider>(`/providers/${id}`, updates)
}

/**
 * Get provider for current user
 */
export async function getMyProvider(): Promise<Provider | null> {
  try {
    return await authApi.get<Provider>('/providers/me')
  } catch (error: any) {
    if (error.status === 404) {
      return null
    }
    throw error
  }
}
