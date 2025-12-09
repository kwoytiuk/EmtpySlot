'use client'

import { useEffect, useState } from 'react'
import { useParams, useRouter } from 'next/navigation'
import Link from 'next/link'
import { providersApi } from 'shared'
import { Button } from '@/components/ui/button'
import {
  Card,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle,
} from '@/components/ui/card'
import { useAuth } from '@/contexts/AuthContext'
import { QuickBooking } from '@/components/booking/QuickBooking'

export default function ProviderDetailPage() {
  const params = useParams()
  const router = useRouter()
  const { user } = useAuth()
  const [provider, setProvider] = useState<any>(null)
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState('')

  useEffect(() => {
    if (params.id) {
      loadProvider(params.id as string)
    }
  }, [params.id])

  const loadProvider = async (id: string) => {
    try {
      const data = await providersApi.getProvider(id)
      setProvider(data)
    } catch (err: any) {
      setError(err.message || 'Failed to load provider')
    } finally {
      setLoading(false)
    }
  }

  const handleBookNow = (serviceId: string) => {
    if (!user) {
      router.push(`/auth/login?redirect=/booking?provider=${params.id}&service=${serviceId}`)
      return
    }
    router.push(`/booking?provider=${params.id}&service=${serviceId}`)
  }

  if (loading) {
    return (
      <div className="min-h-screen flex items-center justify-center">
        <div className="text-center">
          <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-primary mx-auto"></div>
          <p className="mt-4 text-muted-foreground">Loading provider...</p>
        </div>
      </div>
    )
  }

  if (error || !provider) {
    return (
      <div className="min-h-screen flex items-center justify-center">
        <Card className="w-full max-w-md">
          <CardHeader>
            <CardTitle>Provider Not Found</CardTitle>
            <CardDescription>{error || 'This provider does not exist'}</CardDescription>
          </CardHeader>
          <CardContent>
            <Link href="/search">
              <Button className="w-full">Back to Search</Button>
            </Link>
          </CardContent>
        </Card>
      </div>
    )
  }

  const primaryLocation = provider.provider_locations?.find((loc: any) => loc.is_primary) ||
    provider.provider_locations?.[0]

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
            <Link href="/search">
              <Button variant="outline">← Back to Search</Button>
            </Link>
          </div>
        </div>
      </header>

      <main className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
        <div className="grid lg:grid-cols-3 gap-8">
          {/* Main Content */}
          <div className="lg:col-span-2 space-y-6">
            {/* Quick Booking - OpenTable Style */}
            {provider.services && provider.services.length > 0 && (
              <QuickBooking
                providerId={provider.id}
                services={provider.services.map((s: any) => ({
                  id: s.id,
                  name: s.name,
                  price: s.price,
                  duration_minutes: s.duration_minutes,
                }))}
              />
            )}

            {/* Provider Info */}
            <Card>
              <CardHeader>
                <div className="flex justify-between items-start">
                  <div>
                    <CardTitle className="text-3xl">
                      {provider.business_name}
                    </CardTitle>
                    {provider.verified && (
                      <span className="inline-block mt-2 bg-primary/10 text-primary text-sm px-3 py-1 rounded-full">
                        ✓ Verified Provider
                      </span>
                    )}
                  </div>
                </div>
              </CardHeader>
              <CardContent className="space-y-4">
                {/* Rating */}
                <div className="flex items-center gap-3">
                  <div className="flex items-center">
                    {[...Array(5)].map((_, i) => (
                      <span
                        key={i}
                        className={`text-2xl ${
                          i < Math.floor(provider.rating_average || 0)
                            ? 'text-yellow-400'
                            : 'text-gray-300'
                        }`}
                      >
                        ★
                      </span>
                    ))}
                  </div>
                  <span className="text-lg font-medium">
                    {provider.rating_average ? provider.rating_average.toFixed(1) : 'No reviews'}
                  </span>
                  {provider.rating_count > 0 && (
                    <span className="text-muted-foreground">
                      ({provider.rating_count} reviews)
                    </span>
                  )}
                </div>

                {/* Description */}
                {provider.description && (
                  <div>
                    <h3 className="font-semibold mb-2">About</h3>
                    <p className="text-gray-600">{provider.description}</p>
                  </div>
                )}

                {/* Contact Info */}
                <div className="space-y-2">
                  <h3 className="font-semibold">Contact Information</h3>
                  <div className="space-y-1 text-sm">
                    {provider.phone && (
                      <p className="flex items-center gap-2">
                        <span className="text-muted-foreground">Phone:</span>
                        <span>{provider.phone}</span>
                      </p>
                    )}
                    {provider.email && (
                      <p className="flex items-center gap-2">
                        <span className="text-muted-foreground">Email:</span>
                        <span>{provider.email}</span>
                      </p>
                    )}
                    {provider.website && (
                      <p className="flex items-center gap-2">
                        <span className="text-muted-foreground">Website:</span>
                        <a
                          href={provider.website}
                          target="_blank"
                          rel="noopener noreferrer"
                          className="text-primary hover:underline"
                        >
                          {provider.website}
                        </a>
                      </p>
                    )}
                  </div>
                </div>

                {/* Location */}
                {primaryLocation && (
                  <div>
                    <h3 className="font-semibold mb-2">Location</h3>
                    <div className="text-sm space-y-1">
                      <p>{primaryLocation.address_line1}</p>
                      {primaryLocation.address_line2 && (
                        <p>{primaryLocation.address_line2}</p>
                      )}
                      <p>
                        {primaryLocation.city}, {primaryLocation.state_province}{' '}
                        {primaryLocation.postal_code}
                      </p>
                    </div>
                  </div>
                )}
              </CardContent>
            </Card>

            {/* Services */}
            <Card>
              <CardHeader>
                <CardTitle>Services Offered</CardTitle>
              </CardHeader>
              <CardContent>
                {provider.services && provider.services.length > 0 ? (
                  <div className="space-y-4">
                    {provider.services.map((service: any) => (
                      <div
                        key={service.id}
                        className="flex justify-between items-start p-4 border rounded-lg hover:border-primary transition-colors"
                      >
                        <div className="flex-1">
                          <h4 className="font-semibold">{service.name}</h4>
                          {service.description && (
                            <p className="text-sm text-gray-600 mt-1">
                              {service.description}
                            </p>
                          )}
                          <div className="flex items-center gap-4 mt-2 text-sm text-muted-foreground">
                            <span>⏱ {service.duration_minutes} min</span>
                            <span className="font-semibold text-foreground">
                              ${service.price.toFixed(2)}
                            </span>
                          </div>
                        </div>
                        <Button
                          onClick={() => handleBookNow(service.id)}
                          className="ml-4"
                        >
                          Book Now
                        </Button>
                      </div>
                    ))}
                  </div>
                ) : (
                  <p className="text-center py-8 text-muted-foreground">
                    No services available at this time
                  </p>
                )}
              </CardContent>
            </Card>

            {/* Reviews */}
            <Card>
              <CardHeader>
                <CardTitle>Customer Reviews</CardTitle>
              </CardHeader>
              <CardContent>
                {provider.reviews && provider.reviews.length > 0 ? (
                  <div className="space-y-4">
                    {provider.reviews.slice(0, 5).map((review: any) => (
                      <div key={review.id} className="border-b last:border-0 pb-4 last:pb-0">
                        <div className="flex items-center gap-2 mb-2">
                          <div className="flex">
                            {[...Array(5)].map((_, i) => (
                              <span
                                key={i}
                                className={`text-sm ${
                                  i < review.rating
                                    ? 'text-yellow-400'
                                    : 'text-gray-300'
                                }`}
                              >
                                ★
                              </span>
                            ))}
                          </div>
                          <span className="text-sm font-medium">
                            {review.profiles?.full_name || 'Anonymous'}
                          </span>
                        </div>
                        {review.comment && (
                          <p className="text-sm text-gray-600">{review.comment}</p>
                        )}
                        {review.response && (
                          <div className="mt-2 ml-4 p-3 bg-gray-50 rounded-lg">
                            <p className="text-sm font-medium text-primary mb-1">
                              Response from {provider.business_name}
                            </p>
                            <p className="text-sm text-gray-600">{review.response}</p>
                          </div>
                        )}
                      </div>
                    ))}
                  </div>
                ) : (
                  <p className="text-center py-8 text-muted-foreground">
                    No reviews yet. Be the first to leave a review!
                  </p>
                )}
              </CardContent>
            </Card>
          </div>

          {/* Sidebar */}
          <aside className="lg:col-span-1">
            <Card className="sticky top-4">
              <CardHeader>
                <CardTitle>Quick Actions</CardTitle>
              </CardHeader>
              <CardContent className="space-y-3">
                <Button className="w-full" size="lg">
                  Book Appointment
                </Button>
                <Button variant="outline" className="w-full">
                  Save to Favorites
                </Button>
                <Button variant="outline" className="w-full">
                  Share Provider
                </Button>
              </CardContent>
            </Card>

            {/* Business Hours (Placeholder) */}
            <Card className="mt-6">
              <CardHeader>
                <CardTitle>Business Hours</CardTitle>
              </CardHeader>
              <CardContent className="text-sm space-y-1">
                <div className="flex justify-between">
                  <span>Monday</span>
                  <span className="text-muted-foreground">9:00 AM - 6:00 PM</span>
                </div>
                <div className="flex justify-between">
                  <span>Tuesday</span>
                  <span className="text-muted-foreground">9:00 AM - 6:00 PM</span>
                </div>
                <div className="flex justify-between">
                  <span>Wednesday</span>
                  <span className="text-muted-foreground">9:00 AM - 6:00 PM</span>
                </div>
                <div className="flex justify-between">
                  <span>Thursday</span>
                  <span className="text-muted-foreground">9:00 AM - 6:00 PM</span>
                </div>
                <div className="flex justify-between">
                  <span>Friday</span>
                  <span className="text-muted-foreground">9:00 AM - 6:00 PM</span>
                </div>
                <div className="flex justify-between">
                  <span>Saturday</span>
                  <span className="text-muted-foreground">10:00 AM - 4:00 PM</span>
                </div>
                <div className="flex justify-between">
                  <span>Sunday</span>
                  <span className="text-muted-foreground">Closed</span>
                </div>
              </CardContent>
            </Card>
          </aside>
        </div>
      </main>
    </div>
  )
}
