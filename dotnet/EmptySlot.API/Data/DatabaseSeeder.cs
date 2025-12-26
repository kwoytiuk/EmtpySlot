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

        // Seed Providers with Alberta, Canada locations
        var providers = new List<Provider>
        {
            new Provider
            {
                Id = Guid.NewGuid(),
                UserId = Guid.NewGuid(), // Temporary user ID
                BusinessName = "Luxe Hair Studio Calgary",
                Description = "Award-winning hair salon in the heart of downtown Calgary. Specializing in precision cuts, color correction, balayage, and luxury treatments. Our master stylists stay current with international trends.",
                Email = "hello@luxehaircalgary.ca",
                Phone = "+1 (403) 555-0101",
                Website = "https://luxehaircalgary.ca",
                RatingAverage = 4.8m,
                RatingCount = 247,
                Verified = true,
                IsFeatured = true,
                CreatedAt = DateTime.UtcNow,
                Locations = new List<ProviderLocation>
                {
                    new ProviderLocation
                    {
                        Id = Guid.NewGuid(),
                        Name = "Downtown Calgary",
                        AddressLine1 = "850 2nd Street SW",
                        AddressLine2 = "Suite 240",
                        City = "Calgary",
                        StateProvince = "AB",
                        PostalCode = "T2P 0R8",
                        Country = "Canada",
                        Phone = "+1 (403) 555-0101",
                        Latitude = 51.0447,
                        Longitude = -114.0719,
                        IsPrimary = true
                    }
                },
                Services = new List<Service>
                {
                    new Service
                    {
                        Id = Guid.NewGuid(),
                        Name = "Women's Precision Cut",
                        Description = "Consultation, shampoo, precision cut, blow-dry and style. Includes complimentary scalp massage.",
                        CategoryId = hairCategory.Id,
                        DurationMinutes = 60,
                        Price = 85.00m,
                        IsActive = true
                    },
                    new Service
                    {
                        Id = Guid.NewGuid(),
                        Name = "Men's Executive Cut",
                        Description = "Hot towel treatment, precision cut, style and grooming consultation.",
                        CategoryId = hairCategory.Id,
                        DurationMinutes = 45,
                        Price = 55.00m,
                        IsActive = true
                    },
                    new Service
                    {
                        Id = Guid.NewGuid(),
                        Name = "Balayage & Highlights",
                        Description = "Full balayage or highlight service with toner, Olaplex treatment included.",
                        CategoryId = hairCategory.Id,
                        DurationMinutes = 180,
                        Price = 245.00m,
                        DepositRequired = 75.00m,
                        IsActive = true
                    },
                    new Service
                    {
                        Id = Guid.NewGuid(),
                        Name = "Keratin Treatment",
                        Description = "Professional smoothing treatment for frizz-free, manageable hair lasting 3-5 months.",
                        CategoryId = hairCategory.Id,
                        DurationMinutes = 150,
                        Price = 295.00m,
                        DepositRequired = 100.00m,
                        IsActive = true
                    }
                },
                StaffMembers = new List<StaffMember>
                {
                    new StaffMember
                    {
                        Id = Guid.NewGuid(),
                        Name = "Sarah Thompson",
                        Email = "sarah@luxehaircalgary.ca",
                        Bio = "Master Stylist & Color Specialist. Redken certified, 12+ years experience. Specializes in balayage and color correction.",
                        IsActive = true
                    },
                    new StaffMember
                    {
                        Id = Guid.NewGuid(),
                        Name = "Marcus Chen",
                        Email = "marcus@luxehaircalgary.ca",
                        Bio = "Senior Stylist. Vidal Sassoon trained, expert in precision cutting and men's grooming.",
                        IsActive = true
                    }
                }
            },
            new Provider
            {
                Id = Guid.NewGuid(),
                UserId = Guid.NewGuid(), // Temporary user ID
                BusinessName = "Serenity Spa & Wellness Edmonton",
                Description = "Luxury spa retreat in Edmonton offering therapeutic massages, advanced skincare, and holistic wellness treatments. Escape the everyday and restore your mind, body, and spirit.",
                Email = "book@serenityspaedmonton.ca",
                Phone = "+1 (780) 555-0202",
                Website = "https://serenityspaedmonton.ca",
                RatingAverage = 4.9m,
                RatingCount = 318,
                Verified = true,
                IsFeatured = true,
                CreatedAt = DateTime.UtcNow,
                Locations = new List<ProviderLocation>
                {
                    new ProviderLocation
                    {
                        Id = Guid.NewGuid(),
                        Name = "Whyte Avenue Wellness Centre",
                        AddressLine1 = "8212 104th Street NW",
                        City = "Edmonton",
                        StateProvince = "AB",
                        PostalCode = "T6E 4E6",
                        Country = "Canada",
                        Phone = "+1 (780) 555-0202",
                        Latitude = 53.5461,
                        Longitude = -113.4938,
                        IsPrimary = true
                    }
                },
                Services = new List<Service>
                {
                    new Service
                    {
                        Id = Guid.NewGuid(),
                        Name = "Relaxation Massage (60 min)",
                        Description = "Classic Swedish massage to promote relaxation, improve circulation, and reduce tension. Perfect for stress relief.",
                        CategoryId = spaCategory.Id,
                        DurationMinutes = 60,
                        Price = 120.00m,
                        IsActive = true
                    },
                    new Service
                    {
                        Id = Guid.NewGuid(),
                        Name = "Deep Tissue Therapy (90 min)",
                        Description = "Intensive massage targeting chronic muscle tension and knots. Ideal for athletes and those with persistent pain.",
                        CategoryId = spaCategory.Id,
                        DurationMinutes = 90,
                        Price = 165.00m,
                        IsActive = true
                    },
                    new Service
                    {
                        Id = Guid.NewGuid(),
                        Name = "Hot Stone Massage",
                        Description = "Heated basalt stones combined with therapeutic massage to melt away tension and promote deep relaxation.",
                        CategoryId = spaCategory.Id,
                        DurationMinutes = 75,
                        Price = 145.00m,
                        IsActive = true
                    },
                    new Service
                    {
                        Id = Guid.NewGuid(),
                        Name = "Signature Facial Treatment",
                        Description = "Customized facial including deep cleansing, exfoliation, extraction, massage, and hydrating mask. Tailored to your skin type.",
                        CategoryId = spaCategory.Id,
                        DurationMinutes = 60,
                        Price = 135.00m,
                        IsActive = true
                    },
                    new Service
                    {
                        Id = Guid.NewGuid(),
                        Name = "Couples Spa Package",
                        Description = "Side-by-side massage experience for two, followed by champagne and chocolate. Perfect for anniversaries or special occasions.",
                        CategoryId = spaCategory.Id,
                        DurationMinutes = 120,
                        Price = 399.00m,
                        DepositRequired = 150.00m,
                        IsActive = true
                    }
                },
                StaffMembers = new List<StaffMember>
                {
                    new StaffMember
                    {
                        Id = Guid.NewGuid(),
                        Name = "Alicia Morrison",
                        Email = "alicia@serenityspaedmonton.ca",
                        Bio = "RMT - Registered Massage Therapist with 8 years experience. Specializes in therapeutic and prenatal massage.",
                        IsActive = true
                    },
                    new StaffMember
                    {
                        Id = Guid.NewGuid(),
                        Name = "David Park",
                        Email = "david@serenityspaedmonton.ca",
                        Bio = "RMT & Sports Therapy Specialist. Works with professional athletes and chronic pain management.",
                        IsActive = true
                    },
                    new StaffMember
                    {
                        Id = Guid.NewGuid(),
                        Name = "Emma Laurent",
                        Email = "emma@serenityspaedmonton.ca",
                        Bio = "Licensed Esthetician. CIDESCO certified with expertise in advanced skincare and anti-aging treatments.",
                        IsActive = true
                    }
                }
            },
            new Provider
            {
                Id = Guid.NewGuid(),
                UserId = Guid.NewGuid(), // Temporary user ID
                BusinessName = "Polished Nail Studio Calgary",
                Description = "Premier nail salon and beauty bar specializing in luxury manicures, pedicures, and nail art. We use only premium, vegan, and cruelty-free products in a modern, relaxing atmosphere.",
                Email = "hello@polishednailstudio.ca",
                Phone = "+1 (403) 555-0303",
                Website = "https://polishednailstudio.ca",
                RatingAverage = 4.7m,
                RatingCount = 189,
                Verified = true,
                IsFeatured = false,
                CreatedAt = DateTime.UtcNow,
                Locations = new List<ProviderLocation>
                {
                    new ProviderLocation
                    {
                        Id = Guid.NewGuid(),
                        Name = "Kensington Village",
                        AddressLine1 = "1132 Kensington Road NW",
                        City = "Calgary",
                        StateProvince = "AB",
                        PostalCode = "T2N 3P4",
                        Country = "Canada",
                        Phone = "+1 (403) 555-0303",
                        Latitude = 51.0527,
                        Longitude = -114.0857,
                        IsPrimary = true
                    }
                },
                Services = new List<Service>
                {
                    new Service
                    {
                        Id = Guid.NewGuid(),
                        Name = "Signature Manicure",
                        Description = "Nail shaping, cuticle care, hand massage with luxe lotion, and polish of your choice. Includes paraffin wax treatment.",
                        CategoryId = nailsCategory.Id,
                        DurationMinutes = 45,
                        Price = 45.00m,
                        IsActive = true
                    },
                    new Service
                    {
                        Id = Guid.NewGuid(),
                        Name = "Gel Polish Manicure",
                        Description = "Long-lasting gel polish that lasts up to 3 weeks. Includes nail shaping, cuticle care, and hand massage.",
                        CategoryId = nailsCategory.Id,
                        DurationMinutes = 60,
                        Price = 65.00m,
                        IsActive = true
                    },
                    new Service
                    {
                        Id = Guid.NewGuid(),
                        Name = "Deluxe Spa Pedicure",
                        Description = "Ultimate foot pampering with exfoliation scrub, callus treatment, hot stone massage, paraffin mask, and polish. Bliss for tired feet!",
                        CategoryId = nailsCategory.Id,
                        DurationMinutes = 75,
                        Price = 85.00m,
                        IsActive = true
                    },
                    new Service
                    {
                        Id = Guid.NewGuid(),
                        Name = "Custom Nail Art",
                        Description = "Hand-painted designs, embellishments, or intricate patterns. Price per nail, book with any manicure service.",
                        CategoryId = nailsCategory.Id,
                        DurationMinutes = 30,
                        Price = 15.00m,
                        IsActive = true
                    }
                },
                StaffMembers = new List<StaffMember>
                {
                    new StaffMember
                    {
                        Id = Guid.NewGuid(),
                        Name = "Jessica Park",
                        Email = "jessica@polishednailstudio.ca",
                        Bio = "Licensed Nail Technician & Nail Artist. 7 years experience, specializes in gel extensions and intricate nail art.",
                        IsActive = true
                    }
                }
            },
            new Provider
            {
                Id = Guid.NewGuid(),
                UserId = Guid.NewGuid(), // Temporary user ID
                BusinessName = "Elevation Fitness & Performance",
                Description = "State-of-the-art fitness facility offering personal training, group classes, and athletic performance programs. Equipped with the latest technology and staffed by certified professionals dedicated to helping you reach your peak.",
                Email = "train@elevationfitness.ca",
                Phone = "+1 (403) 555-0404",
                Website = "https://elevationfitness.ca",
                RatingAverage = 4.8m,
                RatingCount = 276,
                Verified = true,
                IsFeatured = false,
                CreatedAt = DateTime.UtcNow,
                Locations = new List<ProviderLocation>
                {
                    new ProviderLocation
                    {
                        Id = Guid.NewGuid(),
                        Name = "Beltline Performance Centre",
                        AddressLine1 = "1025 9th Avenue SE",
                        City = "Calgary",
                        StateProvince = "AB",
                        PostalCode = "T2G 0S6",
                        Country = "Canada",
                        Phone = "+1 (403) 555-0404",
                        Latitude = 51.0366,
                        Longitude = -114.0503,
                        IsPrimary = true
                    }
                },
                Services = new List<Service>
                {
                    new Service
                    {
                        Id = Guid.NewGuid(),
                        Name = "Personal Training (1 Hour)",
                        Description = "One-on-one coaching with certified personal trainer. Customized workout plan, form correction, and progress tracking. Nutritional guidance included.",
                        CategoryId = fitnessCategory.Id,
                        DurationMinutes = 60,
                        Price = 95.00m,
                        IsActive = true
                    },
                    new Service
                    {
                        Id = Guid.NewGuid(),
                        Name = "HIIT Group Class",
                        Description = "High-Intensity Interval Training in a motivating group environment. Burn calories, build strength, and improve cardio. All fitness levels welcome.",
                        CategoryId = fitnessCategory.Id,
                        DurationMinutes = 45,
                        Price = 28.00m,
                        IsActive = true
                    },
                    new Service
                    {
                        Id = Guid.NewGuid(),
                        Name = "Athletic Performance Assessment",
                        Description = "Comprehensive fitness evaluation including strength testing, mobility screening, and personalized performance plan. Essential for serious athletes.",
                        CategoryId = fitnessCategory.Id,
                        DurationMinutes = 90,
                        Price = 175.00m,
                        IsActive = true
                    },
                    new Service
                    {
                        Id = Guid.NewGuid(),
                        Name = "Yoga & Flexibility Class",
                        Description = "Mindful movement combining strength, balance, and flexibility. Perfect for recovery days or stress management.",
                        CategoryId = fitnessCategory.Id,
                        DurationMinutes = 60,
                        Price = 25.00m,
                        IsActive = true
                    }
                },
                StaffMembers = new List<StaffMember>
                {
                    new StaffMember
                    {
                        Id = Guid.NewGuid(),
                        Name = "Ryan Mitchell",
                        Email = "ryan@elevationfitness.ca",
                        Bio = "CSCS, Head Strength Coach. Former CFL athlete with 10+ years coaching experience. Specializes in athletic performance and injury prevention.",
                        IsActive = true
                    },
                    new StaffMember
                    {
                        Id = Guid.NewGuid(),
                        Name = "Sofia Rodriguez",
                        Email = "sofia@elevationfitness.ca",
                        Bio = "Certified Personal Trainer & Nutrition Coach. Passionate about helping clients achieve sustainable lifestyle changes.",
                        IsActive = true
                    }
                }
            }
        };

        context.Providers.AddRange(providers);

        await context.SaveChangesAsync();
    }
}
