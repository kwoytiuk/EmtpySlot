'use client'

import { useEffect, useState } from 'react'
import { useAuth } from '@/contexts/AuthContext'
import { useRouter } from 'next/navigation'
import { providersApi, type Provider } from 'shared'
import { Button } from '@/components/ui/button'
import {
  Card,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle,
} from '@/components/ui/card'
import Link from 'next/link'

export default function ProviderDashboard() {
  const { user, profile, loading: authLoading } = useAuth()
  const router = useRouter()
  const [provider, setProvider] = useState<any>(null)
  const [loading, setLoading] = useState(true)

  useEffect(() => {
    if (!authLoading) {
      if (!user) {
        router.push('/auth/login')
        return
      }

      if (profile?.user_type !== 'provider') {
        router.push('/')
        return
      }

      loadProviderData()
    }
  }, [user, profile, authLoading])

  const loadProviderData = async () => {
    try {
      const data = await providersApi.getMyProvider()
      if (!data) {
        // No provider profile yet, redirect to onboarding
        router.push('/provider/onboarding')
        return
      }
      setProvider(data)
    } catch (error) {
      console.error('Error loading provider:', error)
    } finally {
      setLoading(false)
    }
  }

  if (authLoading || loading) {
    return (
      <div className="min-h-screen flex items-center justify-center">
        <div className="text-center">
          <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-primary mx-auto"></div>
          <p className="mt-4 text-muted-foreground">Loading dashboard...</p>
        </div>
      </div>
    )
  }

  if (!provider) {
    return null // Will redirect to onboarding
  }

  const hasLocation = provider.provider_locations && provider.provider_locations.length > 0
  const hasServices = provider.services && provider.services.length > 0

  return (
    <div className="min-h-screen bg-gray-50">
      {/* Header */}
      <header className="bg-white shadow-sm">
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-4">
          <div className="flex justify-between items-center">
            <div>
              <h1 className="text-2xl font-bold text-gray-900">
                {provider.business_name}
              </h1>
              <p className="text-sm text-gray-500">Provider Dashboard</p>
            </div>
            <Link href="/">
              <Button variant="outline">Back to Home</Button>
            </Link>
          </div>
        </div>
      </header>

      {/* Main Content */}
      <main className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
        {/* Setup Alert */}
        {(!hasLocation || !hasServices) && (
          <div className="bg-yellow-50 border border-yellow-200 rounded-lg p-4 mb-6">
            <h3 className="text-sm font-medium text-yellow-800 mb-2">
              Complete your profile setup
            </h3>
            <ul className="text-sm text-yellow-700 space-y-1 mb-3">
              {!hasLocation && <li>• Add your business location</li>}
              {!hasServices && <li>• Create your first service</li>}
            </ul>
            <Link href="/provider/onboarding">
              <Button size="sm">Complete Setup</Button>
            </Link>
          </div>
        )}

        {/* Stats Grid */}
        <div className="grid md:grid-cols-4 gap-6 mb-8">
          <Card>
            <CardHeader className="pb-2">
              <CardDescription>Total Appointments</CardDescription>
              <CardTitle className="text-3xl">0</CardTitle>
            </CardHeader>
            <CardContent>
              <p className="text-xs text-muted-foreground">
                +0% from last month
              </p>
            </CardContent>
          </Card>

          <Card>
            <CardHeader className="pb-2">
              <CardDescription>Active Services</CardDescription>
              <CardTitle className="text-3xl">
                {provider.services?.length || 0}
              </CardTitle>
            </CardHeader>
            <CardContent>
              <p className="text-xs text-muted-foreground">
                Services offered
              </p>
            </CardContent>
          </Card>

          <Card>
            <CardHeader className="pb-2">
              <CardDescription>Average Rating</CardDescription>
              <CardTitle className="text-3xl">
                {provider.rating_average ? provider.rating_average.toFixed(1) : 'N/A'}
              </CardTitle>
            </CardHeader>
            <CardContent>
              <p className="text-xs text-muted-foreground">
                {provider.rating_count || 0} reviews
              </p>
            </CardContent>
          </Card>

          <Card>
            <CardHeader className="pb-2">
              <CardDescription>This Month</CardDescription>
              <CardTitle className="text-3xl">$0</CardTitle>
            </CardHeader>
            <CardContent>
              <p className="text-xs text-muted-foreground">
                Revenue
              </p>
            </CardContent>
          </Card>
        </div>

        {/* Quick Actions */}
        <div className="grid md:grid-cols-2 lg:grid-cols-3 gap-6">
          <Card className="hover:shadow-lg transition-shadow cursor-pointer">
            <Link href="/provider/services">
              <CardHeader>
                <CardTitle>Manage Services</CardTitle>
                <CardDescription>
                  Add, edit, or remove your service offerings
                </CardDescription>
              </CardHeader>
            </Link>
          </Card>

          <Card className="hover:shadow-lg transition-shadow cursor-pointer border-blue-200 bg-blue-50/50">
            <Link href="/provider/employees">
              <CardHeader>
                <CardTitle className="flex items-center gap-2">
                  👥 Manage Employees
                  <span className="text-xs font-normal bg-blue-600 text-white px-2 py-0.5 rounded-full">New</span>
                </CardTitle>
                <CardDescription>
                  Add staff members and manage their schedules
                </CardDescription>
              </CardHeader>
            </Link>
          </Card>

          <Card className="hover:shadow-lg transition-shadow cursor-pointer">
            <Link href="/provider/settings">
              <CardHeader>
                <CardTitle>Business Settings</CardTitle>
                <CardDescription>
                  Update your profile, location, and business hours
                </CardDescription>
              </CardHeader>
            </Link>
          </Card>

          <Card className="hover:shadow-lg transition-shadow cursor-pointer">
            <CardHeader>
              <CardTitle>Calendar</CardTitle>
              <CardDescription>
                Manage your availability and appointments
              </CardDescription>
            </CardHeader>
          </Card>

          <Card className="hover:shadow-lg transition-shadow cursor-pointer">
            <CardHeader>
              <CardTitle>Promotions</CardTitle>
              <CardDescription>
                Create last-minute deals and special offers
              </CardDescription>
            </CardHeader>
          </Card>

          <Card className="hover:shadow-lg transition-shadow cursor-pointer">
            <CardHeader>
              <CardTitle>Reviews</CardTitle>
              <CardDescription>
                View and respond to customer reviews
              </CardDescription>
            </CardHeader>
          </Card>

          <Card className="hover:shadow-lg transition-shadow cursor-pointer">
            <CardHeader>
              <CardTitle>Analytics</CardTitle>
              <CardDescription>
                Track your performance and insights
              </CardDescription>
            </CardHeader>
          </Card>
        </div>

        {/* Recent Activity */}
        <div className="mt-8">
          <Card>
            <CardHeader>
              <CardTitle>Recent Activity</CardTitle>
              <CardDescription>
                Your latest appointments and updates
              </CardDescription>
            </CardHeader>
            <CardContent>
              <div className="text-center py-8 text-muted-foreground">
                No recent activity
              </div>
            </CardContent>
          </Card>
        </div>
      </main>
    </div>
  )
}
