# EmptySlot - Database Schema Design

## Core Tables

### 1. profiles (extends Supabase auth.users)
```sql
- id (uuid, PK, references auth.users)
- user_type (enum: 'customer', 'provider', 'admin')
- full_name (text)
- phone (text)
- avatar_url (text)
- created_at (timestamp)
- updated_at (timestamp)
```

### 2. service_categories
```sql
- id (uuid, PK)
- name (text) -- e.g., "Hair Salon", "Dental", "Plumbing"
- slug (text, unique)
- icon (text) -- icon name or URL
- description (text)
- parent_id (uuid, nullable) -- for subcategories
- display_order (int)
- created_at (timestamp)
```

### 3. providers
```sql
- id (uuid, PK)
- user_id (uuid, references profiles.id)
- business_name (text)
- description (text)
- phone (text)
- email (text)
- website (text, nullable)
- logo_url (text, nullable)
- cover_image_url (text, nullable)
- verified (boolean, default: false)
- rating_average (decimal, default: 0)
- rating_count (int, default: 0)
- cancellation_policy (text)
- created_at (timestamp)
- updated_at (timestamp)
```

### 4. provider_locations
```sql
- id (uuid, PK)
- provider_id (uuid, references providers.id)
- name (text) -- e.g., "Downtown Location"
- address_line1 (text)
- address_line2 (text, nullable)
- city (text)
- state_province (text)
- postal_code (text)
- country (text)
- latitude (decimal)
- longitude (decimal)
- location (geography(Point,4326)) -- PostGIS for spatial queries
- phone (text, nullable)
- is_primary (boolean, default: false)
- created_at (timestamp)
- updated_at (timestamp)
```

**Spatial Index:**
```sql
CREATE INDEX idx_provider_locations_location ON provider_locations USING GIST (location);
```

### 5. services
```sql
- id (uuid, PK)
- provider_id (uuid, references providers.id)
- category_id (uuid, references service_categories.id)
- name (text)
- description (text)
- duration_minutes (int)
- price (decimal)
- deposit_required (decimal, nullable)
- image_url (text, nullable)
- is_active (boolean, default: true)
- created_at (timestamp)
- updated_at (timestamp)
```

### 6. staff_members
```sql
- id (uuid, PK)
- provider_id (uuid, references providers.id)
- name (text)
- email (text, nullable)
- phone (text, nullable)
- avatar_url (text, nullable)
- bio (text, nullable)
- is_active (boolean, default: true)
- created_at (timestamp)
- updated_at (timestamp)
```

### 7. staff_services (junction table)
```sql
- staff_id (uuid, references staff_members.id)
- service_id (uuid, references services.id)
- PRIMARY KEY (staff_id, service_id)
```

### 8. availability_schedules
```sql
- id (uuid, PK)
- provider_id (uuid, references providers.id)
- staff_id (uuid, references staff_members.id, nullable)
- location_id (uuid, references provider_locations.id)
- day_of_week (int) -- 0=Sunday, 6=Saturday
- start_time (time)
- end_time (time)
- is_active (boolean, default: true)
- created_at (timestamp)
```

### 9. availability_exceptions
```sql
- id (uuid, PK)
- provider_id (uuid, references providers.id)
- staff_id (uuid, references staff_members.id, nullable)
- location_id (uuid, references provider_locations.id)
- date (date)
- start_time (time, nullable) -- null means closed all day
- end_time (time, nullable)
- reason (text, nullable)
- created_at (timestamp)
```

### 10. appointments
```sql
- id (uuid, PK)
- booking_reference (text, unique) -- e.g., "ES-ABC123"
- customer_id (uuid, references profiles.id)
- provider_id (uuid, references providers.id)
- location_id (uuid, references provider_locations.id)
- service_id (uuid, references services.id)
- staff_id (uuid, references staff_members.id, nullable)
- appointment_date (date)
- start_time (time)
- end_time (time)
- status (enum: 'pending', 'confirmed', 'completed', 'cancelled', 'no_show')
- price (decimal)
- deposit_paid (decimal, nullable)
- payment_status (enum: 'pending', 'paid', 'refunded')
- payment_intent_id (text, nullable) -- Stripe payment intent
- customer_notes (text, nullable)
- provider_notes (text, nullable)
- cancelled_by (uuid, nullable, references profiles.id)
- cancelled_at (timestamp, nullable)
- cancellation_reason (text, nullable)
- created_at (timestamp)
- updated_at (timestamp)
```

### 11. promotions
```sql
- id (uuid, PK)
- provider_id (uuid, references providers.id)
- service_id (uuid, references services.id, nullable) -- null means all services
- title (text)
- description (text)
- discount_type (enum: 'percentage', 'fixed_amount')
- discount_value (decimal)
- valid_from (timestamp)
- valid_until (timestamp)
- max_uses (int, nullable)
- current_uses (int, default: 0)
- min_notice_hours (int) -- e.g., 2 for "within 2 hours"
- is_active (boolean, default: true)
- created_at (timestamp)
- updated_at (timestamp)
```

### 12. reviews
```sql
- id (uuid, PK)
- appointment_id (uuid, references appointments.id, unique)
- customer_id (uuid, references profiles.id)
- provider_id (uuid, references providers.id)
- rating (int) -- 1-5 stars
- comment (text, nullable)
- response (text, nullable) -- provider response
- response_at (timestamp, nullable)
- is_verified (boolean, default: false) -- verified booking
- created_at (timestamp)
- updated_at (timestamp)
```

### 13. favorites
```sql
- id (uuid, PK)
- customer_id (uuid, references profiles.id)
- provider_id (uuid, references providers.id)
- created_at (timestamp)
- UNIQUE(customer_id, provider_id)
```

### 14. notifications
```sql
- id (uuid, PK)
- user_id (uuid, references profiles.id)
- type (enum: 'booking_confirmed', 'booking_reminder', 'booking_cancelled', 'review_request', 'promotion')
- title (text)
- message (text)
- data (jsonb) -- additional data
- read (boolean, default: false)
- created_at (timestamp)
```

## Indexes for Performance

```sql
-- Geospatial search
CREATE INDEX idx_provider_locations_location ON provider_locations USING GIST (location);

-- Common queries
CREATE INDEX idx_appointments_customer_id ON appointments(customer_id);
CREATE INDEX idx_appointments_provider_id ON appointments(provider_id);
CREATE INDEX idx_appointments_date_status ON appointments(appointment_date, status);
CREATE INDEX idx_appointments_booking_ref ON appointments(booking_reference);

CREATE INDEX idx_services_provider_id ON services(provider_id);
CREATE INDEX idx_services_category_id ON services(category_id);

CREATE INDEX idx_reviews_provider_id ON reviews(provider_id);
CREATE INDEX idx_providers_verified ON providers(verified);

CREATE INDEX idx_promotions_active_dates ON promotions(is_active, valid_from, valid_until);
```

## Row Level Security (RLS) Policies

### Customers can:
- Read all public provider data
- Create/read/update their own appointments
- Create reviews for their completed appointments
- Read/create/delete their own favorites

### Providers can:
- Read/update their own provider profile
- Create/read/update/delete their own services
- Read/update appointments for their business
- Respond to reviews

### Security Examples:
```sql
-- Appointments: Customers can only see their own
CREATE POLICY "Customers can view own appointments"
ON appointments FOR SELECT
TO authenticated
USING (auth.uid() = customer_id);

-- Appointments: Providers can see their bookings
CREATE POLICY "Providers can view their appointments"
ON appointments FOR SELECT
TO authenticated
USING (
  provider_id IN (
    SELECT id FROM providers WHERE user_id = auth.uid()
  )
);
```

## Views for Common Queries

### available_time_slots
Combines schedules, exceptions, and existing bookings to show available slots

### provider_search_view
Denormalized view for fast search with location, ratings, categories

### upcoming_appointments_view
Pre-joined view of appointments with all related data
