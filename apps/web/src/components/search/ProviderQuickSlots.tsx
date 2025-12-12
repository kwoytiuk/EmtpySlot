'use client'

import { useEffect, useState } from 'react'
import { useRouter } from 'next/navigation'
import { appointmentsApi, type TimeSlot } from 'shared'
import { Button } from '@/components/ui/button'

interface ProviderQuickSlotsProps {
  providerId: string
  serviceId: string
  serviceName: string
  autoLoad?: boolean
  preloadedSlots?: TimeSlot[]
}

export function ProviderQuickSlots({
  providerId,
  serviceId,
  serviceName,
  autoLoad = false,
  preloadedSlots,
}: ProviderQuickSlotsProps) {
  const router = useRouter()
  const [slots, setSlots] = useState<TimeSlot[]>([])
  const [loading, setLoading] = useState(false)
  const [showSlots, setShowSlots] = useState(autoLoad)
  const [error, setError] = useState('')

  // If preloaded slots are provided, use them immediately
  useEffect(() => {
    if (preloadedSlots && preloadedSlots.length > 0) {
      setSlots(preloadedSlots.slice(0, 6))
      setShowSlots(true)
    }
  }, [preloadedSlots])

  useEffect(() => {
    // Only load slots if not preloaded and showSlots is true
    if (showSlots && slots.length === 0 && !preloadedSlots) {
      loadSlots()
    }
  }, [showSlots, preloadedSlots])

  const loadSlots = async () => {
    setLoading(true)
    setError('')
    try {
      const data = await appointmentsApi.getQuickBookingSlots(providerId, serviceId)
      // Only show first 6 slots to keep it compact
      setSlots(data.slice(0, 6))
    } catch (err: any) {
      setError(err.message || 'Failed to load slots')
      setSlots([])
    } finally {
      setLoading(false)
    }
  }

  const handleSlotClick = (slot: TimeSlot) => {
    if (!slot.available) return

    const today = new Date().toISOString().split('T')[0]
    router.push(
      `/booking?provider=${providerId}&service=${serviceId}&date=${today}&time=${slot.start_time}`
    )
  }

  const formatTime = (time: string) => {
    const [hours, minutes] = time.split(':')
    const hour = parseInt(hours)
    const ampm = hour >= 12 ? 'PM' : 'AM'
    const hour12 = hour % 12 || 12
    return `${hour12}:${minutes} ${ampm}`
  }

  const availableSlots = slots.filter((s) => s.available)
  const hasAvailableSlots = availableSlots.length > 0

  // If autoLoad is false, show button first
  if (!showSlots && !autoLoad) {
    return (
      <Button
        variant="outline"
        size="sm"
        onClick={() => setShowSlots(true)}
        className="w-full text-purple-600 border-purple-300 hover:bg-purple-50 hover:border-purple-400 font-semibold"
      >
        ⚡ Show Available Times
      </Button>
    )
  }

  if (loading) {
    return (
      <div className="border-t border-gray-200 pt-3 mt-3">
        <div className="flex items-center justify-center py-4">
          <div className="animate-spin rounded-full h-5 w-5 border-2 border-purple-600 border-t-transparent mr-2"></div>
          <span className="text-sm text-gray-600">Checking availability...</span>
        </div>
      </div>
    )
  }

  if (error) {
    return (
      <div className="border-t border-gray-200 pt-3 mt-3">
        <div className="text-center py-2">
          <p className="text-xs text-red-600">{error}</p>
        </div>
      </div>
    )
  }

  if (slots.length === 0) {
    return (
      <div className="border-t border-purple-100 pt-3 mt-3 bg-gray-50 -mx-5 px-5 pb-3 rounded-b-xl">
        <div className="text-center py-3">
          <p className="text-sm font-semibold text-gray-700 mb-2">⌚ No immediate availability</p>
          <Button
            variant="outline"
            size="sm"
            onClick={() => router.push(`/providers/${providerId}`)}
            className="text-xs bg-white hover:bg-gray-50"
          >
            See Future Times →
          </Button>
        </div>
      </div>
    )
  }

  return (
    <div className="border-t border-purple-100 pt-3 mt-3 bg-gradient-to-br from-purple-50/50 to-pink-50/50 -mx-5 px-5 pb-3 rounded-b-xl">
      {/* Header */}
      <div className="flex items-center justify-between mb-2">
        <div className="flex items-center gap-2">
          <span className="text-xs font-bold text-purple-700">⚡ Available Now</span>
          {hasAvailableSlots && (
            <span className="text-xs bg-green-100 text-green-700 px-2 py-0.5 rounded-full font-semibold">
              {availableSlots.length} {availableSlots.length === 1 ? 'slot' : 'slots'}
            </span>
          )}
        </div>
        {!autoLoad && (
          <button
            onClick={() => setShowSlots(false)}
            className="text-xs text-gray-500 hover:text-gray-700"
          >
            Hide
          </button>
        )}
      </div>

      {/* Slots */}
      <div className="grid grid-cols-3 gap-2 mb-2">
        {slots.map((slot, index) => (
          <button
            key={index}
            onClick={() => handleSlotClick(slot)}
            disabled={!slot.available}
            className={`
              px-2 py-2 rounded-lg text-xs font-bold transition-all border
              ${
                slot.available
                  ? 'bg-green-50 border-green-400 text-green-900 hover:bg-green-100 hover:shadow-sm cursor-pointer'
                  : 'bg-gray-100 border-gray-300 text-gray-400 cursor-not-allowed relative'
              }
            `}
            title={slot.available ? `Book for ${formatTime(slot.start_time)}` : 'Taken'}
          >
            {formatTime(slot.start_time)}
            {!slot.available && (
              <div className="absolute inset-0 flex items-center justify-center">
                <div className="w-full h-0.5 bg-gray-400 transform rotate-[-20deg]"></div>
              </div>
            )}
          </button>
        ))}
      </div>

      {/* CTA */}
      {hasAvailableSlots ? (
        <div className="text-center">
          <p className="text-xs text-purple-600 font-medium">
            Click any green slot to book instantly
          </p>
        </div>
      ) : (
        <div className="text-center">
          <Button
            variant="outline"
            size="sm"
            onClick={() => router.push(`/providers/${providerId}`)}
            className="text-xs"
          >
            See More Times →
          </Button>
        </div>
      )}
    </div>
  )
}
