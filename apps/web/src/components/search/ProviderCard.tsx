import Link from 'next/link'
import { Card, CardContent, CardHeader } from '@/components/ui/card'
import { Button } from '@/components/ui/button'

interface ProviderCardProps {
  provider: any
}

export function ProviderCard({ provider }: ProviderCardProps) {
  const primaryLocation = provider.provider_locations?.find((loc: any) => loc.is_primary) ||
    provider.provider_locations?.[0]

  return (
    <Card className="hover:shadow-lg transition-shadow">
      <CardHeader className="pb-3">
        <div className="flex justify-between items-start">
          <div className="flex-1">
            <h3 className="font-semibold text-lg">{provider.business_name}</h3>
            {primaryLocation && (
              <p className="text-sm text-muted-foreground mt-1">
                {primaryLocation.city}, {primaryLocation.state_province}
              </p>
            )}
          </div>
          {provider.verified && (
            <span className="bg-primary/10 text-primary text-xs px-2 py-1 rounded-full">
              Verified
            </span>
          )}
        </div>
      </CardHeader>

      <CardContent className="space-y-3">
        {provider.description && (
          <p className="text-sm text-gray-600 line-clamp-2">
            {provider.description}
          </p>
        )}

        {/* Rating */}
        <div className="flex items-center gap-2">
          <div className="flex items-center">
            {[...Array(5)].map((_, i) => (
              <span
                key={i}
                className={`text-lg ${
                  i < Math.floor(provider.rating_average || 0)
                    ? 'text-yellow-400'
                    : 'text-gray-300'
                }`}
              >
                ★
              </span>
            ))}
          </div>
          <span className="text-sm text-muted-foreground">
            {provider.rating_average ? provider.rating_average.toFixed(1) : 'No reviews'}
            {provider.rating_count > 0 && (
              <span> ({provider.rating_count})</span>
            )}
          </span>
        </div>

        {/* Services Preview */}
        {provider.services && provider.services.length > 0 && (
          <div className="flex flex-wrap gap-2">
            {provider.services.slice(0, 3).map((service: any) => (
              <span
                key={service.id}
                className="text-xs bg-gray-100 px-2 py-1 rounded"
              >
                {service.name}
              </span>
            ))}
            {provider.services.length > 3 && (
              <span className="text-xs text-muted-foreground">
                +{provider.services.length - 3} more
              </span>
            )}
          </div>
        )}

        {/* Price Range */}
        {provider.services && provider.services.length > 0 && (
          <div className="text-sm text-muted-foreground">
            From ${Math.min(...provider.services.map((s: any) => s.price)).toFixed(0)}
          </div>
        )}

        {/* Action */}
        <Link href={`/providers/${provider.id}`}>
          <Button className="w-full" variant="outline">
            View Details
          </Button>
        </Link>
      </CardContent>
    </Card>
  )
}
