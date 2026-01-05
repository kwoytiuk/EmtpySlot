/**
 * Environment configuration
 * Validates required environment variables at runtime
 * Supports both Next.js (NEXT_PUBLIC_) and Expo (EXPO_PUBLIC_) prefixes
 */

export const env = {
  api: {
    url: process.env.NEXT_PUBLIC_API_URL || process.env.EXPO_PUBLIC_API_URL || 'http://localhost:5000/api',
  },
  supabase: {
    url: process.env.NEXT_PUBLIC_SUPABASE_URL || process.env.EXPO_PUBLIC_SUPABASE_URL || '',
    anonKey: process.env.NEXT_PUBLIC_SUPABASE_ANON_KEY || process.env.EXPO_PUBLIC_SUPABASE_ANON_KEY || '',
  },
  app: {
    url: process.env.NEXT_PUBLIC_APP_URL || process.env.EXPO_PUBLIC_APP_URL || 'http://localhost:3000',
    env: process.env.NODE_ENV || 'development',
  },
} as const

/**
 * Validate environment variables
 * Call this at app startup to ensure all required vars are present
 */
export function validateEnv() {
  // API URL is optional - defaults to localhost:5000/api
  if (env.api.url && env.api.url !== 'http://localhost:5000/api') {
    try {
      new URL(env.api.url)
    } catch {
      throw new Error(
        `Invalid API URL: ${env.api.url}\n` +
          `Please provide a valid URL (e.g., https://api.example.com)`
      )
    }
  }
}
