'use client'

import React, { createContext, useContext, useEffect, useState } from 'react'
import { authApi, type User, type Profile } from 'shared'

interface AuthContextType {
  user: User | null
  profile: Profile | null
  loading: boolean
  signOut: () => Promise<void>
  refreshProfile: () => Promise<void>
  refreshUser: () => Promise<void>
}

const AuthContext = createContext<AuthContextType | undefined>(undefined)

export function AuthProvider({ children }: { children: React.ReactNode }) {
  const [user, setUser] = useState<User | null>(null)
  const [profile, setProfile] = useState<Profile | null>(null)
  const [loading, setLoading] = useState(true)

  const fetchProfile = async () => {
    try {
      const profileData = await authApi.getProfile()
      setProfile(profileData)
    } catch (error) {
      console.error('Error fetching profile:', error)
      setProfile(null)
    }
  }

  const fetchUser = async () => {
    try {
      const userData = await authApi.getUser()
      setUser(userData)
      if (userData) {
        await fetchProfile()
      }
    } catch (error) {
      console.error('Error fetching user:', error)
      setUser(null)
      setProfile(null)
    }
  }

  const refreshProfile = async () => {
    if (user) {
      await fetchProfile()
    }
  }

  const refreshUser = async () => {
    await fetchUser()
  }

  useEffect(() => {
    // Get initial user and profile
    fetchUser().finally(() => setLoading(false))
  }, [])

  const signOut = async () => {
    await authApi.signOut()
    setUser(null)
    setProfile(null)
  }

  const value = {
    user,
    profile,
    loading,
    signOut,
    refreshProfile,
    refreshUser,
  }

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>
}

export function useAuth() {
  const context = useContext(AuthContext)
  if (context === undefined) {
    throw new Error('useAuth must be used within an AuthProvider')
  }
  return context
}
