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
      <header className="bg-white border-b border-gray-200">
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-4">
          <div className="flex justify-between items-center">
            <Link href="/">
              <h1 className="text-2xl font-bold text-blue-600 cursor-pointer hover:text-blue-700 transition-colors">
                EmptySlot
              </h1>
            </Link>
            <div className="flex items-center gap-4">
              <Link href="/provider/dashboard">
                <Button variant="ghost" className="text-gray-700">
                  For Providers
                </Button>
              </Link>
              <Link href="/auth/login">
                <Button className="bg-blue-600 hover:bg-blue-700">Sign In</Button>
              </Link>
            </div>
          </div>
        </div>
      </header>

      {/* Search Hero Section */}
      <section className="bg-gradient-to-r from-blue-600 to-blue-800 text-white">
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-12">
          <h1 className="text-4xl md:text-5xl font-bold mb-4">
            Find Your Perfect Service
          </h1>
          <p className="text-xl text-blue-100 mb-6">
            Book last-minute appointments with verified providers near you
          </p>

          {/* Quick search bar */}
          <div className="bg-white rounded-2xl shadow-xl p-2 flex flex-col md:flex-row gap-2 max-w-4xl">
            <select
              value={categoryId}
              onChange={(e) => setCategoryId(e.target.value)}
              className="flex-1 px-4 py-3 rounded-xl text-gray-900 border-0 focus:outline-none focus:ring-2 focus:ring-blue-500"
            >
              <option value="">All Categories</option>
              {categories.map((cat) => (
                <option key={cat.id} value={cat.id}>
                  {cat.icon} {cat.name}
                </option>
              ))}
            </select>
            <input
              type="text"
              placeholder="Calgary, AB"
              value={location}
              onChange={(e) => setLocation(e.target.value)}
              className="flex-1 px-4 py-3 rounded-xl text-gray-900 border-0 focus:outline-none focus:ring-2 focus:ring-blue-500"
            />
            <Button
              onClick={handleSearch}
              disabled={searchLoading}
              className="bg-red-600 hover:bg-red-700 text-white font-semibold px-8 py-3 text-lg rounded-xl"
            >
              {searchLoading ? 'Searching...' : "Let's go"}
            </Button>
          </div>
        </div>
      </section>

      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
        <div className="grid lg:grid-cols-4 gap-8">
          {/* Filters Sidebar */}
          <aside className="lg:col-span-1">
            <div className="sticky top-4">
              <Card className="border-gray-200 shadow-md">
                <CardHeader className="pb-4">
                  <CardTitle className="text-lg">Refine Your Search</CardTitle>
                  <CardDescription>Filter by preferences</CardDescription>
                </CardHeader>
                <CardContent className="space-y-5">
                  {/* Rating */}
                  <div className="space-y-2">
                    <Label htmlFor="rating" className="text-sm font-semibold text-gray-700">
                      Minimum Rating
                    </Label>
                    <select
                      id="rating"
                      value={minRating}
                      onChange={(e) => setMinRating(Number(e.target.value))}
                      className="flex h-10 w-full rounded-lg border border-gray-300 bg-white px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent"
                    >
                      <option value="0">⭐ Any Rating</option>
                      <option value="3">⭐ 3+ Stars</option>
                      <option value="4">⭐ 4+ Stars</option>
                      <option value="4.5">⭐ 4.5+ Stars</option>
                    </select>
                  </div>

                  {/* Distance */}
                  <div className="space-y-3">
                    <div className="flex justify-between items-center">
                      <Label htmlFor="distance" className="text-sm font-semibold text-gray-700">
                        Distance
                      </Label>
                      <span className="text-sm font-bold text-blue-600">
                        {radiusKm} km
                      </span>
                    </div>
                    <input
                      id="distance"
                      type="range"
                      min="1"
                      max="50"
                      value={radiusKm}
                      onChange={(e) => setRadiusKm(Number(e.target.value))}
                      className="w-full h-2 bg-gray-200 rounded-lg appearance-none cursor-pointer accent-blue-600"
                    />
                    <div className="flex justify-between text-xs text-gray-500">
                      <span>1 km</span>
                      <span>50 km</span>
                    </div>
                  </div>

                  {/* Divider */}
                  <div className="border-t border-gray-200 pt-4">
                    <div className="space-y-3">
                      {/* Apply Button */}
                      <Button
                        className="w-full bg-blue-600 hover:bg-blue-700 text-white font-semibold"
                        onClick={handleSearch}
                        disabled={searchLoading}
                      >
                        {searchLoading ? (
                          <span className="flex items-center gap-2">
                            <div className="animate-spin h-4 w-4 border-2 border-white border-t-transparent rounded-full" />
                            Searching...
                          </span>
                        ) : (
                          'Apply Filters'
                        )}
                      </Button>

                      {/* Reset */}
                      <Button
                        variant="outline"
                        className="w-full border-gray-300 text-gray-700 hover:bg-gray-50"
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
                    </div>
                  </div>
                </CardContent>
              </Card>
            </div>
          </aside>

          {/* Results */}
          <main className="lg:col-span-3">
            {/* Results Header */}
            <div className="mb-6 pb-4 border-b border-gray-200">
              <div className="flex items-center justify-between">
                <div>
                  <h2 className="text-3xl font-bold text-gray-900">
                    {providers.length} {providers.length === 1 ? 'Provider' : 'Providers'}
                  </h2>
                  <p className="text-gray-600 mt-1">
                    Available for last-minute appointments
                  </p>
                </div>
                {/* Active filters count */}
                {(categoryId || minRating > 0 || radiusKm !== 10) && (
                  <div className="text-sm text-blue-600 font-medium">
                    {[categoryId, minRating > 0, radiusKm !== 10].filter(Boolean).length} filter(s) active
                  </div>
                )}
              </div>
            </div>

            {/* Results Grid */}
            {searchLoading ? (
              <div className="flex flex-col items-center justify-center py-20">
                <div className="animate-spin rounded-full h-16 w-16 border-4 border-blue-600 border-t-transparent"></div>
                <p className="mt-4 text-gray-600 font-medium">Finding the best providers...</p>
              </div>
            ) : providers.length === 0 ? (
              <Card className="border-gray-200">
                <CardContent className="py-20 text-center">
                  <div className="text-6xl mb-4">🔍</div>
                  <h3 className="text-xl font-bold text-gray-900 mb-2">
                    No providers found
                  </h3>
                  <p className="text-gray-600 mb-6">
                    Try adjusting your filters or search criteria
                  </p>
                  <Button
                    className="bg-blue-600 hover:bg-blue-700"
                    onClick={() => {
                      setLocation('')
                      setCategoryId('')
                      setMinRating(0)
                      setRadiusKm(10)
                      loadInitialData()
                    }}
                  >
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
