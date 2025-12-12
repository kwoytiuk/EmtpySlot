'use client'

import { useRouter } from 'next/navigation'
import { Card } from '@/components/ui/card'
import { Button } from '@/components/ui/button'
import { ProviderQuickSlots } from './ProviderQuickSlots'
import type { TimeSlot } from 'shared'

interface ProviderCardProps {
  provider: any
  userLat?: number
  userLng?: number
  preloadedSlots?: TimeSlot[]
}

export function ProviderCard({ provider, userLat, userLng, preloadedSlots }: ProviderCardProps) {
  const router = useRouter()

  const primaryLocation = provider.provider_locations?.find((loc: any) => loc.is_primary) ||
    provider.provider_locations?.[0]

  // Calculate distance from user location
  const calculateDistance = (lat1: number, lon1: number, lat2: number, lon2: number): number => {
    const R = 6371 // Earth's radius in kilometers
    const dLat = toRad(lat2 - lat1)
    const dLon = toRad(lon2 - lon1)

    const a =
      Math.sin(dLat / 2) * Math.sin(dLat / 2) +
      Math.cos(toRad(lat1)) *
        Math.cos(toRad(lat2)) *
        Math.sin(dLon / 2) *
        Math.sin(dLon / 2)

    const c = 2 * Math.atan2(Math.sqrt(a), Math.sqrt(1 - a))
    const distance = R * c

    return distance
  }

  const toRad = (degrees: number): number => {
    return degrees * (Math.PI / 180)
  }

  // Calculate distance to primary location
  const distance = userLat && userLng && primaryLocation
    ? calculateDistance(userLat, userLng, primaryLocation.latitude, primaryLocation.longitude)
    : null

  // Calculate price range
  const minPrice = provider.services && provider.services.length > 0
    ? Math.min(...provider.services.map((s: any) => s.price))
    : null

  // Get category from first service
  const category = provider.services?.[0]?.service_categories?.slug ||
                   provider.services?.[0]?.categories?.slug ||
                   'service'

  // Get first service for quick booking
  const firstService = provider.services?.[0]

  // Get category-appropriate image
  const getCategoryImage = (categorySlug: string) => {
    const images: Record<string, string> = {
      'hair-salon': 'https://images.unsplash.com/photo-1562322140-8baeececf3df?w=800&h=400&fit=crop',
      'barber-shop': 'https://images.unsplash.com/photo-1503951914875-452162b0f3f1?w=800&h=400&fit=crop',
      'nail-salon': 'https://images.unsplash.com/photo-1604654894610-df63bc536371?w=800&h=400&fit=crop',
      'spa-massage': 'https://images.unsplash.com/photo-1544161515-4ab6ce6db874?w=800&h=400&fit=crop',
      'dental': 'https://images.unsplash.com/photo-1588776814546-1ffcf47267a5?w=800&h=400&fit=crop',
      'medical': 'https://images.unsplash.com/photo-1519494026892-80bbd2d6fd0d?w=800&h=400&fit=crop',
      'plumbing': 'https://images.unsplash.com/photo-1607472586893-edb57bdc0e39?w=800&h=400&fit=crop',
      'hvac': 'https://images.unsplash.com/photo-1581578731548-c64695cc6952?w=800&h=400&fit=crop',
      'electrical': 'https://images.unsplash.com/photo-1621905251918-48416bd8575a?w=800&h=400&fit=crop',
      'cleaning': 'https://images.unsplash.com/photo-1581578731548-c64695cc6952?w=800&h=400&fit=crop',
      'automotive': 'https://images.unsplash.com/photo-1486262715619-67b85e0b08d3?w=800&h=400&fit=crop',
      'fitness': 'https://images.unsplash.com/photo-1534438327276-14e5300c3a48?w=800&h=400&fit=crop',
      'beauty': 'https://images.unsplash.com/photo-1487412947147-5cebf100ffc2?w=800&h=400&fit=crop',
    }
    return images[categorySlug] || 'https://images.unsplash.com/photo-1556761175-b413da4baf72?w=800&h=400&fit=crop'
  }

  const categoryImage = getCategoryImage(category)

  const handleCardClick = (e: React.MouseEvent) => {
    // Only navigate if not clicking on interactive elements
    const target = e.target as HTMLElement
    if (
      target.tagName === 'BUTTON' ||
      target.closest('button') ||
      target.closest('[data-slot-container]')
    ) {
      return
    }
    router.push(`/providers/${provider.id}`)
  }

  return (
    <Card
      className="group hover:shadow-xl transition-all duration-300 cursor-pointer overflow-hidden border-gray-200"
      onClick={handleCardClick}
    >
        {/* Image with actual category photo */}
        <div className="relative h-48 overflow-hidden bg-gray-200">
          <img
            src={categoryImage}
            alt={provider.business_name}
            className="w-full h-full object-cover group-hover:scale-105 transition-transform duration-300"
            onError={(e) => {
              // Fallback to gradient if image fails to load
              e.currentTarget.style.display = 'none'
              e.currentTarget.parentElement!.style.background = 'linear-gradient(to bottom right, rgb(59, 130, 246), rgb(168, 85, 247), rgb(236, 72, 153))'
            }}
          />
          <div className="absolute inset-0 bg-gradient-to-t from-black/40 to-transparent" />

          {provider.verified && (
            <div className="absolute top-3 right-3 bg-white text-blue-600 text-xs font-semibold px-3 py-1.5 rounded-full shadow-lg flex items-center gap-1">
              ✓ Verified
            </div>
          )}

          {/* Category badge */}
          <div className="absolute bottom-3 left-3 bg-white/95 backdrop-blur-sm text-gray-800 text-xs font-medium px-3 py-1.5 rounded-full shadow-lg">
            {provider.services?.[0]?.service_categories?.name ||
             provider.services?.[0]?.categories?.name ||
             'Service'}
          </div>
        </div>

        {/* Content */}
        <div className="p-4">
          {/* Header */}
          <div className="mb-2">
            <h3 className="font-bold text-lg text-gray-900 group-hover:text-blue-600 transition-colors line-clamp-1">
              {provider.business_name}
            </h3>
            {primaryLocation && (
              <p className="text-sm text-gray-600 mt-0.5 flex items-center gap-2">
                <span>📍 {primaryLocation.city}, {primaryLocation.state_province}</span>
                {distance !== null && (
                  <span className="text-xs bg-blue-100 text-blue-700 px-2 py-0.5 rounded-full font-semibold">
                    {distance < 1
                      ? `${(distance * 1000).toFixed(0)}m`
                      : `${distance.toFixed(1)} km`
                    }
                  </span>
                )}
              </p>
            )}
          </div>

          {/* Rating */}
          <div className="flex items-center gap-2 mb-3">
            <div className="flex items-center bg-blue-600 text-white px-2 py-1 rounded-lg text-sm font-bold">
              ★ {provider.rating_average ? provider.rating_average.toFixed(1) : 'New'}
            </div>
            {provider.rating_count > 0 && (
              <span className="text-sm text-gray-600">
                ({provider.rating_count} {provider.rating_count === 1 ? 'review' : 'reviews'})
              </span>
            )}
          </div>

          {/* Description */}
          {provider.description && (
            <p className="text-sm text-gray-600 line-clamp-2 mb-3">
              {provider.description}
            </p>
          )}

          {/* Services Preview */}
          {provider.services && provider.services.length > 0 && (
            <div className="flex flex-wrap gap-1.5 mb-3">
              {provider.services.slice(0, 3).map((service: any) => (
                <span
                  key={service.id}
                  className="text-xs bg-gray-100 text-gray-700 px-2 py-1 rounded-md font-medium"
                >
                  {service.name}
                </span>
              ))}
              {provider.services.length > 3 && (
                <span className="text-xs text-gray-500 font-medium px-2 py-1">
                  +{provider.services.length - 3} more
                </span>
              )}
            </div>
          )}

          {/* Price */}
          <div className="pt-3 border-t border-gray-100 mb-3">
            {minPrice !== null && firstService ? (
              <div className="flex items-baseline gap-1">
                <span className="text-lg font-bold text-gray-900">
                  ${minPrice.toFixed(0)}
                </span>
                <span className="text-sm text-gray-600">and up</span>
                <span className="text-xs text-gray-500 ml-1">• {firstService?.name}</span>
              </div>
            ) : (
              <div className="text-sm text-gray-600 italic">Check availability below</div>
            )}
          </div>

          {/* Quick Booking Slots - Auto-loaded */}
          {firstService && (
            <div data-slot-container onClick={(e) => e.stopPropagation()}>
              <ProviderQuickSlots
                providerId={provider.id}
                serviceId={firstService.id}
                serviceName={firstService.name}
                autoLoad={true}
                preloadedSlots={preloadedSlots}
              />
            </div>
          )}
        </div>
      </Card>
  )
}
