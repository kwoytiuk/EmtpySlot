// Utilities
export * from './utils'
export * from './constants'

// Types
export * from './types'
export * from './types/database.types'

// Configuration
export * from './config/env'

// API Client
export * from './lib/apiClient'

// Supabase client and types (legacy - for migration)
export * from './lib/supabase'

// API
export * as authApi from './api/auth'
export * as providersApi from './api/providers'
export * as categoriesApi from './api/categories'
export * as appointmentsApi from './api/appointments'
export * as reviewsApi from './api/reviews'
export * as employeesApi from './api/employees'

// Employee types
export type {
  Employee,
  EmployeeSchedule,
  EmployeeTimeOff,
  CreateEmployeeInput,
  CreateScheduleInput,
} from './api/employees'

// Provider types
export type {
  Provider,
  ProviderLocation,
  Service,
  SearchProvidersParams,
  SearchProvidersResponse,
  CreateProviderData,
  Review,
} from './api/providers'

// Category types
export type {
  ServiceCategory,
} from './api/categories'

// Appointment types
export type {
  Appointment,
  CreateAppointmentData,
  TimeSlot,
} from './api/appointments'

// Auth types
export type {
  User,
  Profile,
  SignUpData,
  SignInData,
  AuthResponse,
  UserType,
} from './api/auth'

// Review types
export type {
  ReviewResponse,
  CreateReviewData,
} from './api/reviews'
