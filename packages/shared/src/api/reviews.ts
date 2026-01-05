/**
 * Reviews API
 * Functions for managing customer reviews and ratings
 */

import { authApi } from '../lib/apiClient'

export interface CreateReviewData {
  appointmentId: string
  providerId: string
  rating: number
  comment?: string
}

export interface ReviewResponse {
  id: string
  appointmentId: string
  providerId: string
  customerId: string
  rating: number
  comment: string | null
  response: string | null
  responseAt: string | null
  isVerified: boolean
  createdAt: string
  customer?: {
    fullName: string | null
    avatarUrl: string | null
  }
  appointment?: {
    appointmentDate: string
    service?: {
      name: string
    }
  }
}

/**
 * Create a review for a completed appointment
 */
export async function createReview(data: CreateReviewData): Promise<ReviewResponse> {
  return authApi.post<ReviewResponse>('/reviews', {
    appointmentId: data.appointmentId,
    providerId: data.providerId,
    rating: data.rating,
    comment: data.comment,
  })
}

/**
 * Get reviews for a provider
 */
export async function getProviderReviews(providerId: string): Promise<ReviewResponse[]> {
  return authApi.get<ReviewResponse[]>(`/providers/${providerId}/reviews`)
}

/**
 * Get review for a specific appointment
 */
export async function getAppointmentReview(appointmentId: string): Promise<ReviewResponse | null> {
  try {
    return await authApi.get<ReviewResponse>(`/appointments/${appointmentId}/review`)
  } catch (error: any) {
    if (error.status === 404) {
      return null
    }
    throw error
  }
}

/**
 * Update provider's response to a review
 */
export async function respondToReview(reviewId: string, response: string): Promise<ReviewResponse> {
  return authApi.put<ReviewResponse>(`/reviews/${reviewId}/respond`, { response })
}

/**
 * Get all reviews for current user's provider
 */
export async function getMyProviderReviews(): Promise<ReviewResponse[]> {
  return authApi.get<ReviewResponse[]>('/reviews/my-provider')
}
