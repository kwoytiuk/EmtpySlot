'use client'

import { useState, useEffect } from 'react'
import { providersApi, categoriesApi, type ServiceCategory } from 'shared'
import { ProviderCard } from '@/components/search/ProviderCard'
import { Input } from '@/components/ui/input'
import { Button } from '@/components/ui/button'
import { Label } from '@/components/ui/label'
import {
  Card,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle,
} from '@/components/ui/card'
import Link from 'next/link'

export default function SearchPage() {
  const [providers, setProviders] = useState<any[]>([])
  const [categories, setCategories] = useState<ServiceCategory[]>([])
  const [loading, setLoading] = useState(true)
  const [searchLoading, setSearchLoading] = useState(false)

  // Search filters
  const [location, setLocation] = useState('')
  const [categoryId, setCategoryId] = useState('')
  const [minRating, setMinRating] = useState(0)
  const [radiusKm, setRadiusKm] = useState(10)

  useEffect(() => {
    loadInitialData()
  }, [])

  const loadInitialData = async () => {
    try {
      const [categoriesData, providersData] = await Promise.all([
        categoriesApi.getCategories(),
        providersApi.searchProviders({ verified: true, limit: 20 }),
      ])

      setCategories(categoriesData)
      setProviders(providersData as any[])
    } catch (error) {
      console.error('Error loading data:', error)
    } finally {
      setLoading(false)
    }
  }

  const handleSearch = async () => {
    setSearchLoading(true)
    try {
      const results = await providersApi.searchProviders({
        categoryId: categoryId || undefined,
        minRating: minRating || undefined,
        radiusKm: radiusKm || undefined,
        verified: true,
        limit: 50,
      })

      setProviders(results as any[])
    } catch (error) {
      console.error('Error searching:', error)
    } finally {
      setSearchLoading(false)
    }
  }

  if (loading) {
    return (
      <div className="min-h-screen flex items-center justify-center">
        <div className="text-center">
          <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-primary mx-auto"></div>
          <p className="mt-4 text-muted-foreground">Loading services...</p>
        </div>
      </div>
    )
  }

  return (
    <div className="min-h-screen bg-gray-50">
      {/* Header */}
      <header className="bg-white shadow-sm">
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-4">
          <div className="flex justify-between items-center">
            <Link href="/">
              <h1 className="text-2xl font-bold text-primary cursor-pointer">
                EmptySlot
              </h1>
            </Link>
            <Link href="/auth/login">
              <Button variant="outline">Sign In</Button>
            </Link>
          </div>
        </div>
      </header>

      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
        <div className="grid lg:grid-cols-4 gap-6">
          {/* Filters Sidebar */}
          <aside className="lg:col-span-1">
            <Card className="sticky top-4">
              <CardHeader>
                <CardTitle>Filters</CardTitle>
                <CardDescription>Refine your search</CardDescription>
              </CardHeader>
              <CardContent className="space-y-4">
                {/* Location */}
                <div className="space-y-2">
                  <Label htmlFor="location">Location</Label>
                  <Input
                    id="location"
                    placeholder="City or ZIP code"
                    value={location}
                    onChange={(e) => setLocation(e.target.value)}
                  />
                </div>

                {/* Category */}
                <div className="space-y-2">
                  <Label htmlFor="category">Category</Label>
                  <select
                    id="category"
                    value={categoryId}
                    onChange={(e) => setCategoryId(e.target.value)}
                    className="flex h-9 w-full rounded-md border border-input bg-transparent px-3 py-1 text-sm shadow-sm transition-colors focus-visible:outline-none focus-visible:ring-1 focus-visible:ring-ring"
                  >
                    <option value="">All Categories</option>
                    {categories.map((cat) => (
                      <option key={cat.id} value={cat.id}>
                        {cat.icon} {cat.name}
                      </option>
                    ))}
                  </select>
                </div>

                {/* Rating */}
                <div className="space-y-2">
                  <Label htmlFor="rating">Minimum Rating</Label>
                  <select
                    id="rating"
                    value={minRating}
                    onChange={(e) => setMinRating(Number(e.target.value))}
                    className="flex h-9 w-full rounded-md border border-input bg-transparent px-3 py-1 text-sm shadow-sm transition-colors focus-visible:outline-none focus-visible:ring-1 focus-visible:ring-ring"
                  >
                    <option value="0">Any Rating</option>
                    <option value="3">3+ Stars</option>
                    <option value="4">4+ Stars</option>
                    <option value="4.5">4.5+ Stars</option>
                  </select>
                </div>

                {/* Distance */}
                <div className="space-y-2">
                  <Label htmlFor="distance">
                    Distance: {radiusKm} km
                  </Label>
                  <input
                    id="distance"
                    type="range"
                    min="1"
                    max="50"
                    value={radiusKm}
                    onChange={(e) => setRadiusKm(Number(e.target.value))}
                    className="w-full"
                  />
                </div>

                {/* Search Button */}
                <Button
                  className="w-full"
                  onClick={handleSearch}
                  disabled={searchLoading}
                >
                  {searchLoading ? 'Searching...' : 'Search'}
                </Button>

                {/* Reset */}
                <Button
                  variant="outline"
                  className="w-full"
                  onClick={() => {
                    setLocation('')
                    setCategoryId('')
                    setMinRating(0)
                    setRadiusKm(10)
                    loadInitialData()
                  }}
                >
                  Reset Filters
                </Button>
              </CardContent>
            </Card>
          </aside>

          {/* Results */}
          <main className="lg:col-span-3">
            <div className="mb-6">
              <h2 className="text-2xl font-bold">
                {providers.length} Service Provider{providers.length !== 1 ? 's' : ''} Found
              </h2>
              <p className="text-muted-foreground mt-1">
                Discover last-minute appointments near you
              </p>
            </div>

            {searchLoading ? (
              <div className="flex items-center justify-center py-12">
                <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-primary"></div>
              </div>
            ) : providers.length === 0 ? (
              <Card>
                <CardContent className="py-12 text-center">
                  <p className="text-muted-foreground mb-4">
                    No providers found matching your criteria
                  </p>
                  <Button variant="outline" onClick={() => loadInitialData()}>
                    Show All Providers
                  </Button>
                </CardContent>
              </Card>
            ) : (
              <div className="grid md:grid-cols-2 gap-6">
                {providers.map((provider) => (
                  <ProviderCard key={provider.id} provider={provider} />
                ))}
              </div>
            )}
          </main>
        </div>
      </div>
    </div>
  )
}
