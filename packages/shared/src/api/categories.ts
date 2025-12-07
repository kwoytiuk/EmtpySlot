/**
 * Service Categories API
 */

import { supabase } from '../lib/supabase'

/**
 * Get all service categories
 */
export async function getCategories() {
  const { data, error } = await supabase
    .from('service_categories')
    .select('*')
    .order('display_order', { ascending: true })

  if (error) throw error

  return data
}

/**
 * Get a single category by ID
 */
export async function getCategory(id: string) {
  const { data, error } = await supabase
    .from('service_categories')
    .select('*')
    .eq('id', id)
    .single()

  if (error) throw error

  return data
}

/**
 * Get a category by slug
 */
export async function getCategoryBySlug(slug: string) {
  const { data, error} = await supabase
    .from('service_categories')
    .select('*')
    .eq('slug', slug)
    .single()

  if (error) throw error

  return data
}
