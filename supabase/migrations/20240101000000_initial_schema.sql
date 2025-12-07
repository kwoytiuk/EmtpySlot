-- Enable PostGIS extension for geospatial queries
CREATE EXTENSION IF NOT EXISTS postgis;

-- Enable UUID extension
CREATE EXTENSION IF NOT EXISTS "uuid-ossp";

-- Create custom types
CREATE TYPE user_type AS ENUM ('customer', 'provider', 'admin');
CREATE TYPE appointment_status AS ENUM ('pending', 'confirmed', 'completed', 'cancelled', 'no_show');
CREATE TYPE payment_status AS ENUM ('pending', 'paid', 'refunded');
CREATE TYPE discount_type AS ENUM ('percentage', 'fixed_amount');

-- =============================================
-- PROFILES TABLE (extends auth.users)
-- =============================================
CREATE TABLE profiles (
  id UUID PRIMARY KEY REFERENCES auth.users(id) ON DELETE CASCADE,
  user_type user_type NOT NULL DEFAULT 'customer',
  full_name TEXT,
  phone TEXT,
  avatar_url TEXT,
  created_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
  updated_at TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

-- Enable RLS
ALTER TABLE profiles ENABLE ROW LEVEL SECURITY;

-- Policies
CREATE POLICY "Public profiles are viewable by everyone"
  ON profiles FOR SELECT
  USING (true);

CREATE POLICY "Users can update own profile"
  ON profiles FOR UPDATE
  USING (auth.uid() = id);

-- =============================================
-- SERVICE CATEGORIES
-- =============================================
CREATE TABLE service_categories (
  id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
  name TEXT NOT NULL,
  slug TEXT UNIQUE NOT NULL,
  icon TEXT,
  description TEXT,
  parent_id UUID REFERENCES service_categories(id) ON DELETE SET NULL,
  display_order INTEGER DEFAULT 0,
  created_at TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

-- Enable RLS
ALTER TABLE service_categories ENABLE ROW LEVEL SECURITY;

-- Policies
CREATE POLICY "Categories are viewable by everyone"
  ON service_categories FOR SELECT
  USING (true);

-- =============================================
-- PROVIDERS
-- =============================================
CREATE TABLE providers (
  id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
  user_id UUID NOT NULL REFERENCES profiles(id) ON DELETE CASCADE,
  business_name TEXT NOT NULL,
  description TEXT,
  phone TEXT,
  email TEXT,
  website TEXT,
  logo_url TEXT,
  cover_image_url TEXT,
  verified BOOLEAN DEFAULT false,
  rating_average DECIMAL(3,2) DEFAULT 0,
  rating_count INTEGER DEFAULT 0,
  cancellation_policy TEXT,
  created_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
  updated_at TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

-- Enable RLS
ALTER TABLE providers ENABLE ROW LEVEL SECURITY;

-- Policies
CREATE POLICY "Providers are viewable by everyone"
  ON providers FOR SELECT
  USING (true);

CREATE POLICY "Users can create their own provider profile"
  ON providers FOR INSERT
  WITH CHECK (auth.uid() = user_id);

CREATE POLICY "Users can update their own provider profile"
  ON providers FOR UPDATE
  USING (auth.uid() = user_id);

-- Indexes
CREATE INDEX idx_providers_user_id ON providers(user_id);
CREATE INDEX idx_providers_verified ON providers(verified);

-- =============================================
-- PROVIDER LOCATIONS
-- =============================================
CREATE TABLE provider_locations (
  id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
  provider_id UUID NOT NULL REFERENCES providers(id) ON DELETE CASCADE,
  name TEXT NOT NULL,
  address_line1 TEXT NOT NULL,
  address_line2 TEXT,
  city TEXT NOT NULL,
  state_province TEXT NOT NULL,
  postal_code TEXT NOT NULL,
  country TEXT NOT NULL DEFAULT 'USA',
  latitude DECIMAL(10, 8) NOT NULL,
  longitude DECIMAL(11, 8) NOT NULL,
  location GEOGRAPHY(Point, 4326),
  phone TEXT,
  is_primary BOOLEAN DEFAULT false,
  created_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
  updated_at TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

-- Enable RLS
ALTER TABLE provider_locations ENABLE ROW LEVEL SECURITY;

-- Policies
CREATE POLICY "Locations are viewable by everyone"
  ON provider_locations FOR SELECT
  USING (true);

CREATE POLICY "Providers can manage their own locations"
  ON provider_locations FOR ALL
  USING (
    provider_id IN (
      SELECT id FROM providers WHERE user_id = auth.uid()
    )
  );

-- Create geography column from lat/lng
CREATE OR REPLACE FUNCTION update_location_geography()
RETURNS TRIGGER AS $$
BEGIN
  NEW.location := ST_SetSRID(ST_MakePoint(NEW.longitude, NEW.latitude), 4326)::geography;
  RETURN NEW;
END;
$$ LANGUAGE plpgsql;

CREATE TRIGGER set_location_geography
  BEFORE INSERT OR UPDATE ON provider_locations
  FOR EACH ROW
  EXECUTE FUNCTION update_location_geography();

-- Spatial index
CREATE INDEX idx_provider_locations_location ON provider_locations USING GIST (location);
CREATE INDEX idx_provider_locations_provider_id ON provider_locations(provider_id);

-- =============================================
-- SERVICES
-- =============================================
CREATE TABLE services (
  id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
  provider_id UUID NOT NULL REFERENCES providers(id) ON DELETE CASCADE,
  category_id UUID NOT NULL REFERENCES service_categories(id),
  name TEXT NOT NULL,
  description TEXT,
  duration_minutes INTEGER NOT NULL,
  price DECIMAL(10,2) NOT NULL,
  deposit_required DECIMAL(10,2),
  image_url TEXT,
  is_active BOOLEAN DEFAULT true,
  created_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
  updated_at TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

-- Enable RLS
ALTER TABLE services ENABLE ROW LEVEL SECURITY;

-- Policies
CREATE POLICY "Active services are viewable by everyone"
  ON services FOR SELECT
  USING (is_active = true OR provider_id IN (
    SELECT id FROM providers WHERE user_id = auth.uid()
  ));

CREATE POLICY "Providers can manage their own services"
  ON services FOR ALL
  USING (
    provider_id IN (
      SELECT id FROM providers WHERE user_id = auth.uid()
    )
  );

-- Indexes
CREATE INDEX idx_services_provider_id ON services(provider_id);
CREATE INDEX idx_services_category_id ON services(category_id);
CREATE INDEX idx_services_active ON services(is_active);

-- =============================================
-- STAFF MEMBERS
-- =============================================
CREATE TABLE staff_members (
  id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
  provider_id UUID NOT NULL REFERENCES providers(id) ON DELETE CASCADE,
  name TEXT NOT NULL,
  email TEXT,
  phone TEXT,
  avatar_url TEXT,
  bio TEXT,
  is_active BOOLEAN DEFAULT true,
  created_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
  updated_at TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

-- Enable RLS
ALTER TABLE staff_members ENABLE ROW LEVEL SECURITY;

-- Policies
CREATE POLICY "Active staff are viewable by everyone"
  ON staff_members FOR SELECT
  USING (is_active = true);

CREATE POLICY "Providers can manage their own staff"
  ON staff_members FOR ALL
  USING (
    provider_id IN (
      SELECT id FROM providers WHERE user_id = auth.uid()
    )
  );

-- Indexes
CREATE INDEX idx_staff_members_provider_id ON staff_members(provider_id);

-- =============================================
-- STAFF SERVICES (Junction Table)
-- =============================================
CREATE TABLE staff_services (
  staff_id UUID NOT NULL REFERENCES staff_members(id) ON DELETE CASCADE,
  service_id UUID NOT NULL REFERENCES services(id) ON DELETE CASCADE,
  PRIMARY KEY (staff_id, service_id)
);

-- Enable RLS
ALTER TABLE staff_services ENABLE ROW LEVEL SECURITY;

-- Policies
CREATE POLICY "Staff services are viewable by everyone"
  ON staff_services FOR SELECT
  USING (true);

CREATE POLICY "Providers can manage their staff services"
  ON staff_services FOR ALL
  USING (
    staff_id IN (
      SELECT sm.id FROM staff_members sm
      JOIN providers p ON sm.provider_id = p.id
      WHERE p.user_id = auth.uid()
    )
  );

-- =============================================
-- AVAILABILITY SCHEDULES
-- =============================================
CREATE TABLE availability_schedules (
  id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
  provider_id UUID NOT NULL REFERENCES providers(id) ON DELETE CASCADE,
  staff_id UUID REFERENCES staff_members(id) ON DELETE CASCADE,
  location_id UUID NOT NULL REFERENCES provider_locations(id) ON DELETE CASCADE,
  day_of_week INTEGER NOT NULL CHECK (day_of_week >= 0 AND day_of_week <= 6),
  start_time TIME NOT NULL,
  end_time TIME NOT NULL,
  is_active BOOLEAN DEFAULT true,
  created_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
  CHECK (end_time > start_time)
);

-- Enable RLS
ALTER TABLE availability_schedules ENABLE ROW LEVEL SECURITY;

-- Policies
CREATE POLICY "Schedules are viewable by everyone"
  ON availability_schedules FOR SELECT
  USING (is_active = true);

CREATE POLICY "Providers can manage their schedules"
  ON availability_schedules FOR ALL
  USING (
    provider_id IN (
      SELECT id FROM providers WHERE user_id = auth.uid()
    )
  );

-- Indexes
CREATE INDEX idx_availability_schedules_provider_id ON availability_schedules(provider_id);
CREATE INDEX idx_availability_schedules_location_id ON availability_schedules(location_id);

-- =============================================
-- AVAILABILITY EXCEPTIONS
-- =============================================
CREATE TABLE availability_exceptions (
  id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
  provider_id UUID NOT NULL REFERENCES providers(id) ON DELETE CASCADE,
  staff_id UUID REFERENCES staff_members(id) ON DELETE CASCADE,
  location_id UUID NOT NULL REFERENCES provider_locations(id) ON DELETE CASCADE,
  date DATE NOT NULL,
  start_time TIME,
  end_time TIME,
  reason TEXT,
  created_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
  CHECK (start_time IS NULL OR end_time IS NULL OR end_time > start_time)
);

-- Enable RLS
ALTER TABLE availability_exceptions ENABLE ROW LEVEL SECURITY;

-- Policies
CREATE POLICY "Exceptions are viewable by everyone"
  ON availability_exceptions FOR SELECT
  USING (true);

CREATE POLICY "Providers can manage their exceptions"
  ON availability_exceptions FOR ALL
  USING (
    provider_id IN (
      SELECT id FROM providers WHERE user_id = auth.uid()
    )
  );

-- Indexes
CREATE INDEX idx_availability_exceptions_provider_id ON availability_exceptions(provider_id);
CREATE INDEX idx_availability_exceptions_date ON availability_exceptions(date);

-- =============================================
-- APPOINTMENTS
-- =============================================
CREATE TABLE appointments (
  id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
  booking_reference TEXT UNIQUE NOT NULL,
  customer_id UUID NOT NULL REFERENCES profiles(id),
  provider_id UUID NOT NULL REFERENCES providers(id),
  location_id UUID NOT NULL REFERENCES provider_locations(id),
  service_id UUID NOT NULL REFERENCES services(id),
  staff_id UUID REFERENCES staff_members(id),
  appointment_date DATE NOT NULL,
  start_time TIME NOT NULL,
  end_time TIME NOT NULL,
  status appointment_status DEFAULT 'pending',
  price DECIMAL(10,2) NOT NULL,
  deposit_paid DECIMAL(10,2),
  payment_status payment_status DEFAULT 'pending',
  payment_intent_id TEXT,
  customer_notes TEXT,
  provider_notes TEXT,
  cancelled_by UUID REFERENCES profiles(id),
  cancelled_at TIMESTAMPTZ,
  cancellation_reason TEXT,
  created_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
  updated_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
  CHECK (end_time > start_time)
);

-- Enable RLS
ALTER TABLE appointments ENABLE ROW LEVEL SECURITY;

-- Policies
CREATE POLICY "Customers can view their own appointments"
  ON appointments FOR SELECT
  USING (auth.uid() = customer_id);

CREATE POLICY "Providers can view their appointments"
  ON appointments FOR SELECT
  USING (
    provider_id IN (
      SELECT id FROM providers WHERE user_id = auth.uid()
    )
  );

CREATE POLICY "Customers can create appointments"
  ON appointments FOR INSERT
  WITH CHECK (auth.uid() = customer_id);

CREATE POLICY "Customers can update their own appointments"
  ON appointments FOR UPDATE
  USING (auth.uid() = customer_id);

CREATE POLICY "Providers can update their appointments"
  ON appointments FOR UPDATE
  USING (
    provider_id IN (
      SELECT id FROM providers WHERE user_id = auth.uid()
    )
  );

-- Generate booking reference
CREATE OR REPLACE FUNCTION generate_booking_reference()
RETURNS TEXT AS $$
DECLARE
  chars TEXT := 'ABCDEFGHJKLMNPQRSTUVWXYZ23456789';
  result TEXT := 'ES-';
  i INTEGER;
BEGIN
  FOR i IN 1..6 LOOP
    result := result || substr(chars, floor(random() * length(chars) + 1)::int, 1);
  END LOOP;
  RETURN result;
END;
$$ LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION set_booking_reference()
RETURNS TRIGGER AS $$
BEGIN
  IF NEW.booking_reference IS NULL THEN
    NEW.booking_reference := generate_booking_reference();
  END IF;
  RETURN NEW;
END;
$$ LANGUAGE plpgsql;

CREATE TRIGGER set_appointment_booking_reference
  BEFORE INSERT ON appointments
  FOR EACH ROW
  EXECUTE FUNCTION set_booking_reference();

-- Indexes
CREATE INDEX idx_appointments_customer_id ON appointments(customer_id);
CREATE INDEX idx_appointments_provider_id ON appointments(provider_id);
CREATE INDEX idx_appointments_date_status ON appointments(appointment_date, status);
CREATE INDEX idx_appointments_booking_ref ON appointments(booking_reference);

-- =============================================
-- PROMOTIONS
-- =============================================
CREATE TABLE promotions (
  id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
  provider_id UUID NOT NULL REFERENCES providers(id) ON DELETE CASCADE,
  service_id UUID REFERENCES services(id) ON DELETE CASCADE,
  title TEXT NOT NULL,
  description TEXT,
  discount_type discount_type NOT NULL,
  discount_value DECIMAL(10,2) NOT NULL,
  valid_from TIMESTAMPTZ NOT NULL,
  valid_until TIMESTAMPTZ NOT NULL,
  max_uses INTEGER,
  current_uses INTEGER DEFAULT 0,
  min_notice_hours INTEGER DEFAULT 0,
  is_active BOOLEAN DEFAULT true,
  created_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
  updated_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
  CHECK (valid_until > valid_from),
  CHECK (discount_value > 0)
);

-- Enable RLS
ALTER TABLE promotions ENABLE ROW LEVEL SECURITY;

-- Policies
CREATE POLICY "Active promotions are viewable by everyone"
  ON promotions FOR SELECT
  USING (is_active = true AND valid_from <= NOW() AND valid_until >= NOW());

CREATE POLICY "Providers can manage their promotions"
  ON promotions FOR ALL
  USING (
    provider_id IN (
      SELECT id FROM providers WHERE user_id = auth.uid()
    )
  );

-- Indexes
CREATE INDEX idx_promotions_provider_id ON promotions(provider_id);
CREATE INDEX idx_promotions_active_dates ON promotions(is_active, valid_from, valid_until);

-- =============================================
-- REVIEWS
-- =============================================
CREATE TABLE reviews (
  id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
  appointment_id UUID UNIQUE NOT NULL REFERENCES appointments(id),
  customer_id UUID NOT NULL REFERENCES profiles(id),
  provider_id UUID NOT NULL REFERENCES providers(id),
  rating INTEGER NOT NULL CHECK (rating >= 1 AND rating <= 5),
  comment TEXT,
  response TEXT,
  response_at TIMESTAMPTZ,
  is_verified BOOLEAN DEFAULT false,
  created_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
  updated_at TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

-- Enable RLS
ALTER TABLE reviews ENABLE ROW LEVEL SECURITY;

-- Policies
CREATE POLICY "Reviews are viewable by everyone"
  ON reviews FOR SELECT
  USING (true);

CREATE POLICY "Customers can create reviews for their appointments"
  ON reviews FOR INSERT
  WITH CHECK (
    auth.uid() = customer_id
    AND appointment_id IN (
      SELECT id FROM appointments WHERE customer_id = auth.uid() AND status = 'completed'
    )
  );

CREATE POLICY "Providers can respond to their reviews"
  ON reviews FOR UPDATE
  USING (
    provider_id IN (
      SELECT id FROM providers WHERE user_id = auth.uid()
    )
  );

-- Update provider rating when review is created/updated
CREATE OR REPLACE FUNCTION update_provider_rating()
RETURNS TRIGGER AS $$
BEGIN
  UPDATE providers
  SET
    rating_average = (
      SELECT AVG(rating)::DECIMAL(3,2)
      FROM reviews
      WHERE provider_id = NEW.provider_id
    ),
    rating_count = (
      SELECT COUNT(*)
      FROM reviews
      WHERE provider_id = NEW.provider_id
    )
  WHERE id = NEW.provider_id;
  RETURN NEW;
END;
$$ LANGUAGE plpgsql;

CREATE TRIGGER update_provider_rating_on_review
  AFTER INSERT OR UPDATE ON reviews
  FOR EACH ROW
  EXECUTE FUNCTION update_provider_rating();

-- Indexes
CREATE INDEX idx_reviews_provider_id ON reviews(provider_id);
CREATE INDEX idx_reviews_customer_id ON reviews(customer_id);

-- =============================================
-- FAVORITES
-- =============================================
CREATE TABLE favorites (
  id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
  customer_id UUID NOT NULL REFERENCES profiles(id) ON DELETE CASCADE,
  provider_id UUID NOT NULL REFERENCES providers(id) ON DELETE CASCADE,
  created_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
  UNIQUE(customer_id, provider_id)
);

-- Enable RLS
ALTER TABLE favorites ENABLE ROW LEVEL SECURITY;

-- Policies
CREATE POLICY "Users can view their own favorites"
  ON favorites FOR SELECT
  USING (auth.uid() = customer_id);

CREATE POLICY "Users can manage their own favorites"
  ON favorites FOR ALL
  USING (auth.uid() = customer_id);

-- Indexes
CREATE INDEX idx_favorites_customer_id ON favorites(customer_id);
CREATE INDEX idx_favorites_provider_id ON favorites(provider_id);

-- =============================================
-- NOTIFICATIONS
-- =============================================
CREATE TYPE notification_type AS ENUM (
  'booking_confirmed',
  'booking_reminder',
  'booking_cancelled',
  'review_request',
  'promotion'
);

CREATE TABLE notifications (
  id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
  user_id UUID NOT NULL REFERENCES profiles(id) ON DELETE CASCADE,
  type notification_type NOT NULL,
  title TEXT NOT NULL,
  message TEXT NOT NULL,
  data JSONB,
  read BOOLEAN DEFAULT false,
  created_at TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

-- Enable RLS
ALTER TABLE notifications ENABLE ROW LEVEL SECURITY;

-- Policies
CREATE POLICY "Users can view their own notifications"
  ON notifications FOR SELECT
  USING (auth.uid() = user_id);

CREATE POLICY "Users can update their own notifications"
  ON notifications FOR UPDATE
  USING (auth.uid() = user_id);

-- Indexes
CREATE INDEX idx_notifications_user_id ON notifications(user_id);
CREATE INDEX idx_notifications_read ON notifications(read);
CREATE INDEX idx_notifications_created_at ON notifications(created_at);

-- =============================================
-- UPDATED_AT TRIGGER FUNCTION
-- =============================================
CREATE OR REPLACE FUNCTION update_updated_at_column()
RETURNS TRIGGER AS $$
BEGIN
  NEW.updated_at = NOW();
  RETURN NEW;
END;
$$ LANGUAGE plpgsql;

-- Apply to all tables with updated_at
CREATE TRIGGER update_profiles_updated_at BEFORE UPDATE ON profiles FOR EACH ROW EXECUTE FUNCTION update_updated_at_column();
CREATE TRIGGER update_providers_updated_at BEFORE UPDATE ON providers FOR EACH ROW EXECUTE FUNCTION update_updated_at_column();
CREATE TRIGGER update_provider_locations_updated_at BEFORE UPDATE ON provider_locations FOR EACH ROW EXECUTE FUNCTION update_updated_at_column();
CREATE TRIGGER update_services_updated_at BEFORE UPDATE ON services FOR EACH ROW EXECUTE FUNCTION update_updated_at_column();
CREATE TRIGGER update_staff_members_updated_at BEFORE UPDATE ON staff_members FOR EACH ROW EXECUTE FUNCTION update_updated_at_column();
CREATE TRIGGER update_appointments_updated_at BEFORE UPDATE ON appointments FOR EACH ROW EXECUTE FUNCTION update_updated_at_column();
CREATE TRIGGER update_promotions_updated_at BEFORE UPDATE ON promotions FOR EACH ROW EXECUTE FUNCTION update_updated_at_column();
CREATE TRIGGER update_reviews_updated_at BEFORE UPDATE ON reviews FOR EACH ROW EXECUTE FUNCTION update_updated_at_column();

-- =============================================
-- AUTO-CREATE PROFILE ON SIGNUP
-- =============================================
CREATE OR REPLACE FUNCTION handle_new_user()
RETURNS TRIGGER AS $$
BEGIN
  INSERT INTO public.profiles (id, full_name, avatar_url)
  VALUES (
    NEW.id,
    NEW.raw_user_meta_data->>'full_name',
    NEW.raw_user_meta_data->>'avatar_url'
  );
  RETURN NEW;
END;
$$ LANGUAGE plpgsql SECURITY DEFINER;

CREATE TRIGGER on_auth_user_created
  AFTER INSERT ON auth.users
  FOR EACH ROW
  EXECUTE FUNCTION handle_new_user();
