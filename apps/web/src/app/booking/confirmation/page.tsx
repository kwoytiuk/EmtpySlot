'use client'

import { useEffect, useState } from 'react'
import { useSearchParams } from 'next/navigation'
import { appointmentsApi } from 'shared'
import { Button } from '@/components/ui/button'
import {
  Card,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle,
} from '@/components/ui/card'
import Link from 'next/link'

export default function BookingConfirmationPage() {
  const searchParams = useSearchParams()
  const appointmentId = searchParams.get('id')

  const [appointment, setAppointment] = useState<any>(null)
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState('')

  useEffect(() => {
    if (appointmentId) {
      loadAppointment()
    }
  }, [appointmentId])

  const loadAppointment = async () => {
    try {
      const data = await appointmentsApi.getAppointment(appointmentId!)
      setAppointment(data)
    } catch (err: any) {
      setError(err.message || 'Failed to load appointment')
    } finally {
      setLoading(false)
    }
  }

  const formatDate = (date: string) => {
    return new Date(date + 'T00:00:00').toLocaleDateString('en-US', {
      weekday: 'long',
      month: 'long',
      day: 'numeric',
      year: 'numeric',
    })
  }

  const formatTime = (time: string) => {
    const [hours, minutes] = time.split(':')
    const hour = parseInt(hours)
    const ampm = hour >= 12 ? 'PM' : 'AM'
    const hour12 = hour % 12 || 12
    return `${hour12}:${minutes} ${ampm}`
  }

  if (loading) {
    return (
      <div className="min-h-screen flex items-center justify-center">
        <div className="text-center">
          <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-primary mx-auto"></div>
          <p className="mt-4 text-muted-foreground">Loading confirmation...</p>
        </div>
      </div>
    )
  }

  if (error || !appointment) {
    return (
      <div className="min-h-screen flex items-center justify-center p-4">
        <Card className="w-full max-w-md">
          <CardHeader>
            <CardTitle>Booking Not Found</CardTitle>
            <CardDescription>{error || 'This appointment could not be found'}</CardDescription>
          </CardHeader>
          <CardContent>
            <Link href="/">
              <Button className="w-full">Back to Home</Button>
            </Link>
          </CardContent>
        </Card>
      </div>
    )
  }

  return (
    <div className="min-h-screen bg-gradient-to-br from-green-50 to-emerald-100">
      <div className="max-w-3xl mx-auto px-4 sm:px-6 lg:px-8 py-12">
        {/* Success Message */}
        <div className="text-center mb-8">
          <div className="inline-flex items-center justify-center w-16 h-16 bg-green-500 text-white rounded-full mb-4">
            <svg
              className="w-8 h-8"
              fill="none"
              stroke="currentColor"
              viewBox="0 0 24 24"
            >
              <path
                strokeLinecap="round"
                strokeLinejoin="round"
                strokeWidth={2}
                d="M5 13l4 4L19 7"
              />
            </svg>
          </div>
          <h1 className="text-3xl font-bold text-gray-900 mb-2">
            Booking Confirmed!
          </h1>
          <p className="text-lg text-gray-600">
            Your appointment has been successfully booked
          </p>
        </div>

        {/* Appointment Details */}
        <Card className="mb-6">
          <CardHeader>
            <CardTitle>Appointment Details</CardTitle>
            <CardDescription>Booking Reference: {appointment.booking_reference}</CardDescription>
          </CardHeader>
          <CardContent className="space-y-4">
            {/* Service */}
            <div className="border-b pb-4">
              <h3 className="font-semibold mb-2">Service</h3>
              <p className="text-lg">{appointment.services.name}</p>
              {appointment.services.description && (
                <p className="text-sm text-muted-foreground">
                  {appointment.services.description}
                </p>
              )}
            </div>

            {/* Provider */}
            <div className="border-b pb-4">
              <h3 className="font-semibold mb-2">Provider</h3>
              <p className="text-lg">{appointment.providers.business_name}</p>
              <div className="mt-2 space-y-1 text-sm">
                {appointment.providers.phone && (
                  <p className="flex items-center gap-2">
                    <span className="text-muted-foreground">Phone:</span>
                    <span>{appointment.providers.phone}</span>
                  </p>
                )}
                {appointment.providers.email && (
                  <p className="flex items-center gap-2">
                    <span className="text-muted-foreground">Email:</span>
                    <span>{appointment.providers.email}</span>
                  </p>
                )}
              </div>
            </div>

            {/* Date & Time */}
            <div className="border-b pb-4">
              <h3 className="font-semibold mb-2">When</h3>
              <p className="text-lg">{formatDate(appointment.appointment_date)}</p>
              <p className="text-muted-foreground">
                {formatTime(appointment.start_time)} - {formatTime(appointment.end_time)}
              </p>
              <p className="text-sm text-muted-foreground mt-1">
                Duration: {appointment.services.duration_minutes} minutes
              </p>
            </div>

            {/* Location */}
            <div className="border-b pb-4">
              <h3 className="font-semibold mb-2">Where</h3>
              <div className="text-sm">
                <p>{appointment.provider_locations.address_line1}</p>
                {appointment.provider_locations.address_line2 && (
                  <p>{appointment.provider_locations.address_line2}</p>
                )}
                <p>
                  {appointment.provider_locations.city},{' '}
                  {appointment.provider_locations.state_province}{' '}
                  {appointment.provider_locations.postal_code}
                </p>
              </div>
            </div>

            {/* Price */}
            <div>
              <h3 className="font-semibold mb-2">Price</h3>
              <p className="text-2xl font-bold text-primary">
                ${appointment.price.toFixed(2)}
              </p>
              <p className="text-sm text-muted-foreground">
                Payment due at appointment
              </p>
            </div>

            {/* Customer Notes */}
            {appointment.customer_notes && (
              <div className="bg-gray-50 p-4 rounded-lg">
                <h3 className="font-semibold mb-2">Your Notes</h3>
                <p className="text-sm text-gray-600">{appointment.customer_notes}</p>
              </div>
            )}
          </CardContent>
        </Card>

        {/* Actions */}
        <div className="flex flex-col sm:flex-row gap-4">
          <Link href="/" className="flex-1">
            <Button variant="outline" className="w-full" size="lg">
              Back to Home
            </Button>
          </Link>
          <Link href="/appointments" className="flex-1">
            <Button className="w-full" size="lg">
              View My Appointments
            </Button>
          </Link>
        </div>

        {/* Next Steps */}
        <Card className="mt-6 bg-blue-50 border-blue-200">
          <CardHeader>
            <CardTitle className="text-blue-900">What's Next?</CardTitle>
          </CardHeader>
          <CardContent className="text-sm text-blue-900 space-y-2">
            <p>✓ Confirmation email sent to your inbox</p>
            <p>✓ You'll receive a reminder 24 hours before your appointment</p>
            <p>✓ The provider has been notified of your booking</p>
            <p className="pt-2 font-medium">
              Need to cancel or reschedule? Visit "My Appointments" to manage your booking.
            </p>
          </CardContent>
        </Card>
      </div>
    </div>
  )
}
