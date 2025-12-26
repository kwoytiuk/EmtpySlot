using EmptySlot.Shared.Models;
using EmptySlot.Shared.Enums;
using Microsoft.EntityFrameworkCore;

namespace EmptySlot.API.Data;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(ApplicationDbContext context)
    {
        // Check if data already exists
        if (await context.ServiceCategories.AnyAsync())
        {
            return; // Database has been seeded
        }

        // Seed Categories
        var hairCategory = new ServiceCategory
        {
            Id = Guid.NewGuid(),
            Name = "Hair Salons",
            Slug = "hair-salons",
            Icon = "✂️",
            DisplayOrder = 1,
            CreatedAt = DateTime.UtcNow
        };

        var spaCategory = new ServiceCategory
        {
            Id = Guid.NewGuid(),
            Name = "Spa & Massage",
            Slug = "spa-massage",
            Icon = "💆",
            DisplayOrder = 2,
            CreatedAt = DateTime.UtcNow
        };

        var nailsCategory = new ServiceCategory
        {
            Id = Guid.NewGuid(),
            Name = "Nail Salons",
            Slug = "nail-salons",
            Icon = "💅",
            DisplayOrder = 3,
            CreatedAt = DateTime.UtcNow
        };

        var fitnessCategory = new ServiceCategory
        {
            Id = Guid.NewGuid(),
            Name = "Fitness & Gym",
            Slug = "fitness-gym",
            Icon = "💪",
            DisplayOrder = 4,
            CreatedAt = DateTime.UtcNow
        };

        context.ServiceCategories.AddRange(hairCategory, spaCategory, nailsCategory, fitnessCategory);

        // Seed Providers
        var providers = new List<Provider>
        {
            new Provider
            {
                Id = Guid.NewGuid(),
                UserId = Guid.NewGuid(), // Temporary user ID
                BusinessName = "Luxury Hair Studio",
                Description = "Premier hair salon with expert stylists specializing in cuts, color, and treatments.",
                Email = "info@luxuryhair.com",
                Phone = "+1-555-0101",
                RatingAverage = 4.8m,
                RatingCount = 156,
                Verified = true,
                IsFeatured = true,
                CreatedAt = DateTime.UtcNow,
                Locations = new List<ProviderLocation>
                {
                    new ProviderLocation
                    {
                        Id = Guid.NewGuid(),
                        Name = "Downtown Location",
                        AddressLine1 = "123 Main Street",
                        City = "Seattle",
                        StateProvince = "WA",
                        PostalCode = "98101",
                        Country = "USA",
                        Latitude = 47.6062,
                        Longitude = -122.3321
                    }
                },
                Services = new List<Service>
                {
                    new Service
                    {
                        Id = Guid.NewGuid(),
                        Name = "Women's Haircut",
                        Description = "Professional haircut with consultation",
                        CategoryId = hairCategory.Id,
                        DurationMinutes = 60,
                        Price = 75.00m,
                        IsActive = true
                    },
                    new Service
                    {
                        Id = Guid.NewGuid(),
                        Name = "Color & Highlights",
                        Description = "Full color service with highlights",
                        CategoryId = hairCategory.Id,
                        DurationMinutes = 120,
                        Price = 150.00m,
                        DepositRequired = 50.00m,
                        IsActive = true
                    }
                },
                StaffMembers = new List<StaffMember>
                {
                    new StaffMember
                    {
                        Id = Guid.NewGuid(),
                        Name = "Sarah Johnson",
                        Bio = "Senior Stylist with 10+ years experience in hair styling",
                        IsActive = true
                    }
                }
            },
            new Provider
            {
                Id = Guid.NewGuid(),
                UserId = Guid.NewGuid(), // Temporary user ID
                BusinessName = "Serenity Spa & Wellness",
                Description = "Full-service spa offering massages, facials, and relaxation treatments.",
                Email = "contact@serenityspa.com",
                Phone = "+1-555-0102",
                RatingAverage = 4.9m,
                RatingCount = 203,
                Verified = true,
                IsFeatured = true,
                CreatedAt = DateTime.UtcNow,
                Locations = new List<ProviderLocation>
                {
                    new ProviderLocation
                    {
                        Id = Guid.NewGuid(),
                        Name = "Main Spa",
                        AddressLine1 = "456 Wellness Avenue",
                        City = "Seattle",
                        StateProvince = "WA",
                        PostalCode = "98102",
                        Country = "USA",
                        Latitude = 47.6205,
                        Longitude = -122.3493
                    }
                },
                Services = new List<Service>
                {
                    new Service
                    {
                        Id = Guid.NewGuid(),
                        Name = "Swedish Massage",
                        Description = "60-minute relaxing massage",
                        CategoryId = spaCategory.Id,
                        DurationMinutes = 60,
                        Price = 90.00m,
                        IsActive = true
                    },
                    new Service
                    {
                        Id = Guid.NewGuid(),
                        Name = "Deep Tissue Massage",
                        Description = "90-minute therapeutic massage",
                        CategoryId = spaCategory.Id,
                        DurationMinutes = 90,
                        Price = 130.00m,
                        IsActive = true
                    }
                },
                StaffMembers = new List<StaffMember>
                {
                    new StaffMember
                    {
                        Id = Guid.NewGuid(),
                        Name = "Michael Chen",
                        Bio = "Licensed Massage Therapist certified in Swedish and deep tissue techniques",
                        IsActive = true
                    }
                }
            },
            new Provider
            {
                Id = Guid.NewGuid(),
                UserId = Guid.NewGuid(), // Temporary user ID
                BusinessName = "Glamour Nails & Beauty",
                Description = "Professional nail salon offering manicures, pedicures, and nail art.",
                Email = "hello@glamournails.com",
                Phone = "+1-555-0103",
                RatingAverage = 4.7m,
                RatingCount = 89,
                Verified = true,
                IsFeatured = false,
                CreatedAt = DateTime.UtcNow,
                Locations = new List<ProviderLocation>
                {
                    new ProviderLocation
                    {
                        Id = Guid.NewGuid(),
                        Name = "Capitol Hill",
                        AddressLine1 = "789 Beauty Lane",
                        City = "Seattle",
                        StateProvince = "WA",
                        PostalCode = "98122",
                        Country = "USA",
                        Latitude = 47.6247,
                        Longitude = -122.3200
                    }
                },
                Services = new List<Service>
                {
                    new Service
                    {
                        Id = Guid.NewGuid(),
                        Name = "Classic Manicure",
                        Description = "Traditional manicure with polish",
                        CategoryId = nailsCategory.Id,
                        DurationMinutes = 30,
                        Price = 35.00m,
                        IsActive = true
                    },
                    new Service
                    {
                        Id = Guid.NewGuid(),
                        Name = "Gel Manicure",
                        Description = "Long-lasting gel polish manicure",
                        CategoryId = nailsCategory.Id,
                        DurationMinutes = 45,
                        Price = 50.00m,
                        IsActive = true
                    }
                }
            },
            new Provider
            {
                Id = Guid.NewGuid(),
                UserId = Guid.NewGuid(), // Temporary user ID
                BusinessName = "PowerFit Gym",
                Description = "Modern fitness center with personal training and group classes.",
                Email = "info@powerfit.com",
                Phone = "+1-555-0104",
                RatingAverage = 4.6m,
                RatingCount = 124,
                Verified = true,
                IsFeatured = false,
                CreatedAt = DateTime.UtcNow,
                Locations = new List<ProviderLocation>
                {
                    new ProviderLocation
                    {
                        Id = Guid.NewGuid(),
                        Name = "Fremont Gym",
                        AddressLine1 = "321 Fitness Drive",
                        City = "Seattle",
                        StateProvince = "WA",
                        PostalCode = "98103",
                        Country = "USA",
                        Latitude = 47.6501,
                        Longitude = -122.3509
                    }
                },
                Services = new List<Service>
                {
                    new Service
                    {
                        Id = Guid.NewGuid(),
                        Name = "Personal Training Session",
                        Description = "One-on-one fitness training",
                        CategoryId = fitnessCategory.Id,
                        DurationMinutes = 60,
                        Price = 80.00m,
                        IsActive = true
                    },
                    new Service
                    {
                        Id = Guid.NewGuid(),
                        Name = "Group Fitness Class",
                        Description = "High-intensity group workout",
                        CategoryId = fitnessCategory.Id,
                        DurationMinutes = 45,
                        Price = 25.00m,
                        IsActive = true
                    }
                }
            }
        };

        context.Providers.AddRange(providers);

        await context.SaveChangesAsync();
    }
}
