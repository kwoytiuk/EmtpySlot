'use client'

import { useAuth } from '@/contexts/AuthContext'
import { Button } from '@/components/ui/button'
import Link from 'next/link'
import { useRouter } from 'next/navigation'

export default function Home() {
  const { user, profile, loading, signOut } = useAuth()
  const router = useRouter()

  const handleSignOut = async () => {
    await signOut()
    router.refresh()
  }

  if (loading) {
    return (
      <div className="min-h-screen flex items-center justify-center">
        <div className="text-center">
          <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-primary mx-auto"></div>
          <p className="mt-4 text-muted-foreground">Loading...</p>
        </div>
      </div>
    )
  }

  return (
    <div className="min-h-screen bg-gradient-to-br from-blue-50 to-indigo-100">
      {/* Navigation */}
      <nav className="bg-white shadow-sm">
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
          <div className="flex justify-between items-center h-16">
            <div className="flex items-center">
              <h1 className="text-2xl font-bold text-primary">EmptySlot</h1>
            </div>
            <div className="flex items-center gap-4">
              {user ? (
                <>
                  <span className="text-sm text-muted-foreground">
                    Welcome, {profile?.full_name || 'User'}
                  </span>
                  {profile?.user_type === 'provider' && (
                    <Link href="/provider/dashboard">
                      <Button variant="outline">Dashboard</Button>
                    </Link>
                  )}
                  <Button variant="ghost" onClick={handleSignOut}>
                    Sign Out
                  </Button>
                </>
              ) : (
                <>
                  <Link href="/auth/login">
                    <Button variant="ghost">Sign In</Button>
                  </Link>
                  <Link href="/auth/signup">
                    <Button>Get Started</Button>
                  </Link>
                </>
              )}
            </div>
          </div>
        </div>
      </nav>

      {/* Hero Section */}
      <main className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-20">
        <div className="text-center">
          <h1 className="text-5xl md:text-6xl font-bold text-gray-900 mb-6">
            Find Last-Minute
            <span className="text-primary"> Appointments</span>
          </h1>
          <p className="text-xl text-gray-600 mb-8 max-w-2xl mx-auto">
            Book appointments for hair, nails, dental, plumbing, and more.
            Discover last-minute availability near you and save time.
          </p>
          <div className="flex flex-col sm:flex-row gap-4 justify-center">
            {user ? (
              <Link href="/search">
                <Button size="lg" className="text-lg px-8 py-6">
                  Find Services
                </Button>
              </Link>
            ) : (
              <>
                <Link href="/search">
                  <Button size="lg" className="text-lg px-8 py-6">
                    Find Services
                  </Button>
                </Link>
                <Link href="/auth/signup">
                  <Button size="lg" variant="outline" className="text-lg px-8 py-6">
                    I'm a Provider
                  </Button>
                </Link>
              </>
            )}
          </div>
        </div>

        {/* Features Section */}
        <div className="mt-24 grid md:grid-cols-3 gap-8">
          <div className="bg-white p-8 rounded-xl shadow-md">
            <div className="text-4xl mb-4">📍</div>
            <h3 className="text-xl font-semibold mb-2">Location-Based Search</h3>
            <p className="text-gray-600">
              Find services near you with our powerful geospatial search
            </p>
          </div>
          <div className="bg-white p-8 rounded-xl shadow-md">
            <div className="text-4xl mb-4">⚡</div>
            <h3 className="text-xl font-semibold mb-2">Instant Booking</h3>
            <p className="text-gray-600">
              Book appointments in seconds with real-time availability
            </p>
          </div>
          <div className="bg-white p-8 rounded-xl shadow-md">
            <div className="text-4xl mb-4">💰</div>
            <h3 className="text-xl font-semibold mb-2">Last-Minute Deals</h3>
            <p className="text-gray-600">
              Save money with special promotions on last-minute slots
            </p>
          </div>
        </div>

        {/* Categories Preview */}
        <div className="mt-24">
          <h2 className="text-3xl font-bold text-center mb-12">Popular Categories</h2>
          <div className="grid grid-cols-2 md:grid-cols-4 lg:grid-cols-6 gap-4">
            {['Hair Salon', 'Dental', 'Nail Salon', 'Plumbing', 'HVAC', 'Auto Repair'].map((category) => (
              <div
                key={category}
                className="bg-white p-6 rounded-lg shadow-md hover:shadow-lg transition-shadow cursor-pointer text-center"
              >
                <p className="font-medium text-gray-900">{category}</p>
              </div>
            ))}
          </div>
        </div>
      </main>
    </div>
  )
}
