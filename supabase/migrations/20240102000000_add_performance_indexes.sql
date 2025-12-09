-- Add performance indexes for faster queries

-- Providers table indexes
CREATE INDEX IF NOT EXISTS idx_providers_verified ON providers(verified) WHERE verified = true;
CREATE INDEX IF NOT EXISTS idx_providers_rating ON providers(rating_average DESC) WHERE rating_average IS NOT NULL;
CREATE INDEX IF NOT EXISTS idx_providers_user_id ON providers(user_id);

-- Services table indexes
CREATE INDEX IF NOT EXISTS idx_services_provider_id ON services(provider_id);
CREATE INDEX IF NOT EXISTS idx_services_category_id ON services(category_id);
CREATE INDEX IF NOT EXISTS idx_services_price ON services(price);

-- Appointments table indexes
CREATE INDEX IF NOT EXISTS idx_appointments_customer_id ON appointments(customer_id);
CREATE INDEX IF NOT EXISTS idx_appointments_provider_id ON appointments(provider_id);
CREATE INDEX IF NOT EXISTS idx_appointments_date_time ON appointments(appointment_date, start_time);
CREATE INDEX IF NOT EXISTS idx_appointments_status ON appointments(status);
CREATE INDEX IF NOT EXISTS idx_appointments_staff_id ON appointments(staff_id) WHERE staff_id IS NOT NULL;

-- Provider locations indexes
CREATE INDEX IF NOT EXISTS idx_provider_locations_provider_id ON provider_locations(provider_id);
CREATE INDEX IF NOT EXISTS idx_provider_locations_primary ON provider_locations(is_primary) WHERE is_primary = true;

-- Employee schedules indexes
CREATE INDEX IF NOT EXISTS idx_employee_schedules_employee_id ON employee_schedules(employee_id);
CREATE INDEX IF NOT EXISTS idx_employee_schedules_day ON employee_schedules(day_of_week, is_available);

-- Employee time off indexes
CREATE INDEX IF NOT EXISTS idx_employee_time_off_employee_id ON employee_time_off(employee_id);
CREATE INDEX IF NOT EXISTS idx_employee_time_off_dates ON employee_time_off(start_date, end_date);

-- Reviews indexes
CREATE INDEX IF NOT EXISTS idx_reviews_provider_id ON reviews(provider_id);
CREATE INDEX IF NOT EXISTS idx_reviews_rating ON reviews(rating);

-- Composite index for common appointment queries
CREATE INDEX IF NOT EXISTS idx_appointments_provider_date_status
  ON appointments(provider_id, appointment_date, status);

COMMENT ON INDEX idx_providers_verified IS 'Speed up queries for verified providers';
COMMENT ON INDEX idx_appointments_date_time IS 'Speed up time slot availability checks';
COMMENT ON INDEX idx_appointments_provider_date_status IS 'Composite index for provider dashboard queries';
