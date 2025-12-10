'use client'

import { useEffect, useState } from 'react'
import { useRouter, useSearchParams } from 'next/navigation'
import { useAuth } from '@/contexts/AuthContext'
import { providersApi, appointmentsApi, employeesApi, type Employee } from 'shared'
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

export default function BookingPage() {
  const { user, loading: authLoading } = useAuth()
  const router = useRouter()
  const searchParams = useSearchParams()

  const providerId = searchParams.get('provider')
  const serviceId = searchParams.get('service')
  const prefilledDate = searchParams.get('date')
  const prefilledTime = searchParams.get('time')

  const [provider, setProvider] = useState<any>(null)
  const [service, setService] = useState<any>(null)
  const [employees, setEmployees] = useState<Employee[]>([])
  const [selectedEmployee, setSelectedEmployee] = useState<string>('')
  const [loading, setLoading] = useState(true)
  const [selectedDate, setSelectedDate] = useState('')
  const [timeSlots, setTimeSlots] = useState<any[]>([])
  const [selectedSlot, setSelectedSlot] = useState<any>(null)
  const [loadingSlots, setLoadingSlots] = useState(false)
  const [booking, setBooking] = useState(false)
  const [notes, setNotes] = useState('')
  const [error, setError] = useState('')

  useEffect(() => {
    if (!authLoading) {
      if (!user) {
        router.push(`/auth/login?redirect=/booking?provider=${providerId}&service=${serviceId}`)
        return
      }

      if (!providerId || !serviceId) {
        router.push('/search')
        return
      }

      loadData()
    }
  }, [user, authLoading, providerId, serviceId])

  const loadData = async () => {
    try {
      const providerData = await providersApi.getProvider(providerId!)
      setProvider(providerData)

      const serviceData = providerData.services?.find((s: any) => s.id === serviceId)
      if (!serviceData) {
        throw new Error('Service not found')
      }
      setService(serviceData)

      // Load employees for this provider
      try {
        const employeesData = await employeesApi.getProviderEmployees(providerId!)
        const activeEmployees = employeesData.filter((e: Employee) => e.is_active)
        setEmployees(activeEmployees)
      } catch (err) {
        // No employees or error loading - continue without employee selection
        console.log('No employees available for this provider')
      }

      // Set default date (use prefilled or tomorrow)
      if (prefilledDate) {
        setSelectedDate(prefilledDate)
      } else {
        const tomorrow = new Date()
        tomorrow.setDate(tomorrow.getDate() + 1)
        setSelectedDate(tomorrow.toISOString().split('T')[0])
      }
    } catch (err: any) {
      setError(err.message || 'Failed to load booking data')
    } finally {
      setLoading(false)
    }
  }

  const loadTimeSlots = async (date: string, employeeId?: string) => {
    setLoadingSlots(true)
    setSelectedSlot(null)
    try {
      const slots = await appointmentsApi.getAvailableTimeSlots(
        providerId!,
        serviceId!,
        date,
        employeeId || undefined
      )

      // Filter out past time slots if the selected date is today
      const today = new Date().toISOString().split('T')[0]
      const isToday = date === today

      let filteredSlots = slots
      if (isToday) {
        const now = new Date()
        const currentTimeMinutes = now.getHours() * 60 + now.getMinutes()

        // If there's a prefilled time, use that as the minimum time
        // Otherwise, use current time
        let minimumTimeMinutes = currentTimeMinutes
        if (prefilledTime) {
          const [hours, minutes] = prefilledTime.split(':')
          const prefilledTimeMinutes = parseInt(hours) * 60 + parseInt(minutes)
          // Use the later of current time or prefilled time
          minimumTimeMinutes = Math.max(currentTimeMinutes, prefilledTimeMinutes)
        }

        filteredSlots = slots.filter(slot => {
          const [hours, minutes] = slot.start_time.split(':')
          const slotTimeMinutes = parseInt(hours) * 60 + parseInt(minutes)
          return slotTimeMinutes >= minimumTimeMinutes
        })
      }

      setTimeSlots(filteredSlots)
    } catch (err: any) {
      setError(err.message || 'Failed to load time slots')
    } finally {
      setLoadingSlots(false)
    }
  }

  useEffect(() => {
    if (selectedDate && provider) {
      loadTimeSlots(selectedDate, selectedEmployee || undefined)
    }
  }, [selectedDate, selectedEmployee, provider])

  // Auto-select prefilled time slot
  useEffect(() => {
    if (prefilledTime && timeSlots.length > 0 && !selectedSlot) {
      const slot = timeSlots.find((s) => s.start_time === prefilledTime && s.available)
      if (slot) {
        setSelectedSlot(slot)
        // Scroll to show the selected slot is pre-filled
        setTimeout(() => {
          const element = document.querySelector('[data-slot-selected="true"]')
          if (element) {
            element.scrollIntoView({ behavior: 'smooth', block: 'center' })
          }
        }, 100)
      }
    }
  }, [prefilledTime, timeSlots])

  const handleBooking = async () => {
    if (!selectedSlot) return

    setBooking(true)
    setError('')

    try {
      const location = provider.provider_locations?.[0]
      if (!location) {
        throw new Error('Provider location not found')
      }

      const appointment = await appointmentsApi.createAppointment({
        provider_id: providerId!,
        location_id: location.id,
        service_id: serviceId!,
        staff_id: selectedEmployee || undefined,
        appointment_date: selectedDate,
        start_time: selectedSlot.start_time,
        end_time: selectedSlot.end_time,
        price: service.price,
        customer_notes: notes || undefined,
      })

      // Redirect to confirmation page
      router.push(`/booking/confirmation?id=${appointment.id}`)
    } catch (err: any) {
      setError(err.message || 'Failed to create booking')
    } finally {
      setBooking(false)
    }
  }

  const formatTime = (time: string) => {
    const [hours, minutes] = time.split(':')
    const hour = parseInt(hours)
    const ampm = hour >= 12 ? 'PM' : 'AM'
    const hour12 = hour % 12 || 12
    return `${hour12}:${minutes} ${ampm}`
  }

  if (authLoading || loading) {
    return (
      <div className="min-h-screen flex items-center justify-center">
        <div className="text-center">
          <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-primary mx-auto"></div>
          <p className="mt-4 text-muted-foreground">Loading...</p>
        </div>
      </div>
    )
  }

  if (error && !provider) {
    return (
      <div className="min-h-screen flex items-center justify-center p-4">
        <Card className="w-full max-w-md">
          <CardHeader>
            <CardTitle>Error</CardTitle>
            <CardDescription>{error}</CardDescription>
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

  return (
    <div className="min-h-screen bg-gray-50">
      {/* Header */}
      <header className="bg-white shadow-sm">
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-4">
          <div className="flex justify-between items-center">
            <h1 className="text-2xl font-bold text-primary">Book Appointment</h1>
            <Link href={`/providers/${providerId}`}>
              <Button variant="outline">← Back</Button>
            </Link>
          </div>
        </div>
      </header>

      <main className="max-w-4xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
        <div className="grid lg:grid-cols-3 gap-8">
          {/* Booking Form */}
          <div className="lg:col-span-2 space-y-6">
            {/* Service Info */}
            <Card>
              <CardHeader>
                <CardTitle>Service Details</CardTitle>
              </CardHeader>
              <CardContent className="space-y-2">
                <div className="flex justify-between items-start">
                  <div>
                    <h3 className="font-semibold text-lg">{service.name}</h3>
                    <p className="text-sm text-muted-foreground">
                      {provider.business_name}
                    </p>
                  </div>
                  <div className="text-right">
                    <p className="font-semibold text-lg">${service.price.toFixed(2)}</p>
                    <p className="text-sm text-muted-foreground">
                      {service.duration_minutes} min
                    </p>
                  </div>
                </div>
                {service.description && (
                  <p className="text-sm text-gray-600 pt-2">{service.description}</p>
                )}
              </CardContent>
            </Card>

            {/* Employee Selection */}
            {employees.length > 0 && (
              <Card>
                <CardHeader>
                  <CardTitle>Select Staff Member (Optional)</CardTitle>
                  <CardDescription>
                    Choose a specific staff member or leave blank for any available
                  </CardDescription>
                </CardHeader>
                <CardContent>
                  <div className="grid grid-cols-2 sm:grid-cols-3 gap-3">
                    {/* Any Staff Member Option */}
                    <button
                      onClick={() => setSelectedEmployee('')}
                      className={`p-4 rounded-lg border-2 transition-all ${
                        selectedEmployee === ''
                          ? 'border-blue-600 bg-blue-50'
                          : 'border-gray-200 hover:border-gray-300 hover:bg-gray-50'
                      }`}
                    >
                      <div className="flex flex-col items-center gap-2">
                        <div className="w-12 h-12 bg-gradient-to-br from-gray-400 to-gray-500 rounded-full flex items-center justify-center text-white font-bold text-lg">
                          ?
                        </div>
                        <div className="text-center">
                          <p className="font-semibold text-sm">Any Staff</p>
                          <p className="text-xs text-gray-600">First available</p>
                        </div>
                      </div>
                    </button>

                    {/* Individual Staff Members */}
                    {employees.map((employee) => (
                      <button
                        key={employee.id}
                        onClick={() => setSelectedEmployee(employee.id)}
                        className={`p-4 rounded-lg border-2 transition-all ${
                          selectedEmployee === employee.id
                            ? 'border-blue-600 bg-blue-50'
                            : 'border-gray-200 hover:border-gray-300 hover:bg-gray-50'
                        }`}
                      >
                        <div className="flex flex-col items-center gap-2">
                          <div className="w-12 h-12 bg-gradient-to-br from-blue-500 to-purple-600 rounded-full flex items-center justify-center text-white font-bold text-lg">
                            {employee.first_name[0]}{employee.last_name[0]}
                          </div>
                          <div className="text-center">
                            <p className="font-semibold text-sm">
                              {employee.first_name} {employee.last_name}
                            </p>
                            {employee.position && (
                              <p className="text-xs text-gray-600">{employee.position}</p>
                            )}
                          </div>
                        </div>
                      </button>
                    ))}
                  </div>
                </CardContent>
              </Card>
            )}

            {/* Date Selection */}
            <Card>
              <CardHeader>
                <CardTitle>Select Date</CardTitle>
              </CardHeader>
              <CardContent>
                <input
                  type="date"
                  value={selectedDate}
                  onChange={(e) => setSelectedDate(e.target.value)}
                  min={new Date().toISOString().split('T')[0]}
                  className="flex h-9 w-full rounded-md border border-input bg-transparent px-3 py-1 text-sm shadow-sm transition-colors focus-visible:outline-none focus-visible:ring-1 focus-visible:ring-ring"
                />
              </CardContent>
            </Card>

            {/* Time Slot Selection */}
            <Card>
              <CardHeader>
                <CardTitle>Select Time</CardTitle>
                <CardDescription>
                  {selectedDate && `Available slots for ${new Date(selectedDate + 'T00:00:00').toLocaleDateString('en-US', { weekday: 'long', month: 'long', day: 'numeric' })}`}
                </CardDescription>
              </CardHeader>
              <CardContent>
                {loadingSlots ? (
                  <div className="text-center py-8">
                    <div className="animate-spin rounded-full h-8 w-8 border-b-2 border-primary mx-auto"></div>
                    <p className="mt-2 text-sm text-muted-foreground">Loading available times...</p>
                  </div>
                ) : timeSlots.length === 0 ? (
                  <p className="text-center py-8 text-muted-foreground">
                    No available time slots for this date
                  </p>
                ) : (
                  <div className="grid grid-cols-3 sm:grid-cols-4 gap-2">
                    {timeSlots.filter(slot => slot.available).map((slot, index) => (
                      <button
                        key={index}
                        onClick={() => setSelectedSlot(slot)}
                        data-slot-selected={selectedSlot?.start_time === slot.start_time ? 'true' : 'false'}
                        className={`p-3 text-sm rounded-lg border-2 transition-all ${
                          selectedSlot?.start_time === slot.start_time
                            ? 'bg-gradient-to-r from-purple-600 to-pink-600 text-white border-purple-600 shadow-lg scale-110 font-bold'
                            : 'border-gray-200 hover:border-primary hover:bg-primary/5'
                        }`}
                      >
                        {formatTime(slot.start_time)}
                        {selectedSlot?.start_time === slot.start_time && prefilledTime && (
                          <div className="text-xs mt-1 text-purple-100">✓ Selected</div>
                        )}
                      </button>
                    ))}
                  </div>
                )}
              </CardContent>
            </Card>

            {/* Additional Notes */}
            <Card>
              <CardHeader>
                <CardTitle>Additional Notes (Optional)</CardTitle>
              </CardHeader>
              <CardContent>
                <textarea
                  value={notes}
                  onChange={(e) => setNotes(e.target.value)}
                  placeholder="Any special requests or information for the provider..."
                  className="flex min-h-[100px] w-full rounded-md border border-input bg-transparent px-3 py-2 text-sm shadow-sm placeholder:text-muted-foreground focus-visible:outline-none focus-visible:ring-1 focus-visible:ring-ring"
                  maxLength={500}
                />
                <p className="text-xs text-muted-foreground mt-1">
                  {notes.length}/500 characters
                </p>
              </CardContent>
            </Card>

            {error && (
              <div className="bg-destructive/10 text-destructive text-sm p-3 rounded-md">
                {error}
              </div>
            )}
          </div>

          {/* Booking Summary */}
          <aside className="lg:col-span-1">
            <Card className="sticky top-4">
              <CardHeader>
                <CardTitle>Booking Summary</CardTitle>
              </CardHeader>
              <CardContent className="space-y-4">
                <div className="space-y-2 text-sm">
                  {selectedEmployee && employees.length > 0 && (
                    <div className="flex justify-between pb-2 border-b">
                      <span className="text-muted-foreground">Staff:</span>
                      <span className="font-medium">
                        {employees.find(e => e.id === selectedEmployee)?.first_name}{' '}
                        {employees.find(e => e.id === selectedEmployee)?.last_name}
                      </span>
                    </div>
                  )}
                  <div className="flex justify-between">
                    <span className="text-muted-foreground">Date:</span>
                    <span className="font-medium">
                      {selectedDate
                        ? new Date(selectedDate + 'T00:00:00').toLocaleDateString('en-US', { month: 'short', day: 'numeric' })
                        : '—'}
                    </span>
                  </div>
                  <div className="flex justify-between">
                    <span className="text-muted-foreground">Time:</span>
                    <span className="font-medium">
                      {selectedSlot ? formatTime(selectedSlot.start_time) : '—'}
                    </span>
                  </div>
                  <div className="flex justify-between">
                    <span className="text-muted-foreground">Duration:</span>
                    <span className="font-medium">{service.duration_minutes} min</span>
                  </div>
                  <div className="border-t pt-2 mt-2">
                    <div className="flex justify-between font-semibold text-base">
                      <span>Total:</span>
                      <span>${service.price.toFixed(2)}</span>
                    </div>
                  </div>
                </div>

                <Button
                  className="w-full"
                  size="lg"
                  onClick={handleBooking}
                  disabled={!selectedSlot || booking}
                >
                  {booking ? 'Booking...' : 'Confirm Booking'}
                </Button>

                <p className="text-xs text-muted-foreground text-center">
                  You'll receive a confirmation email after booking
                </p>
              </CardContent>
            </Card>
          </aside>
        </div>
      </main>
    </div>
  )
}
