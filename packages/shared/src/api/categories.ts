/**
 * Service Categories API
 */

import { get } from '../lib/apiClient'

export interface ServiceCategory {
  id: string
  name: string
  slug: string
  icon: string | null
  description: string | null
  parentId: string | null
  displayOrder: number
  createdAt: string
}

/**
 * Get all service categories
 */
export async function getCategories(): Promise<ServiceCategory[]> {
  return get<ServiceCategory[]>('/categories')
}

/**
 * Get a single category by ID
 */
export async function getCategory(id: string): Promise<ServiceCategory> {
  return get<ServiceCategory>(`/categories/${id}`)
}

/**
 * Get a category by slug
 */
export async function getCategoryBySlug(slug: string): Promise<ServiceCategory> {
  return get<ServiceCategory>(`/categories/slug/${slug}`)
}
