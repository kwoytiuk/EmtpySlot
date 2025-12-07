export const APP_NAME = 'EmptySlot'
export const APP_VERSION = '1.0.0'

export const API_ENDPOINTS = {
  BASE_URL: process.env.NEXT_PUBLIC_API_URL || 'http://localhost:3000',
  USERS: '/api/users',
  AUTH: '/api/auth',
} as const

export const COLORS = {
  primary: '#0070f3',
  secondary: '#7928ca',
  success: '#00ff00',
  error: '#ff0000',
  warning: '#ffaa00',
  info: '#00aaff',
} as const
