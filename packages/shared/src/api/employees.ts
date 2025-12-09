import { supabase } from '../lib/supabase';

export interface Employee {
  id: string;
  provider_id: string;
  first_name: string;
  last_name: string;
  email: string;
  phone?: string;
  position?: string;
  is_active: boolean;
  hire_date?: string;
  created_at: string;
  updated_at: string;
}

export interface EmployeeSchedule {
  id: string;
  employee_id: string;
  day_of_week: number; // 0-6 (Sunday-Saturday)
  start_time: string;
  end_time: string;
  is_available: boolean;
  created_at: string;
  updated_at: string;
}

export interface EmployeeTimeOff {
  id: string;
  employee_id: string;
  start_date: string;
  end_date: string;
  reason?: string;
  created_at: string;
}

export interface CreateEmployeeInput {
  first_name: string;
  last_name: string;
  email: string;
  phone?: string;
  position?: string;
  hire_date?: string;
}

export interface CreateScheduleInput {
  employee_id: string;
  day_of_week: number;
  start_time: string;
  end_time: string;
}

/**
 * Get all employees for the current provider
 */
export async function getProviderEmployees(): Promise<Employee[]> {
  const { data: profile } = await supabase
    .from('profiles')
    .select('id')
    .eq('id', (await supabase.auth.getUser()).data.user?.id)
    .single();

  if (!profile) return [];

  const { data: provider } = await supabase
    .from('providers')
    .select('id')
    .eq('user_id', profile.id)
    .single();

  if (!provider) return [];

  const { data, error } = await supabase
    .from('employees')
    .select('*')
    .eq('provider_id', provider.id)
    .eq('is_active', true)
    .order('last_name', { ascending: true });

  if (error) throw error;
  return data || [];
}

/**
 * Get a single employee by ID
 */
export async function getEmployeeById(employeeId: string): Promise<Employee | null> {
  const { data, error } = await supabase
    .from('employees')
    .select('*')
    .eq('id', employeeId)
    .single();

  if (error) throw error;
  return data;
}

/**
 * Create a new employee
 */
export async function createEmployee(
  input: CreateEmployeeInput
): Promise<{ employee: Employee | null; error: any }> {
  try {
    const { data: profile } = await supabase
      .from('profiles')
      .select('id')
      .eq('id', (await supabase.auth.getUser()).data.user?.id)
      .single();

    if (!profile) {
      return { employee: null, error: { message: 'Not authenticated' } };
    }

    const { data: provider } = await supabase
      .from('providers')
      .select('id')
      .eq('user_id', profile.id)
      .single();

    if (!provider) {
      return { employee: null, error: { message: 'Provider not found' } };
    }

    const { data, error } = await supabase
      .from('employees')
      .insert({
        provider_id: provider.id,
        ...input,
      })
      .select()
      .single();

    return { employee: data, error };
  } catch (error) {
    return { employee: null, error };
  }
}

/**
 * Update an employee
 */
export async function updateEmployee(
  employeeId: string,
  updates: Partial<CreateEmployeeInput>
): Promise<{ error: any }> {
  const { error } = await supabase
    .from('employees')
    .update(updates)
    .eq('id', employeeId);

  return { error };
}

/**
 * Deactivate an employee (soft delete)
 */
export async function deactivateEmployee(employeeId: string): Promise<{ error: any }> {
  const { error } = await supabase
    .from('employees')
    .update({ is_active: false })
    .eq('id', employeeId);

  return { error };
}

/**
 * Get schedule for an employee
 */
export async function getEmployeeSchedule(employeeId: string): Promise<EmployeeSchedule[]> {
  const { data, error } = await supabase
    .from('employee_schedules')
    .select('*')
    .eq('employee_id', employeeId)
    .order('day_of_week', { ascending: true })
    .order('start_time', { ascending: true });

  if (error) throw error;
  return data || [];
}

/**
 * Create or update employee schedule
 */
export async function upsertEmployeeSchedule(
  schedule: CreateScheduleInput
): Promise<{ error: any }> {
  const { error } = await supabase
    .from('employee_schedules')
    .upsert(schedule, {
      onConflict: 'employee_id,day_of_week,start_time',
    });

  return { error };
}

/**
 * Delete a schedule entry
 */
export async function deleteEmployeeSchedule(scheduleId: string): Promise<{ error: any }> {
  const { error } = await supabase
    .from('employee_schedules')
    .delete()
    .eq('id', scheduleId);

  return { error };
}

/**
 * Get available employees for a specific date/time
 */
export async function getAvailableEmployees(
  providerId: string,
  date: string,
  startTime: string
): Promise<Employee[]> {
  const dayOfWeek = new Date(date).getDay();

  // Get all active employees for the provider
  const { data: employees, error: employeesError } = await supabase
    .from('employees')
    .select('*')
    .eq('provider_id', providerId)
    .eq('is_active', true);

  if (employeesError || !employees) return [];

  // Filter employees based on schedules and time off
  const availableEmployees: Employee[] = [];

  for (const employee of employees) {
    // Check if employee has a schedule for this day
    const { data: schedules } = await supabase
      .from('employee_schedules')
      .select('*')
      .eq('employee_id', employee.id)
      .eq('day_of_week', dayOfWeek)
      .eq('is_available', true);

    if (!schedules || schedules.length === 0) continue;

    // Check if the requested time falls within any schedule
    const isWithinSchedule = schedules.some((schedule) => {
      return startTime >= schedule.start_time && startTime < schedule.end_time;
    });

    if (!isWithinSchedule) continue;

    // Check if employee has time off on this date
    const { data: timeOff } = await supabase
      .from('employee_time_off')
      .select('*')
      .eq('employee_id', employee.id)
      .lte('start_date', date)
      .gte('end_date', date);

    if (timeOff && timeOff.length > 0) continue;

    availableEmployees.push(employee);
  }

  return availableEmployees;
}

/**
 * Add time off for an employee
 */
export async function addEmployeeTimeOff(
  employeeId: string,
  startDate: string,
  endDate: string,
  reason?: string
): Promise<{ error: any }> {
  const { error } = await supabase.from('employee_time_off').insert({
    employee_id: employeeId,
    start_date: startDate,
    end_date: endDate,
    reason,
  });

  return { error };
}

/**
 * Get time off for an employee
 */
export async function getEmployeeTimeOff(employeeId: string): Promise<EmployeeTimeOff[]> {
  const { data, error } = await supabase
    .from('employee_time_off')
    .select('*')
    .eq('employee_id', employeeId)
    .order('start_date', { ascending: true });

  if (error) throw error;
  return data || [];
}
