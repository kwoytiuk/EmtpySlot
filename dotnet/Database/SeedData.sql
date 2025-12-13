-- Sample data for testing
USE EmptySlotDb;
GO

-- Insert sample categories
INSERT INTO ServiceCategories (Name, Slug, DisplayOrder) VALUES
('Beauty & Wellness', 'beauty-wellness', 1),
('Healthcare', 'healthcare', 2),
('Professional Services', 'professional-services', 3),
('Home Services', 'home-services', 4);

-- Insert subcategories
DECLARE @beautyId UNIQUEIDENTIFIER = (SELECT Id FROM ServiceCategories WHERE Slug = 'beauty-wellness');
INSERT INTO ServiceCategories (Name, Slug, ParentId, DisplayOrder) VALUES
('Hair Salon', 'hair-salon', @beautyId, 1),
('Spa & Massage', 'spa-massage', @beautyId, 2),
('Nail Salon', 'nail-salon', @beautyId, 3);

PRINT 'Sample data seeded successfully!';
GO
