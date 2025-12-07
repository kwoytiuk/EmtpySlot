-- Calgary Test Data Seed
-- This creates realistic test data for Calgary, Alberta

-- First, we need to create test user accounts for providers
-- Note: In production, these would be created through the signup flow
-- For testing, we'll create profiles directly

-- Create test provider users (these IDs are UUIDs you'll need to replace with actual auth user IDs)
-- Or you can sign up manually and then run this script with real user IDs

-- =============================================
-- PROVIDERS
-- =============================================

-- Hair Salons
INSERT INTO providers (id, user_id, business_name, description, phone, email, verified, rating_average, rating_count) VALUES
('550e8400-e29b-41d4-a716-446655440001', 'USER_ID_1', 'Kensington Hair Studio', 'Premier hair salon in the heart of Kensington offering cuts, colors, and styling for over 15 years.', '(403) 555-0101', 'info@kensingtonhair.ca', true, 4.8, 127),
('550e8400-e29b-41d4-a716-446655440002', 'USER_ID_2', 'Inglewood Salon & Spa', 'Full-service salon and spa in historic Inglewood. Specializing in modern cuts and natural products.', '(403) 555-0102', 'hello@inglewoodsalon.ca', true, 4.6, 89),
('550e8400-e29b-41d4-a716-446655440003', 'USER_ID_3', 'Mission Hair Lounge', 'Trendy salon on 4th Street SW. Walk-ins welcome!', '(403) 555-0103', 'contact@missionhair.ca', true, 4.9, 203);

-- Dental Clinics
INSERT INTO providers (id, user_id, business_name, description, phone, email, verified, rating_average, rating_count) VALUES
('550e8400-e29b-41d4-a716-446655440004', 'USER_ID_4', 'Beltline Dental Clinic', 'Modern dental care in downtown Calgary. Accepting new patients and emergencies.', '(403) 555-0104', 'appointments@beltlinedental.ca', true, 4.7, 156),
('550e8400-e29b-41d4-a716-446655440005', 'USER_ID_5', 'Eau Claire Family Dentistry', 'Family-friendly dental practice with evening and weekend hours.', '(403) 555-0105', 'info@eauclairefamilydental.ca', true, 4.8, 142);

-- Nail Salons
INSERT INTO providers (id, user_id, business_name, description, phone, email, verified, rating_average, rating_count) VALUES
('550e8400-e29b-41d4-a716-446655440006', 'USER_ID_6', '17th Avenue Nails & Spa', 'Luxury nail salon on 17th Ave SW. Gel, acrylic, and spa pedicures.', '(403) 555-0106', 'book@17avenails.ca', true, 4.5, 94),
('550e8400-e29b-41d4-a716-446655440007', 'USER_ID_7', 'Marda Loop Nail Bar', 'Quick, professional service in Marda Loop. Same-day appointments available.', '(403) 555-0107', 'hello@mardaloopnails.ca', true, 4.6, 78);

-- Plumbing Services
INSERT INTO providers (id, user_id, business_name, description, phone, email, verified, rating_average, rating_count) VALUES
('550e8400-e29b-41d4-a716-446655440008', 'USER_ID_8', 'Calgary Pro Plumbing', '24/7 emergency plumbing service. Residential and commercial.', '(403) 555-0108', 'service@calgarypro.ca', true, 4.9, 234),
('550e8400-e29b-41d4-a716-446655440009', 'USER_ID_9', 'Bow River Plumbing', 'Licensed and insured plumbers serving Calgary since 1995.', '(403) 555-0109', 'info@bowriverplumbing.ca', true, 4.7, 189);

-- HVAC Services
INSERT INTO providers (id, user_id, business_name, description, phone, email, verified, rating_average, rating_count) VALUES
('550e8400-e29b-41d4-a716-446655440010', 'USER_ID_10', 'Chinook HVAC Services', 'Heating, cooling, and furnace repair. Same-day service available.', '(403) 555-0110', 'service@chinookhvac.ca', true, 4.8, 167),
('550e8400-e29b-41d4-a716-446655440011', 'USER_ID_11', 'Foothills Heating & Cooling', 'Expert HVAC installation and maintenance for Calgary homes.', '(403) 555-0111', 'info@foothillshvac.ca', true, 4.6, 143);

-- Massage Therapy
INSERT INTO providers (id, user_id, business_name, description, phone, email, verified, rating_average, rating_count) VALUES
('550e8400-e29b-41d4-a716-446655440012', 'USER_ID_12', 'Elbow River Massage Therapy', 'Registered massage therapists. Direct billing available.', '(403) 555-0112', 'book@elbowrivermassage.ca', true, 4.9, 178),
('550e8400-e29b-41d4-a716-446655440013', 'USER_ID_13', 'Kensington Wellness Centre', 'Holistic wellness center offering RMT, acupuncture, and physiotherapy.', '(403) 555-0113', 'wellness@kensingtoncentre.ca', true, 4.7, 134);

-- =============================================
-- PROVIDER LOCATIONS (with real Calgary addresses)
-- =============================================

-- Hair Salons
INSERT INTO provider_locations (id, provider_id, name, address_line1, city, state_province, postal_code, country, latitude, longitude, is_primary) VALUES
('650e8400-e29b-41d4-a716-446655440001', '550e8400-e29b-41d4-a716-446655440001', 'Main Location', '1234 Kensington Road NW', 'Calgary', 'AB', 'T2N 3P7', 'Canada', 51.0501, -114.0853, true),
('650e8400-e29b-41d4-a716-446655440002', '550e8400-e29b-41d4-a716-446655440002', 'Main Location', '1301 9th Avenue SE', 'Calgary', 'AB', 'T2G 0T3', 'Canada', 51.0374, -114.0499, true),
('650e8400-e29b-41d4-a716-446655440003', '550e8400-e29b-41d4-a716-446655440003', 'Main Location', '2004 4th Street SW', 'Calgary', 'AB', 'T2S 1W6', 'Canada', 51.0371, -114.0754, true);

-- Dental Clinics
INSERT INTO provider_locations (id, provider_id, name, address_line1, city, state_province, postal_code, country, latitude, longitude, is_primary) VALUES
('650e8400-e29b-41d4-a716-446655440004', '550e8400-e29b-41d4-a716-446655440004', 'Main Location', '888 3rd Street SW', 'Calgary', 'AB', 'T2P 5C5', 'Canada', 51.0486, -114.0708, true),
('650e8400-e29b-41d4-a716-446655440005', '550e8400-e29b-41d4-a716-446655440005', 'Main Location', '200 Barclay Parade SW', 'Calgary', 'AB', 'T2P 4R5', 'Canada', 51.0530, -114.0740, true);

-- Nail Salons
INSERT INTO provider_locations (id, provider_id, name, address_line1, city, state_province, postal_code, country, latitude, longitude, is_primary) VALUES
('650e8400-e29b-41d4-a716-446655440006', '550e8400-e29b-41d4-a716-446655440006', 'Main Location', '1515 17th Avenue SW', 'Calgary', 'AB', 'T2T 0C9', 'Canada', 51.0378, -114.0935, true),
('650e8400-e29b-41d4-a716-446655440007', '550e8400-e29b-41d4-a716-446655440007', 'Main Location', '3506 33rd Street SW', 'Calgary', 'AB', 'T3E 2Y3', 'Canada', 51.0213, -114.1257, true);

-- Plumbing Services
INSERT INTO provider_locations (id, provider_id, name, address_line1, city, state_province, postal_code, country, latitude, longitude, is_primary) VALUES
('650e8400-e29b-41d4-a716-446655440008', '550e8400-e29b-41d4-a716-446655440008', 'Main Office', '5678 Macleod Trail SE', 'Calgary', 'AB', 'T2H 0K5', 'Canada', 50.9897, -114.0625, true),
('650e8400-e29b-41d4-a716-446655440009', '550e8400-e29b-41d4-a716-446655440009', 'Main Office', '7234 16th Avenue NW', 'Calgary', 'AB', 'T3B 0M8', 'Canada', 51.0751, -114.1632, true);

-- HVAC Services
INSERT INTO provider_locations (id, provider_id, name, address_line1, city, state_province, postal_code, country, latitude, longitude, is_primary) VALUES
('650e8400-e29b-41d4-a716-446655440010', '550e8400-e29b-41d4-a716-446655440010', 'Main Office', '9876 Blackfoot Trail SE', 'Calgary', 'AB', 'T2J 3J1', 'Canada', 50.9542, -114.0421, true),
('650e8400-e29b-41d4-a716-446655440011', '550e8400-e29b-41d4-a716-446655440011', 'Main Office', '1234 52nd Street NE', 'Calgary', 'AB', 'T3J 0J7', 'Canada', 51.0889, -113.9542, true);

-- Massage Therapy
INSERT INTO provider_locations (id, provider_id, name, address_line1, city, state_province, postal_code, country, latitude, longitude, is_primary) VALUES
('650e8400-e29b-41d4-a716-446655440012', '550e8400-e29b-41d4-a716-446655440012', 'Main Location', '4567 Elbow Drive SW', 'Calgary', 'AB', 'T2S 2K2', 'Canada', 51.0168, -114.0795, true),
('650e8400-e29b-41d4-a716-446655440013', '550e8400-e29b-41d4-a716-446655440013', 'Main Location', '1420 Kensington Road NW', 'Calgary', 'AB', 'T2N 3P9', 'Canada', 51.0503, -114.0865, true);

-- =============================================
-- SERVICES
-- =============================================

-- Kensington Hair Studio Services
INSERT INTO services (id, provider_id, category_id, name, description, duration_minutes, price, is_active) VALUES
('750e8400-e29b-41d4-a716-446655440001', '550e8400-e29b-41d4-a716-446655440001', (SELECT id FROM service_categories WHERE slug = 'hair-salon'), 'Women''s Haircut', 'Precision cut with styling. Includes wash and blow dry.', 60, 65.00, true),
('750e8400-e29b-41d4-a716-446655440002', '550e8400-e29b-41d4-a716-446655440001', (SELECT id FROM service_categories WHERE slug = 'hair-salon'), 'Men''s Haircut', 'Classic or modern cut with hot towel treatment.', 45, 45.00, true),
('750e8400-e29b-41d4-a716-446655440003', '550e8400-e29b-41d4-a716-446655440001', (SELECT id FROM service_categories WHERE slug = 'hair-salon'), 'Full Color', 'Complete color transformation with toning.', 150, 150.00, true),
('750e8400-e29b-41d4-a716-446655440004', '550e8400-e29b-41d4-a716-446655440001', (SELECT id FROM service_categories WHERE slug = 'hair-salon'), 'Highlights', 'Partial or full highlights with gloss.', 120, 120.00, true);

-- Inglewood Salon Services
INSERT INTO services (id, provider_id, category_id, name, description, duration_minutes, price, is_active) VALUES
('750e8400-e29b-41d4-a716-446655440005', '550e8400-e29b-41d4-a716-446655440002', (SELECT id FROM service_categories WHERE slug = 'hair-salon'), 'Haircut & Style', 'Cut and style with organic products.', 60, 70.00, true),
('750e8400-e29b-41d4-a716-446655440006', '550e8400-e29b-41d4-a716-446655440002', (SELECT id FROM service_categories WHERE slug = 'hair-salon'), 'Keratin Treatment', 'Smoothing treatment for frizz-free hair.', 180, 200.00, true);

-- Mission Hair Lounge Services
INSERT INTO services (id, provider_id, category_id, name, description, duration_minutes, price, is_active) VALUES
('750e8400-e29b-41d4-a716-446655440007', '550e8400-e29b-41d4-a716-446655440003', (SELECT id FROM service_categories WHERE slug = 'hair-salon'), 'Express Cut', 'Quick trim, perfect for maintaining your style.', 30, 35.00, true),
('750e8400-e29b-41d4-a716-446655440008', '550e8400-e29b-41d4-a716-446655440003', (SELECT id FROM service_categories WHERE slug = 'hair-salon'), 'Balayage', 'Hand-painted highlights for natural dimension.', 150, 180.00, true);

-- Beltline Dental Services
INSERT INTO services (id, provider_id, category_id, name, description, duration_minutes, price, is_active) VALUES
('750e8400-e29b-41d4-a716-446655440009', '550e8400-e29b-41d4-a716-446655440004', (SELECT id FROM service_categories WHERE slug = 'dental'), 'Cleaning & Check-up', 'Comprehensive cleaning and examination.', 60, 180.00, true),
('750e8400-e29b-41d4-a716-446655440010', '550e8400-e29b-41d4-a716-446655440004', (SELECT id FROM service_categories WHERE slug = 'dental'), 'Teeth Whitening', 'Professional whitening treatment.', 90, 450.00, true),
('750e8400-e29b-41d4-a716-446655440011', '550e8400-e29b-41d4-a716-446655440004', (SELECT id FROM service_categories WHERE slug = 'dental'), 'Emergency Visit', 'Same-day emergency dental care.', 45, 150.00, true);

-- Eau Claire Dental Services
INSERT INTO services (id, provider_id, category_id, name, description, duration_minutes, price, is_active) VALUES
('750e8400-e29b-41d4-a716-446655440012', '550e8400-e29b-41d4-a716-446655440005', (SELECT id FROM service_categories WHERE slug = 'dental'), 'Family Cleaning', 'Cleaning for all ages. Kids welcome!', 60, 165.00, true),
('750e8400-e29b-41d4-a716-446655440013', '550e8400-e29b-41d4-a716-446655440005', (SELECT id FROM service_categories WHERE slug = 'dental'), 'Filling', 'Composite or amalgam filling.', 60, 220.00, true);

-- 17th Avenue Nails Services
INSERT INTO services (id, provider_id, category_id, name, description, duration_minutes, price, is_active) VALUES
('750e8400-e29b-41d4-a716-446655440014', '550e8400-e29b-41d4-a716-446655440006', (SELECT id FROM service_categories WHERE slug = 'nail-salon'), 'Gel Manicure', 'Long-lasting gel polish with nail art options.', 45, 45.00, true),
('750e8400-e29b-41d4-a716-446655440015', '550e8400-e29b-41d4-a716-446655440006', (SELECT id FROM service_categories WHERE slug = 'nail-salon'), 'Spa Pedicure', 'Luxury pedicure with massage and paraffin wax.', 75, 65.00, true),
('750e8400-e29b-41d4-a716-446655440016', '550e8400-e29b-41d4-a716-446655440006', (SELECT id FROM service_categories WHERE slug = 'nail-salon'), 'Acrylic Full Set', 'Full acrylic nails with your choice of design.', 90, 80.00, true);

-- Marda Loop Nails Services
INSERT INTO services (id, provider_id, category_id, name, description, duration_minutes, price, is_active) VALUES
('750e8400-e29b-41d4-a716-446655440017', '550e8400-e29b-41d4-a716-446655440007', (SELECT id FROM service_categories WHERE slug = 'nail-salon'), 'Express Manicure', 'Quick polish change, perfect for busy schedules.', 30, 30.00, true),
('750e8400-e29b-41d4-a716-446655440018', '550e8400-e29b-41d4-a716-446655440007', (SELECT id FROM service_categories WHERE slug = 'nail-salon'), 'Classic Pedicure', 'Essential pedicure with polish.', 45, 50.00, true);

-- Calgary Pro Plumbing Services
INSERT INTO services (id, provider_id, category_id, name, description, duration_minutes, price, is_active) VALUES
('750e8400-e29b-41d4-a716-446655440019', '550e8400-e29b-41d4-a716-446655440008', (SELECT id FROM service_categories WHERE slug = 'plumbing'), 'Drain Cleaning', 'Professional drain cleaning and camera inspection.', 60, 150.00, true),
('750e8400-e29b-41d4-a716-446655440020', '550e8400-e29b-41d4-a716-446655440008', (SELECT id FROM service_categories WHERE slug = 'plumbing'), 'Water Heater Repair', 'Diagnose and repair water heater issues.', 90, 200.00, true),
('750e8400-e29b-41d4-a716-446655440021', '550e8400-e29b-41d4-a716-446655440008', (SELECT id FROM service_categories WHERE slug = 'plumbing'), 'Emergency Plumbing', '24/7 emergency service for urgent issues.', 60, 250.00, true);

-- Bow River Plumbing Services
INSERT INTO services (id, provider_id, category_id, name, description, duration_minutes, price, is_active) VALUES
('750e8400-e29b-41d4-a716-446655440022', '550e8400-e29b-41d4-a716-446655440009', (SELECT id FROM service_categories WHERE slug = 'plumbing'), 'Leak Repair', 'Fix leaking pipes, faucets, and toilets.', 60, 140.00, true),
('750e8400-e29b-41d4-a716-446655440023', '550e8400-e29b-41d4-a716-446655440009', (SELECT id FROM service_categories WHERE slug = 'plumbing'), 'Fixture Installation', 'Install sinks, toilets, faucets, and showers.', 120, 300.00, true);

-- Chinook HVAC Services
INSERT INTO services (id, provider_id, category_id, name, description, duration_minutes, price, is_active) VALUES
('750e8400-e29b-41d4-a716-446655440024', '550e8400-e29b-41d4-a716-446655440010', (SELECT id FROM service_categories WHERE slug = 'hvac'), 'Furnace Tune-Up', 'Annual furnace maintenance and safety check.', 90, 150.00, true),
('750e8400-e29b-41d4-a716-446655440025', '550e8400-e29b-41d4-a716-446655440010', (SELECT id FROM service_categories WHERE slug = 'hvac'), 'AC Repair', 'Diagnose and repair air conditioning issues.', 120, 180.00, true),
('750e8400-e29b-41d4-a716-446655440026', '550e8400-e29b-41d4-a716-446655440010', (SELECT id FROM service_categories WHERE slug = 'hvac'), 'Emergency HVAC Service', 'Same-day service for heating/cooling emergencies.', 90, 250.00, true);

-- Foothills HVAC Services
INSERT INTO services (id, provider_id, category_id, name, description, duration_minutes, price, is_active) VALUES
('750e8400-e29b-41d4-a716-446655440027', '550e8400-e29b-41d4-a716-446655440011', (SELECT id FROM service_categories WHERE slug = 'hvac'), 'Duct Cleaning', 'Professional air duct cleaning service.', 180, 300.00, true),
('750e8400-e29b-41d4-a716-446655440028', '550e8400-e29b-41d4-a716-446655440011', (SELECT id FROM service_categories WHERE slug = 'hvac'), 'Thermostat Installation', 'Install and program smart thermostats.', 60, 120.00, true);

-- Elbow River Massage Services
INSERT INTO services (id, provider_id, category_id, name, description, duration_minutes, price, is_active) VALUES
('750e8400-e29b-41d4-a716-446655440029', '550e8400-e29b-41d4-a716-446655440012', (SELECT id FROM service_categories WHERE slug = 'spa-massage'), '60-Minute Massage', 'Therapeutic massage for relaxation and pain relief.', 60, 110.00, true),
('750e8400-e29b-41d4-a716-446655440030', '550e8400-e29b-41d4-a716-446655440012', (SELECT id FROM service_categories WHERE slug = 'spa-massage'), '90-Minute Deep Tissue', 'Deep tissue massage for chronic tension.', 90, 145.00, true),
('750e8400-e29b-41d4-a716-446655440031', '550e8400-e29b-41d4-a716-446655440012', (SELECT id FROM service_categories WHERE slug = 'spa-massage'), 'Hot Stone Massage', 'Relaxing hot stone therapy.', 75, 135.00, true);

-- Kensington Wellness Services
INSERT INTO services (id, provider_id, category_id, name, description, duration_minutes, price, is_active) VALUES
('750e8400-e29b-41d4-a716-446655440032', '550e8400-e29b-41d4-a716-446655440013', (SELECT id FROM service_categories WHERE slug = 'spa-massage'), 'Swedish Massage', 'Classic relaxation massage.', 60, 105.00, true),
('750e8400-e29b-41d4-a716-446655440033', '550e8400-e29b-41d4-a716-446655440013', (SELECT id FROM service_categories WHERE slug = 'acupuncture'), 'Acupuncture Treatment', 'Traditional Chinese acupuncture.', 60, 95.00, true);

-- =============================================
-- SAMPLE REVIEWS
-- =============================================

-- Note: These will need actual appointment IDs and customer IDs
-- For now, this is a template showing the structure
/*
INSERT INTO reviews (appointment_id, customer_id, provider_id, rating, comment, is_verified) VALUES
('appointment-id-here', 'customer-id-here', '550e8400-e29b-41d4-a716-446655440001', 5, 'Amazing haircut! Sarah really knows her craft. Will definitely be back!', true),
('appointment-id-here', 'customer-id-here', '550e8400-e29b-41d4-a716-446655440001', 4, 'Great service and atmosphere. Only critique is the wait time, but worth it!', true);
*/

-- =============================================
-- NOTES
-- =============================================
-- To use this seed data:
-- 1. First create provider accounts through the signup flow
-- 2. Replace USER_ID_1 through USER_ID_13 with the actual auth.users IDs
-- 3. Run this SQL in your Supabase SQL Editor
-- 4. The location geography fields will auto-populate via the trigger
