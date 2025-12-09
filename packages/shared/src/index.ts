// Utilities
export * from './utils'
export * from './constants'

// Types
export * from './types'
export * from './types/database.types'

// Configuration
export * from './config/env'

// Supabase client and types
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
