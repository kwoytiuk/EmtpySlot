/**
 * Employees API
 * Functions for managing employees/staff members
 */

import { authApi } from '../lib/apiClient'

export interface Employee {
  id: string
  providerId: string
  firstName: string
  lastName: string
  email: string
  phone?: string | null
  position?: string | null
  isActive: boolean
  hireDate?: string | null
  createdAt: string
  updatedAt: string
}

export interface EmployeeSchedule {
  id: string
  employeeId: string
  dayOfWeek: number // 0-6 (Sunday-Saturday)
  startTime: string
  endTime: string
  isAvailable: boolean
  createdAt: string
  updatedAt: string
}

export interface EmployeeTimeOff {
  id: string
  employeeId: string
  startDate: string
  endDate: string
  reason?: string | null
  createdAt: string
}

export interface CreateEmployeeInput {
  firstName: string
  lastName: string
  email: string
  phone?: string
  position?: string
  hireDate?: string
}

export interface CreateScheduleInput {
  employeeId: string
  dayOfWeek: number
  startTime: string
  endTime: string
}

/**
 * Get all employees for the current provider
 */
export async function getProviderEmployees(): Promise<Employee[]> {
  return authApi.get<Employee[]>('/employees')
}

/**
 * Get a single employee by ID
 */
export async function getEmployeeById(employeeId: string): Promise<Employee | null> {
  try {
    return await authApi.get<Employee>(`/employees/${employeeId}`)
  } catch (error: any) {
    if (error.status === 404) {
      return null
    }
    throw error
  }
}

/**
 * Create a new employee
 */
export async function createEmployee(input: CreateEmployeeInput): Promise<Employee> {
  return authApi.post<Employee>('/employees', input)
}

/**
 * Update an employee
 */
export async function updateEmployee(
  employeeId: string,
  updates: Partial<CreateEmployeeInput>
): Promise<Employee> {
  return authApi.put<Employee>(`/employees/${employeeId}`, updates)
}

/**
 * Deactivate an employee (soft delete)
 */
export async function deactivateEmployee(employeeId: string): Promise<void> {
  await authApi.put(`/employees/${employeeId}/deactivate`, {})
}

/**
 * Get schedules for an employee
 */
export async function getEmployeeSchedules(employeeId: string): Promise<EmployeeSchedule[]> {
  return authApi.get<EmployeeSchedule[]>(`/employees/${employeeId}/schedules`)
}

/**
 * Create a new schedule for an employee
 */
export async function createEmployeeSchedule(input: CreateScheduleInput): Promise<EmployeeSchedule> {
  return authApi.post<EmployeeSchedule>('/employees/schedules', input)
}

/**
 * Update an employee schedule
 */
export async function updateEmployeeSchedule(
  scheduleId: string,
  updates: Partial<CreateScheduleInput>
): Promise<EmployeeSchedule> {
  return authApi.put<EmployeeSchedule>(`/employees/schedules/${scheduleId}`, updates)
}

/**
 * Delete an employee schedule
 */
export async function deleteEmployeeSchedule(scheduleId: string): Promise<void> {
  await authApi.delete(`/employees/schedules/${scheduleId}`)
}

/**
 * Get time off requests for an employee
 */
export async function getEmployeeTimeOff(employeeId: string): Promise<EmployeeTimeOff[]> {
  return authApi.get<EmployeeTimeOff[]>(`/employees/${employeeId}/time-off`)
}

/**
 * Create a time off request for an employee
 */
export async function createEmployeeTimeOff(
  employeeId: string,
  startDate: string,
  endDate: string,
  reason?: string
): Promise<EmployeeTimeOff> {
  return authApi.post<EmployeeTimeOff>(`/employees/${employeeId}/time-off`, {
    startDate,
    endDate,
    reason,
  })
}

/**
 * Delete a time off request
 */
export async function deleteEmployeeTimeOff(timeOffId: string): Promise<void> {
  await authApi.delete(`/employees/time-off/${timeOffId}`)
}
