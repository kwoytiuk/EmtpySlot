'use client'

import Link from 'next/link'
import { useAuth } from '@/contexts/AuthContext'
import { Button } from '@/components/ui/button'
import { useRouter } from 'next/navigation'

export function HomeNavigation() {
  const { user, signOut, loading } = useAuth()
  const router = useRouter()

  const handleSignOut = async () => {
    await signOut()
    router.push('/')
    router.refresh()
  }

  return (
    <nav className="absolute top-0 left-0 right-0 z-50">
      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-6">
        <div className="flex justify-between items-center">
          <Link href="/" className="text-2xl font-black text-white">
            EmptySlot
          </Link>
          <div className="flex items-center gap-6">
            <Link href="/search" className="text-white hover:text-blue-200 font-medium">
              Find Services
            </Link>
            {loading ? (
              <div className="text-white">Loading...</div>
            ) : user ? (
              <>
                <Link href="/appointments" className="text-white hover:text-blue-200 font-medium">
                  My Bookings
                </Link>
                <Link href="/provider/dashboard" className="text-white hover:text-blue-200 font-medium">
                  Dashboard
                </Link>
                <button
                  onClick={handleSignOut}
                  className="text-white hover:text-blue-200 font-medium"
                >
                  Sign Out
                </button>
              </>
            ) : (
              <>
                <Link href="/auth/login" className="text-white hover:text-blue-200 font-medium">
                  Log In
                </Link>
                <Link
                  href="/auth/signup"
                  className="bg-white text-blue-600 px-6 py-2 rounded-full font-bold hover:bg-blue-50 transition-colors"
                >
                  Sign Up
                </Link>
              </>
            )}
          </div>
        </div>
      </div>
    </nav>
  )
}
