-- Add employee management and scheduling
-- This migration adds tables for provider employees and their schedules

-- Create employees table
CREATE TABLE IF NOT EXISTS employees (
  id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
  provider_id UUID NOT NULL REFERENCES providers(id) ON DELETE CASCADE,
  first_name VARCHAR(100) NOT NULL,
  last_name VARCHAR(100) NOT NULL,
  email VARCHAR(255) UNIQUE NOT NULL,
  phone VARCHAR(20),
  position VARCHAR(100),
  is_active BOOLEAN DEFAULT true,
  hire_date DATE,
  created_at TIMESTAMP WITH TIME ZONE DEFAULT NOW(),
  updated_at TIMESTAMP WITH TIME ZONE DEFAULT NOW()
);

-- Create employee_schedules table
CREATE TABLE IF NOT EXISTS employee_schedules (
  id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
  employee_id UUID NOT NULL REFERENCES employees(id) ON DELETE CASCADE,
  day_of_week INTEGER NOT NULL CHECK (day_of_week >= 0 AND day_of_week <= 6), -- 0 = Sunday, 6 = Saturday
  start_time TIME NOT NULL,
  end_time TIME NOT NULL,
  is_available BOOLEAN DEFAULT true,
  created_at TIMESTAMP WITH TIME ZONE DEFAULT NOW(),
  updated_at TIMESTAMP WITH TIME ZONE DEFAULT NOW(),
  UNIQUE(employee_id, day_of_week, start_time)
);

-- Create employee_time_off table for tracking unavailable periods
CREATE TABLE IF NOT EXISTS employee_time_off (
  id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
  employee_id UUID NOT NULL REFERENCES employees(id) ON DELETE CASCADE,
  start_date DATE NOT NULL,
  end_date DATE NOT NULL,
  reason VARCHAR(255),
  created_at TIMESTAMP WITH TIME ZONE DEFAULT NOW()
);

-- Add employee_id to appointments table
ALTER TABLE appointments
ADD COLUMN IF NOT EXISTS employee_id UUID REFERENCES employees(id) ON DELETE SET NULL;

-- Add indexes
CREATE INDEX IF NOT EXISTS idx_employees_provider_id ON employees(provider_id);
CREATE INDEX IF NOT EXISTS idx_employees_is_active ON employees(is_active);
CREATE INDEX IF NOT EXISTS idx_employee_schedules_employee_id ON employee_schedules(employee_id);
CREATE INDEX IF NOT EXISTS idx_employee_schedules_day ON employee_schedules(day_of_week);
CREATE INDEX IF NOT EXISTS idx_employee_time_off_employee_id ON employee_time_off(employee_id);
CREATE INDEX IF NOT EXISTS idx_employee_time_off_dates ON employee_time_off(start_date, end_date);
CREATE INDEX IF NOT EXISTS idx_appointments_employee_id ON appointments(employee_id);

-- Add updated_at trigger for employees
CREATE TRIGGER set_employees_updated_at
  BEFORE UPDATE ON employees
  FOR EACH ROW
  EXECUTE FUNCTION update_updated_at_column();

-- Add updated_at trigger for employee_schedules
CREATE TRIGGER set_employee_schedules_updated_at
  BEFORE UPDATE ON employee_schedules
  FOR EACH ROW
  EXECUTE FUNCTION update_updated_at_column();

-- Add RLS policies for employees
ALTER TABLE employees ENABLE ROW LEVEL SECURITY;

CREATE POLICY "Providers can view their own employees"
  ON employees FOR SELECT
  USING (
    provider_id IN (
      SELECT id FROM providers WHERE user_id = auth.uid()
    )
  );

CREATE POLICY "Providers can insert their own employees"
  ON employees FOR INSERT
  WITH CHECK (
    provider_id IN (
      SELECT id FROM providers WHERE user_id = auth.uid()
    )
  );

CREATE POLICY "Providers can update their own employees"
  ON employees FOR UPDATE
  USING (
    provider_id IN (
      SELECT id FROM providers WHERE user_id = auth.uid()
    )
  );

CREATE POLICY "Providers can delete their own employees"
  ON employees FOR DELETE
  USING (
    provider_id IN (
      SELECT id FROM providers WHERE user_id = auth.uid()
    )
  );

-- Add RLS policies for employee_schedules
ALTER TABLE employee_schedules ENABLE ROW LEVEL SECURITY;

CREATE POLICY "Providers can manage schedules for their employees"
  ON employee_schedules FOR ALL
  USING (
    employee_id IN (
      SELECT e.id FROM employees e
      JOIN providers p ON e.provider_id = p.id
      WHERE p.user_id = auth.uid()
    )
  );

CREATE POLICY "Customers can view schedules"
  ON employee_schedules FOR SELECT
  USING (true);

-- Add RLS policies for employee_time_off
ALTER TABLE employee_time_off ENABLE ROW LEVEL SECURITY;

CREATE POLICY "Providers can manage time off for their employees"
  ON employee_time_off FOR ALL
  USING (
    employee_id IN (
      SELECT e.id FROM employees e
      JOIN providers p ON e.provider_id = p.id
      WHERE p.user_id = auth.uid()
    )
  );

COMMENT ON TABLE employees IS 'Provider employees who can service appointments';
COMMENT ON TABLE employee_schedules IS 'Weekly recurring schedules for employees';
COMMENT ON TABLE employee_time_off IS 'Time off periods when employees are unavailable';
COMMENT ON COLUMN employee_schedules.day_of_week IS '0=Sunday, 1=Monday, 2=Tuesday, 3=Wednesday, 4=Thursday, 5=Friday, 6=Saturday';
