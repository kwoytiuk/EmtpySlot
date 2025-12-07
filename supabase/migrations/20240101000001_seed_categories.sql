-- Seed service categories

-- Personal Care & Beauty
INSERT INTO service_categories (name, slug, icon, description, display_order) VALUES
('Hair Salon', 'hair-salon', '✂️', 'Haircuts, styling, coloring, and treatments', 1),
('Nail Salon', 'nail-salon', '💅', 'Manicures, pedicures, nail art', 2),
('Spa & Massage', 'spa-massage', '💆', 'Massages, facials, body treatments', 3),
('Waxing & Hair Removal', 'waxing', '✨', 'Waxing, threading, laser hair removal', 4),
('Barbershop', 'barbershop', '💈', 'Mens haircuts, shaves, beard trims', 5),
('Makeup & Beauty', 'makeup-beauty', '💄', 'Makeup application, beauty services', 6);

-- Health & Wellness
INSERT INTO service_categories (name, slug, icon, description, display_order) VALUES
('Dental', 'dental', '🦷', 'Dental cleanings, checkups, procedures', 10),
('Chiropractic', 'chiropractic', '🏥', 'Chiropractic adjustments and treatments', 11),
('Physical Therapy', 'physical-therapy', '🏃', 'Physical therapy and rehabilitation', 12),
('Acupuncture', 'acupuncture', '🌿', 'Acupuncture and holistic treatments', 13),
('Mental Health', 'mental-health', '🧠', 'Therapy, counseling, mental health services', 14);

-- Home Services
INSERT INTO service_categories (name, slug, icon, description, display_order) VALUES
('Plumbing', 'plumbing', '🔧', 'Plumbing repairs and installations', 20),
('Electrical', 'electrical', '⚡', 'Electrical repairs and installations', 21),
('HVAC', 'hvac', '❄️', 'Heating, cooling, and ventilation services', 22),
('Handyman', 'handyman', '🔨', 'General home repairs and maintenance', 23),
('Cleaning', 'cleaning', '🧹', 'Home and office cleaning services', 24),
('Pest Control', 'pest-control', '🐛', 'Pest inspection and extermination', 25);

-- Automotive
INSERT INTO service_categories (name, slug, icon, description, display_order) VALUES
('Auto Repair', 'auto-repair', '🔧', 'Auto maintenance and repairs', 30),
('Oil Change', 'oil-change', '🛢️', 'Oil changes and fluid services', 31),
('Car Wash', 'car-wash', '🚗', 'Car washing and detailing', 32),
('Tire Service', 'tire-service', '🚙', 'Tire installation, rotation, repair', 33);

-- Professional Services
INSERT INTO service_categories (name, slug, icon, description, display_order) VALUES
('Legal', 'legal', '⚖️', 'Legal consultations and services', 40),
('Financial', 'financial', '💰', 'Financial planning and tax services', 41),
('Real Estate', 'real-estate', '🏠', 'Real estate consultations', 42),
('Photography', 'photography', '📷', 'Photography services', 43);

-- Pet Services
INSERT INTO service_categories (name, slug, icon, description, display_order) VALUES
('Veterinary', 'veterinary', '🐾', 'Veterinary care and checkups', 50),
('Pet Grooming', 'pet-grooming', '🐕', 'Pet grooming and bathing', 51),
('Dog Training', 'dog-training', '🦮', 'Dog training and behavior services', 52);

-- Fitness & Sports
INSERT INTO service_categories (name, slug, icon, description, display_order) VALUES
('Personal Training', 'personal-training', '💪', 'One-on-one fitness training', 60),
('Yoga', 'yoga', '🧘', 'Yoga classes and instruction', 61),
('Pilates', 'pilates', '🤸', 'Pilates classes and instruction', 62);
