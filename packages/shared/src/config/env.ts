/**
 * Environment configuration
 * Validates required environment variables at runtime
 * Supports both Next.js (NEXT_PUBLIC_) and Expo (EXPO_PUBLIC_) prefixes
 */

export const env = {
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
  const requiredEnvVars = [
    'NEXT_PUBLIC_SUPABASE_URL',
    'NEXT_PUBLIC_SUPABASE_ANON_KEY',
  ]

  const missing = requiredEnvVars.filter((key) => !process.env[key])

  if (missing.length > 0) {
    throw new Error(
      `Missing required environment variables:\n${missing.map((key) => `  - ${key}`).join('\n')}\n\n` +
        `Please check your .env.local file.`
    )
  }

  // Validate URLs
  try {
    new URL(env.supabase.url)
  } catch {
    throw new Error(
      `Invalid NEXT_PUBLIC_SUPABASE_URL: ${env.supabase.url}\n` +
        `Please provide a valid URL (e.g., https://xxx.supabase.co)`
    )
  }
}
