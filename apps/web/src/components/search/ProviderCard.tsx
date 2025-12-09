'use client'

import { useRouter } from 'next/navigation'
import { Card } from '@/components/ui/card'
import { Button } from '@/components/ui/button'
import { ProviderQuickSlots } from './ProviderQuickSlots'

interface ProviderCardProps {
  provider: any
}

export function ProviderCard({ provider }: ProviderCardProps) {
  const router = useRouter()

  const primaryLocation = provider.provider_locations?.find((loc: any) => loc.is_primary) ||
    provider.provider_locations?.[0]

  // Calculate price range
  const minPrice = provider.services && provider.services.length > 0
    ? Math.min(...provider.services.map((s: any) => s.price))
    : null

  // Get category from first service
  const category = provider.services?.[0]?.categories?.name || 'Service'

  // Get first service for quick booking
  const firstService = provider.services?.[0]

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
        {/* Image Placeholder with Gradient */}
        <div className="relative h-48 bg-gradient-to-br from-blue-500 via-purple-500 to-pink-500 overflow-hidden">
          <div className="absolute inset-0 bg-black/20 group-hover:bg-black/10 transition-colors" />

          {provider.verified && (
            <div className="absolute top-3 right-3 bg-white text-blue-600 text-xs font-semibold px-3 py-1.5 rounded-full shadow-lg flex items-center gap-1">
              ✓ Verified
            </div>
          )}

          {/* Category badge */}
          <div className="absolute bottom-3 left-3 bg-white/95 backdrop-blur-sm text-gray-800 text-xs font-medium px-3 py-1.5 rounded-full">
            {category}
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
              <p className="text-sm text-gray-600 mt-0.5">
                📍 {primaryLocation.city}, {primaryLocation.state_province}
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
          <div className="pt-3 border-t border-gray-100">
            {minPrice !== null ? (
              <div className="flex items-baseline gap-1">
                <span className="text-lg font-bold text-gray-900">
                  ${minPrice.toFixed(0)}
                </span>
                <span className="text-sm text-gray-600">and up</span>
                <span className="text-xs text-gray-500 ml-1">• {firstService?.name}</span>
              </div>
            ) : (
              <div className="text-sm text-gray-600">Contact for pricing</div>
            )}
          </div>

          {/* Quick Booking Slots */}
          {firstService && (
            <div data-slot-container onClick={(e) => e.stopPropagation()}>
              <ProviderQuickSlots
                providerId={provider.id}
                serviceId={firstService.id}
                serviceName={firstService.name}
                autoLoad={false}
              />
            </div>
          )}
        </div>
      </Card>
  )
}
