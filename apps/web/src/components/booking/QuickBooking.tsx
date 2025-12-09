'use client'

import { useEffect, useState } from 'react'
import { useRouter } from 'next/navigation'
import { appointmentsApi, type TimeSlot } from 'shared'
import { Button } from '@/components/ui/button'
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card'

interface QuickBookingProps {
  providerId: string
  services: Array<{
    id: string
    name: string
    price: number
    duration_minutes: number
  }>
}

export function QuickBooking({ providerId, services }: QuickBookingProps) {
  const router = useRouter()
  const [selectedService, setSelectedService] = useState<string>(services[0]?.id || '')
  const [timeSlots, setTimeSlots] = useState<TimeSlot[]>([])
  const [loading, setLoading] = useState(false)
  const [error, setError] = useState('')

  useEffect(() => {
    if (selectedService) {
      loadQuickSlots()
    }
  }, [selectedService])

  const loadQuickSlots = async () => {
    setLoading(true)
    setError('')
    try {
      const slots = await appointmentsApi.getQuickBookingSlots(providerId, selectedService)
      setTimeSlots(slots)
    } catch (err: any) {
      setError(err.message || 'Failed to load available slots')
      setTimeSlots([])
    } finally {
      setLoading(false)
    }
  }

  const handleSlotClick = (slot: TimeSlot) => {
    if (!slot.available) return

    // Redirect to booking page with pre-filled time
    const today = new Date().toISOString().split('T')[0]
    router.push(
      `/booking?provider=${providerId}&service=${selectedService}&date=${today}&time=${slot.start_time}`
    )
  }

  const formatTime = (time: string) => {
    const [hours, minutes] = time.split(':')
    const hour = parseInt(hours)
    const ampm = hour >= 12 ? 'PM' : 'AM'
    const hour12 = hour % 12 || 12
    return `${hour12}:${minutes} ${ampm}`
  }

  const getCurrentTime = () => {
    const now = new Date()
    return now.toLocaleTimeString('en-US', {
      hour: 'numeric',
      minute: '2-digit',
      hour12: true,
    })
  }

  const availableCount = timeSlots.filter((s) => s.available).length
  const takenCount = timeSlots.filter((s) => !s.available).length

  return (
    <Card className="border-2 border-purple-200 bg-gradient-to-br from-purple-50 to-pink-50">
      <CardHeader className="pb-4">
        <div className="flex items-center justify-between">
          <div>
            <CardTitle className="text-2xl font-black">⚡ Book Right Now</CardTitle>
            <CardDescription className="text-base mt-1">
              Available slots in the next 3 hours • Updated {getCurrentTime()}
            </CardDescription>
          </div>
          <Button
            variant="ghost"
            size="sm"
            onClick={loadQuickSlots}
            disabled={loading}
            className="text-purple-600 hover:text-purple-700 hover:bg-purple-100"
          >
            {loading ? '↻ Refreshing...' : '↻ Refresh'}
          </Button>
        </div>
      </CardHeader>

      <CardContent className="space-y-4">
        {/* Service Selector */}
        {services.length > 1 && (
          <div>
            <label className="text-sm font-semibold text-gray-700 mb-2 block">
              Select Service:
            </label>
            <div className="grid grid-cols-1 gap-2">
              {services.map((service) => (
                <button
                  key={service.id}
                  onClick={() => setSelectedService(service.id)}
                  className={`p-3 rounded-xl border-2 transition-all text-left ${
                    selectedService === service.id
                      ? 'border-purple-600 bg-purple-100'
                      : 'border-gray-200 hover:border-purple-300 hover:bg-white'
                  }`}
                >
                  <div className="flex justify-between items-center">
                    <div>
                      <div className="font-bold text-gray-900">{service.name}</div>
                      <div className="text-sm text-gray-600">{service.duration_minutes} min</div>
                    </div>
                    <div className="font-black text-lg text-gray-900">
                      ${service.price.toFixed(0)}
                    </div>
                  </div>
                </button>
              ))}
            </div>
          </div>
        )}

        {/* Stats */}
        {timeSlots.length > 0 && (
          <div className="flex items-center gap-6 text-sm">
            <div className="flex items-center gap-2">
              <div className="w-3 h-3 rounded-full bg-green-500"></div>
              <span className="text-gray-700">
                <span className="font-bold text-green-700">{availableCount}</span> Available
              </span>
            </div>
            <div className="flex items-center gap-2">
              <div className="w-3 h-3 rounded-full bg-gray-300"></div>
              <span className="text-gray-700">
                <span className="font-bold text-gray-600">{takenCount}</span> Taken
              </span>
            </div>
          </div>
        )}

        {/* Time Slots */}
        {loading ? (
          <div className="text-center py-8">
            <div className="animate-spin rounded-full h-10 w-10 border-4 border-purple-600 border-t-transparent mx-auto"></div>
            <p className="mt-3 text-sm text-gray-600 font-medium">
              Checking availability...
            </p>
          </div>
        ) : error ? (
          <div className="text-center py-8">
            <div className="text-4xl mb-2">😔</div>
            <p className="text-red-600 font-medium">{error}</p>
          </div>
        ) : timeSlots.length === 0 ? (
          <div className="text-center py-8">
            <div className="text-5xl mb-3">🌙</div>
            <p className="text-gray-900 font-bold text-lg mb-1">No slots available right now</p>
            <p className="text-gray-600 text-sm">
              Try booking for a future date instead
            </p>
            <Button
              onClick={() => router.push(`/booking?provider=${providerId}&service=${selectedService}`)}
              className="mt-4 bg-purple-600 hover:bg-purple-700"
            >
              Book for Later →
            </Button>
          </div>
        ) : (
          <>
            <div className="grid grid-cols-3 sm:grid-cols-4 gap-2">
              {timeSlots.map((slot, index) => (
                <button
                  key={index}
                  onClick={() => handleSlotClick(slot)}
                  disabled={!slot.available}
                  className={`
                    p-3 rounded-xl font-bold text-sm transition-all border-2
                    ${
                      slot.available
                        ? 'bg-green-50 border-green-500 text-green-900 hover:bg-green-100 hover:shadow-md hover:scale-105 cursor-pointer'
                        : 'bg-gray-100 border-gray-300 text-gray-400 cursor-not-allowed relative'
                    }
                  `}
                  title={slot.available ? `Book for ${formatTime(slot.start_time)}` : 'Already booked'}
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

            {availableCount > 0 && (
              <div className="bg-gradient-to-r from-purple-600 to-pink-600 rounded-xl p-4 text-center text-white">
                <div className="font-black text-lg mb-1">
                  🔥 {availableCount} {availableCount === 1 ? 'Slot' : 'Slots'} Available Now!
                </div>
                <div className="text-sm text-purple-100">
                  Click any green slot to book instantly
                </div>
              </div>
            )}
          </>
        )}
      </CardContent>
    </Card>
  )
}
