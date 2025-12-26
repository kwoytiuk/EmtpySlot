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
            },
            new Provider
            {
                Id = Guid.NewGuid(),
                UserId = Guid.NewGuid(),
                BusinessName = "Radiance Hair Boutique Edmonton",
                Description = "Trendy hair boutique specializing in fashion-forward cuts, vivid color, and hair extensions. Instagram-worthy transformations by award-winning stylists.",
                Email = "style@radiancehair.ca",
                Phone = "+1 (780) 555-0505",
                Website = "https://radiancehair.ca",
                RatingAverage = 4.9m,
                RatingCount = 412,
                Verified = true,
                IsFeatured = true,
                CreatedAt = DateTime.UtcNow,
                Locations = new List<ProviderLocation>
                {
                    new ProviderLocation
                    {
                        Id = Guid.NewGuid(),
                        Name = "Oliver District",
                        AddressLine1 = "11044 82nd Avenue NW",
                        City = "Edmonton",
                        StateProvince = "AB",
                        PostalCode = "T6G 0T2",
                        Country = "Canada",
                        Phone = "+1 (780) 555-0505",
                        Latitude = 53.5232,
                        Longitude = -113.5106,
                        IsPrimary = true
                    }
                },
                Services = new List<Service>
                {
                    new Service { Id = Guid.NewGuid(), Name = "Fashion Color", Description = "Bold, vibrant color transformations. Fantasy colors, pastels, or rainbow hair.", CategoryId = hairCategory.Id, DurationMinutes = 240, Price = 350.00m, DepositRequired = 120.00m, IsActive = true },
                    new Service { Id = Guid.NewGuid(), Name = "Hair Extensions Application", Description = "Tape-in or keratin bond extensions. Add length and volume that lasts months.", CategoryId = hairCategory.Id, DurationMinutes = 180, Price = 425.00m, DepositRequired = 150.00m, IsActive = true },
                    new Service { Id = Guid.NewGuid(), Name = "Updo Styling", Description = "Special occasion styling for weddings, proms, or events. Includes trial run.", CategoryId = hairCategory.Id, DurationMinutes = 90, Price = 125.00m, IsActive = true }
                },
                StaffMembers = new List<StaffMember>
                {
                    new StaffMember { Id = Guid.NewGuid(), Name = "Zara Malik", Email = "zara@radiancehair.ca", Bio = "Color Specialist. L'Oréal certified, featured in Salon Magazine. Expert in vivid fashion colors.", IsActive = true }
                }
            },
            new Provider
            {
                Id = Guid.NewGuid(),
                UserId = Guid.NewGuid(),
                BusinessName = "Zen Balance Wellness Red Deer",
                Description = "Holistic wellness center offering RMT massage, acupuncture, and physiotherapy. Integrated approach to health and recovery in Red Deer's premiere facility.",
                Email = "info@zenbalance.ca",
                Phone = "+1 (403) 555-0606",
                Website = "https://zenbalance.ca",
                RatingAverage = 4.8m,
                RatingCount = 234,
                Verified = true,
                IsFeatured = false,
                CreatedAt = DateTime.UtcNow,
                Locations = new List<ProviderLocation>
                {
                    new ProviderLocation
                    {
                        Id = Guid.NewGuid(),
                        Name = "Parkland Mall Medical Centre",
                        AddressLine1 = "4900 Molly Bannister Drive",
                        City = "Red Deer",
                        StateProvince = "AB",
                        PostalCode = "T4R 0A7",
                        Country = "Canada",
                        Phone = "+1 (403) 555-0606",
                        Latitude = 52.2681,
                        Longitude = -113.8111,
                        IsPrimary = true
                    }
                },
                Services = new List<Service>
                {
                    new Service { Id = Guid.NewGuid(), Name = "Therapeutic Massage (60 min)", Description = "RMT treatment covered by most insurance plans. Focus on injury recovery and pain management.", CategoryId = spaCategory.Id, DurationMinutes = 60, Price = 110.00m, IsActive = true },
                    new Service { Id = Guid.NewGuid(), Name = "Acupuncture Session", Description = "Traditional Chinese medicine acupuncture for pain relief, stress, and wellness.", CategoryId = spaCategory.Id, DurationMinutes = 45, Price = 95.00m, IsActive = true },
                    new Service { Id = Guid.NewGuid(), Name = "Physiotherapy Assessment", Description = "Comprehensive assessment with personalized treatment plan. Direct billing available.", CategoryId = spaCategory.Id, DurationMinutes = 60, Price = 130.00m, IsActive = true }
                },
                StaffMembers = new List<StaffMember>
                {
                    new StaffMember { Id = Guid.NewGuid(), Name = "Dr. James Wong", Email = "dr.wong@zenbalance.ca", Bio = "Physiotherapist & Acupuncturist. 15 years clinical experience, sports injury specialist.", IsActive = true },
                    new StaffMember { Id = Guid.NewGuid(), Name = "Karen Schmidt", Email = "karen@zenbalance.ca", Bio = "RMT with advanced certification in myofascial release and trigger point therapy.", IsActive = true }
                }
            },
            new Provider
            {
                Id = Guid.NewGuid(),
                UserId = Guid.NewGuid(),
                BusinessName = "Core Strength Athletics Lethbridge",
                Description = "CrossFit box and strength training facility. Olympic lifting platforms, competition rigs, and expert coaching. From beginners to competitive athletes.",
                Email = "train@corestrength.ca",
                Phone = "+1 (403) 555-0707",
                Website = "https://corestrength.ca",
                RatingAverage = 4.9m,
                RatingCount = 198,
                Verified = true,
                IsFeatured = false,
                CreatedAt = DateTime.UtcNow,
                Locations = new List<ProviderLocation>
                {
                    new ProviderLocation
                    {
                        Id = Guid.NewGuid(),
                        Name = "West Lethbridge",
                        AddressLine1 = "2920 18th Avenue North",
                        City = "Lethbridge",
                        StateProvince = "AB",
                        PostalCode = "T1H 5C9",
                        Country = "Canada",
                        Phone = "+1 (403) 555-0707",
                        Latitude = 49.7050,
                        Longitude = -112.8389,
                        IsPrimary = true
                    }
                },
                Services = new List<Service>
                {
                    new Service { Id = Guid.NewGuid(), Name = "CrossFit Class", Description = "High-intensity functional fitness. Scalable for all levels. Unlimited monthly membership available.", CategoryId = fitnessCategory.Id, DurationMinutes = 60, Price = 22.00m, IsActive = true },
                    new Service { Id = Guid.NewGuid(), Name = "Olympic Lifting Coaching", Description = "Learn snatch and clean & jerk with certified USAW coach. Technical skill development.", CategoryId = fitnessCategory.Id, DurationMinutes = 60, Price = 85.00m, IsActive = true },
                    new Service { Id = Guid.NewGuid(), Name = "Strength & Conditioning Program", Description = "12-week personalized program with weekly check-ins. Includes nutrition guidance.", CategoryId = fitnessCategory.Id, DurationMinutes = 60, Price = 750.00m, DepositRequired = 250.00m, IsActive = true }
                },
                StaffMembers = new List<StaffMember>
                {
                    new StaffMember { Id = Guid.NewGuid(), Name = "Tyler Brooks", Email = "tyler@corestrength.ca", Bio = "Head Coach, CF-L2, USAW Sports Performance Coach. Former provincial powerlifting champion.", IsActive = true }
                }
            },
            new Provider
            {
                Id = Guid.NewGuid(),
                UserId = Guid.NewGuid(),
                BusinessName = "Bella Nails & Spa Calgary South",
                Description = "Upscale nail salon and spa in south Calgary. Organic products, sterilization certified. Specializing in gel nails, nail art, and pampering pedicures.",
                Email = "book@bellanails.ca",
                Phone = "+1 (403) 555-0808",
                Website = "https://bellanails.ca",
                RatingAverage = 4.6m,
                RatingCount = 167,
                Verified = true,
                IsFeatured = false,
                CreatedAt = DateTime.UtcNow,
                Locations = new List<ProviderLocation>
                {
                    new ProviderLocation
                    {
                        Id = Guid.NewGuid(),
                        Name = "Shawnessy Shopping Centre",
                        AddressLine1 = "70 Shawville Boulevard SE",
                        City = "Calgary",
                        StateProvince = "AB",
                        PostalCode = "T2Y 2Z3",
                        Country = "Canada",
                        Phone = "+1 (403) 555-0808",
                        Latitude = 50.9045,
                        Longitude = -114.0653,
                        IsPrimary = true
                    }
                },
                Services = new List<Service>
                {
                    new Service { Id = Guid.NewGuid(), Name = "Gel Extensions", Description = "Full set gel nail extensions. Long-lasting, natural look. 2-3 week wear.", CategoryId = nailsCategory.Id, DurationMinutes = 90, Price = 80.00m, IsActive = true },
                    new Service { Id = Guid.NewGuid(), Name = "Luxury Pedicure", Description = "Extended pedicure with sugar scrub, mud mask, and hot stone massage.", CategoryId = nailsCategory.Id, DurationMinutes = 90, Price = 95.00m, IsActive = true },
                    new Service { Id = Guid.NewGuid(), Name = "Dip Powder Manicure", Description = "Durable dip powder application. Lasts 3-4 weeks without chipping.", CategoryId = nailsCategory.Id, DurationMinutes = 75, Price = 70.00m, IsActive = true }
                },
                StaffMembers = new List<StaffMember>
                {
                    new StaffMember { Id = Guid.NewGuid(), Name = "Linh Nguyen", Email = "linh@bellanails.ca", Bio = "Master Nail Technician, 9 years experience. Specializes in intricate 3D nail art and extensions.", IsActive = true }
                }
            },
            new Provider
            {
                Id = Guid.NewGuid(),
                UserId = Guid.NewGuid(),
                BusinessName = "Urban Barber Co. Calgary",
                Description = "Modern barbershop offering classic and contemporary men's grooming. Hot shave service, beard sculpting, and premium hair products.",
                Email = "bookings@urbanbarber.ca",
                Phone = "+1 (403) 555-0909",
                Website = "https://urbanbarber.ca",
                RatingAverage = 4.9m,
                RatingCount = 523,
                Verified = true,
                IsFeatured = true,
                CreatedAt = DateTime.UtcNow,
                Locations = new List<ProviderLocation>
                {
                    new ProviderLocation
                    {
                        Id = Guid.NewGuid(),
                        Name = "17th Avenue SW",
                        AddressLine1 = "1420 17th Avenue SW",
                        City = "Calgary",
                        StateProvince = "AB",
                        PostalCode = "T2T 0C6",
                        Country = "Canada",
                        Phone = "+1 (403) 555-0909",
                        Latitude = 51.0373,
                        Longitude = -114.0881,
                        IsPrimary = true
                    }
                },
                Services = new List<Service>
                {
                    new Service { Id = Guid.NewGuid(), Name = "Signature Haircut", Description = "Precision cut with hot towel, shampoo, and styling. Complimentary beverage included.", CategoryId = hairCategory.Id, DurationMinutes = 45, Price = 48.00m, IsActive = true },
                    new Service { Id = Guid.NewGuid(), Name = "Executive Shave", Description = "Traditional straight razor shave with pre-shave oil, hot towels, and aftershave treatment.", CategoryId = hairCategory.Id, DurationMinutes = 45, Price = 55.00m, IsActive = true },
                    new Service { Id = Guid.NewGuid(), Name = "Beard Grooming", Description = "Beard trim and shaping with oil conditioning. Maintain your signature look.", CategoryId = hairCategory.Id, DurationMinutes = 30, Price = 35.00m, IsActive = true }
                },
                StaffMembers = new List<StaffMember>
                {
                    new StaffMember { Id = Guid.NewGuid(), Name = "Alex Rodriguez", Email = "alex@urbanbarber.ca", Bio = "Master Barber with Red Seal certification. 14 years experience in classic and modern cuts.", IsActive = true },
                    new StaffMember { Id = Guid.NewGuid(), Name = "Brandon Lee", Email = "brandon@urbanbarber.ca", Bio = "Barber & Beard Specialist. Expert in fade techniques and beard sculpting.", IsActive = true }
                }
            },
            new Provider
            {
                Id = Guid.NewGuid(),
                UserId = Guid.NewGuid(),
                BusinessName = "Namaste Yoga Studio Airdrie",
                Description = "Community-focused yoga studio offering Hatha, Vinyasa, and Yin yoga. Beginner-friendly classes and experienced teacher training programs.",
                Email = "om@namasteyoga.ca",
                Phone = "+1 (403) 555-1010",
                Website = "https://namasteyoga.ca",
                RatingAverage = 4.8m,
                RatingCount = 156,
                Verified = true,
                IsFeatured = false,
                CreatedAt = DateTime.UtcNow,
                Locations = new List<ProviderLocation>
                {
                    new ProviderLocation
                    {
                        Id = Guid.NewGuid(),
                        Name = "Main Street Airdrie",
                        AddressLine1 = "105 Main Street South",
                        City = "Airdrie",
                        StateProvince = "AB",
                        PostalCode = "T4B 3C3",
                        Country = "Canada",
                        Phone = "+1 (403) 555-1010",
                        Latitude = 51.2917,
                        Longitude = -114.0144,
                        IsPrimary = true
                    }
                },
                Services = new List<Service>
                {
                    new Service { Id = Guid.NewGuid(), Name = "Vinyasa Flow (60 min)", Description = "Dynamic, flowing yoga practice linking breath with movement. All levels welcome.", CategoryId = fitnessCategory.Id, DurationMinutes = 60, Price = 22.00m, IsActive = true },
                    new Service { Id = Guid.NewGuid(), Name = "Yin Yoga (75 min)", Description = "Slow-paced practice with poses held longer. Deep stretch and meditation combined.", CategoryId = fitnessCategory.Id, DurationMinutes = 75, Price = 25.00m, IsActive = true },
                    new Service { Id = Guid.NewGuid(), Name = "Private Yoga Session", Description = "One-on-one personalized yoga instruction. Perfect for beginners or specific goals.", CategoryId = fitnessCategory.Id, DurationMinutes = 60, Price = 90.00m, IsActive = true }
                },
                StaffMembers = new List<StaffMember>
                {
                    new StaffMember { Id = Guid.NewGuid(), Name = "Priya Sharma", Email = "priya@namasteyoga.ca", Bio = "RYT-500 Certified Yoga Instructor. Trained in India, teaching mindful yoga for 12 years.", IsActive = true }
                }
            },
            new Provider
            {
                Id = Guid.NewGuid(),
                UserId = Guid.NewGuid(),
                BusinessName = "Revive Esthetics & Laser Edmonton",
                Description = "Medical esthetics clinic offering laser treatments, injectables, and advanced skincare. Physician-supervised by board-certified doctors.",
                Email = "consult@reviveesthetics.ca",
                Phone = "+1 (780) 555-1111",
                Website = "https://reviveesthetics.ca",
                RatingAverage = 4.9m,
                RatingCount = 289,
                Verified = true,
                IsFeatured = true,
                CreatedAt = DateTime.UtcNow,
                Locations = new List<ProviderLocation>
                {
                    new ProviderLocation
                    {
                        Id = Guid.NewGuid(),
                        Name = "Southgate Centre Medical",
                        AddressLine1 = "5015 111th Street NW",
                        City = "Edmonton",
                        StateProvince = "AB",
                        PostalCode = "T6H 3M7",
                        Country = "Canada",
                        Phone = "+1 (780) 555-1111",
                        Latitude = 53.4809,
                        Longitude = -113.5081,
                        IsPrimary = true
                    }
                },
                Services = new List<Service>
                {
                    new Service { Id = Guid.NewGuid(), Name = "Botox Treatment", Description = "Cosmetic botulinum toxin injections for wrinkle reduction. Consultation included.", CategoryId = spaCategory.Id, DurationMinutes = 30, Price = 275.00m, IsActive = true },
                    new Service { Id = Guid.NewGuid(), Name = "Laser Hair Removal", Description = "Permanent hair reduction using advanced laser technology. Package pricing available.", CategoryId = spaCategory.Id, DurationMinutes = 45, Price = 150.00m, IsActive = true },
                    new Service { Id = Guid.NewGuid(), Name = "Chemical Peel", Description = "Medical-grade chemical peel for skin rejuvenation and texture improvement.", CategoryId = spaCategory.Id, DurationMinutes = 60, Price = 225.00m, IsActive = true }
                },
                StaffMembers = new List<StaffMember>
                {
                    new StaffMember { Id = Guid.NewGuid(), Name = "Dr. Amanda Foster", Email = "dr.foster@reviveesthetics.ca", Bio = "Medical Director, Board-Certified Dermatologist with 18 years experience in cosmetic procedures.", IsActive = true },
                    new StaffMember { Id = Guid.NewGuid(), Name = "Natalie Kim", Email = "natalie@reviveesthetics.ca", Bio = "Licensed Medical Esthetician specializing in laser treatments and advanced skincare.", IsActive = true }
                }
            },
            new Provider
            {
                Id = Guid.NewGuid(),
                UserId = Guid.NewGuid(),
                BusinessName = "Peak Performance Pilates Calgary",
                Description = "Reformer Pilates studio with small class sizes and private sessions. Core strength, flexibility, and injury rehabilitation in a supportive environment.",
                Email = "studio@peakpilates.ca",
                Phone = "+1 (403) 555-1212",
                Website = "https://peakpilates.ca",
                RatingAverage = 4.8m,
                RatingCount = 203,
                Verified = true,
                IsFeatured = false,
                CreatedAt = DateTime.UtcNow,
                Locations = new List<ProviderLocation>
                {
                    new ProviderLocation
                    {
                        Id = Guid.NewGuid(),
                        Name = "Inglewood",
                        AddressLine1 = "1235 9th Avenue SE",
                        City = "Calgary",
                        StateProvince = "AB",
                        PostalCode = "T2G 0T3",
                        Country = "Canada",
                        Phone = "+1 (403) 555-1212",
                        Latitude = 51.0378,
                        Longitude = -114.0389,
                        IsPrimary = true
                    }
                },
                Services = new List<Service>
                {
                    new Service { Id = Guid.NewGuid(), Name = "Reformer Pilates Class", Description = "Full-body workout on Pilates reformer. Maximum 6 people per class for personalized attention.", CategoryId = fitnessCategory.Id, DurationMinutes = 55, Price = 32.00m, IsActive = true },
                    new Service { Id = Guid.NewGuid(), Name = "Private Pilates Session", Description = "One-on-one reformer session tailored to your goals and fitness level.", CategoryId = fitnessCategory.Id, DurationMinutes = 55, Price = 95.00m, IsActive = true },
                    new Service { Id = Guid.NewGuid(), Name = "Mat Pilates", Description = "Classical mat Pilates focusing on core strength and controlled movement.", CategoryId = fitnessCategory.Id, DurationMinutes = 50, Price = 24.00m, IsActive = true }
                },
                StaffMembers = new List<StaffMember>
                {
                    new StaffMember { Id = Guid.NewGuid(), Name = "Lindsay Carter", Email = "lindsay@peakpilates.ca", Bio = "Certified Pilates Instructor (PMA). Former professional dancer with expertise in rehabilitation.", IsActive = true }
                }
            }
        };

        context.Providers.AddRange(providers);

        await context.SaveChangesAsync();
    }
}
