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

        // Create service categories
        var categories = CreateCategories();
        context.ServiceCategories.AddRange(categories);
        await context.SaveChangesAsync();

        // Create providers with bookings
        var providers = CreateProviders(categories);
        context.Providers.AddRange(providers);
        await context.SaveChangesAsync();

        // Create staff members and schedules for providers
        var (staffMembers, schedules) = CreateStaffAndSchedules(providers);
        context.StaffMembers.AddRange(staffMembers);
        await context.SaveChangesAsync();

        context.StaffSchedules.AddRange(schedules);
        await context.SaveChangesAsync();

        // Generate time slots for the next 14 days
        var timeSlots = GenerateTimeSlots(staffMembers, schedules);
        context.TimeSlots.AddRange(timeSlots);
        await context.SaveChangesAsync();

        var profiles = CreateProfiles();
        context.Profiles.AddRange(profiles);
        await context.SaveChangesAsync();
    }

    private static List<Profile> CreateProfiles()
    {
        return new List<Profile>
        {
            new Profile  {
                Id = Guid.NewGuid(),
                UserType = UserType.Customer,
                FullName = "Ken Woytiuk",
                Email = "kenwoytiuk@gmail.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Jadeliam1!"),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }
        };
    }

    private static List<ServiceCategory> CreateCategories()
    {
        return new List<ServiceCategory>
        {
            new ServiceCategory
            {
                Id = Guid.NewGuid(),
                Name = "Hair Salons",
                Slug = "hair-salons",
                Icon = "✂️",
                DisplayOrder = 1,
                CreatedAt = DateTime.UtcNow
            },
            new ServiceCategory
            {
                Id = Guid.NewGuid(),
                Name = "Spa & Wellness",
                Slug = "spa-wellness",
                Icon = "💆",
                DisplayOrder = 2,
                CreatedAt = DateTime.UtcNow
            },
            new ServiceCategory
            {
                Id = Guid.NewGuid(),
                Name = "Nail Salons",
                Slug = "nail-salons",
                Icon = "💅",
                DisplayOrder = 3,
                CreatedAt = DateTime.UtcNow
            },
            new ServiceCategory
            {
                Id = Guid.NewGuid(),
                Name = "Fitness & Gym",
                Slug = "fitness-gym",
                Icon = "💪",
                DisplayOrder = 4,
                CreatedAt = DateTime.UtcNow
            },
            new ServiceCategory
            {
                Id = Guid.NewGuid(),
                Name = "Medical & Dental",
                Slug = "medical-dental",
                Icon = "🏥",
                DisplayOrder = 5,
                CreatedAt = DateTime.UtcNow
            },
            new ServiceCategory
            {
                Id = Guid.NewGuid(),
                Name = "Beauty & Esthetics",
                Slug = "beauty-esthetics",
                Icon = "✨",
                DisplayOrder = 6,
                CreatedAt = DateTime.UtcNow
            },
            new ServiceCategory
            {
                Id = Guid.NewGuid(),
                Name = "Automotive",
                Slug = "automotive",
                Icon = "🚗",
                DisplayOrder = 7,
                CreatedAt = DateTime.UtcNow
            }
        };
    }

    private static List<Provider> CreateProviders(List<ServiceCategory> categories)
    {
        var providers = new List<Provider>();
        var random = new Random(42); // Seed for consistent data

        // Get category references
        var hairCat = categories.First(c => c.Slug == "hair-salons");
        var spaCat = categories.First(c => c.Slug == "spa-wellness");
        var nailsCat = categories.First(c => c.Slug == "nail-salons");
        var fitnessCat = categories.First(c => c.Slug == "fitness-gym");
        var medicalCat = categories.First(c => c.Slug == "medical-dental");
        var beautyCat = categories.First(c => c.Slug == "beauty-esthetics");
        var autoCat = categories.First(c => c.Slug == "automotive");

        // Calgary Hair Salons (20)
        providers.AddRange(CreateHairSalons(hairCat, "Calgary", 51.0447, -114.0719, 20, random));

        // Edmonton Hair Salons (15)
        providers.AddRange(CreateHairSalons(hairCat, "Edmonton", 53.5461, -113.4938, 15, random));

        // Calgary Spas (15)
        providers.AddRange(CreateSpas(spaCat, "Calgary", 51.0447, -114.0719, 15, random));

        // Edmonton Spas (10)
        providers.AddRange(CreateSpas(spaCat, "Edmonton", 53.5461, -113.4938, 10, random));

        // Nail Salons (15 total)
        providers.AddRange(CreateNailSalons(nailsCat, "Calgary", 51.0447, -114.0719, 10, random));
        providers.AddRange(CreateNailSalons(nailsCat, "Edmonton", 53.5461, -113.4938, 5, random));

        // Fitness Centers (12 total)
        providers.AddRange(CreateFitnessCenters(fitnessCat, "Calgary", 51.0447, -114.0719, 8, random));
        providers.AddRange(CreateFitnessCenters(fitnessCat, "Edmonton", 53.5461, -113.4938, 4, random));

        return providers;
    }

    private static List<Provider> CreateHairSalons(ServiceCategory category, string city, double baseLat, double baseLng, int count, Random random)
    {
        var salonNames = new[]
        {
            "Luxe Hair Studio", "Urban Cuts & Color", "Tress Lounge", "The Hair Lab",
            "Salon Bleu", "Muse Hair Design", "Avant-Garde Salon", "Pure Hair Lounge",
            "Fringe Hair Studio", "The Styling Co.", "Bombshell Salon", "Alchemy Hair",
            "Canvas Hair Studio", "Halo Hair Design", "Mint Salon"
        };

        var providers = new List<Provider>();

        for (int i = 0; i < count && i < salonNames.Length; i++)
        {
            var name = salonNames[i] + (city != "Calgary" ? $" {city}" : "");
            var rating = 4.0m + (decimal)(random.NextDouble() * 1.0);

            var provider = new Provider
            {
                Id = Guid.NewGuid(),
                UserId = null, // Seeded providers don't have user accounts
                BusinessName = name,
                Description = "Award-winning salon specializing in precision cuts, color treatments, and luxury styling. Our expert stylists stay current with the latest trends.",
                Email = $"book@{name.ToLower().Replace(" ", "")}.ca",
                Phone = $"+1 ({GetAreaCode(city)}) {random.Next(200, 999)}-{random.Next(1000, 9999)}",
                RatingAverage = Math.Round(rating, 1),
                RatingCount = random.Next(50, 500),
                Verified = true,
                IsFeatured = i < 3,
                CreatedAt = DateTime.UtcNow.AddDays(-random.Next(30, 365)),
                Locations = new List<ProviderLocation>
                {
                    new ProviderLocation
                    {
                        Id = Guid.NewGuid(),
                        Name = $"{city} Location",
                        AddressLine1 = $"{random.Next(100, 9999)} {GetStreetName(random)} {GetStreetType(random)}",
                        City = city,
                        StateProvince = "AB",
                        PostalCode = GetPostalCode(random),
                        Country = "Canada",
                        Latitude = baseLat + (random.NextDouble() - 0.5) * 0.1,
                        Longitude = baseLng + (random.NextDouble() - 0.5) * 0.1,
                        IsPrimary = true
                    }
                },
                Services = new List<Service>
                {
                    new Service { Id = Guid.NewGuid(), Name = "Women's Cut & Style", CategoryId = category.Id, DurationMinutes = 60, Price = 85.00m, ImageUrl = "https://images.unsplash.com/photo-1562322140-8baeececf3df?w=400", IsActive = true },
                    new Service { Id = Guid.NewGuid(), Name = "Men's Cut", CategoryId = category.Id, DurationMinutes = 30, Price = 45.00m, ImageUrl = "https://images.unsplash.com/photo-1503951914875-452162b0f3f1?w=400", IsActive = true },
                    new Service { Id = Guid.NewGuid(), Name = "Color Treatment", CategoryId = category.Id, DurationMinutes = 120, Price = 150.00m, DepositRequired = 50.00m, ImageUrl = "https://images.unsplash.com/photo-1560066984-138dadb4c035?w=400", IsActive = true },
                    new Service { Id = Guid.NewGuid(), Name = "Balayage/Highlights", CategoryId = category.Id, DurationMinutes = 180, Price = 250.00m, DepositRequired = 75.00m, ImageUrl = "https://images.unsplash.com/photo-1522337660859-02fbefca4702?w=400", IsActive = true }
                }
            };

            providers.Add(provider);
        }

        return providers;
    }

    private static List<Provider> CreateSpas(ServiceCategory category, string city, double baseLat, double baseLng, int count, Random random)
    {
        var spaNames = new[]
        {
            "Serenity Spa & Wellness", "Zen Retreat", "Oasis Day Spa", "The Spa at River",
            "Bliss Wellness Centre", "Tranquility Spa", "Radiance Med Spa", "Elements Spa",
            "Lotus Wellness Spa", "Revive Day Spa"
        };

        var providers = new List<Provider>();

        for (int i = 0; i < count && i < spaNames.Length; i++)
        {
            var name = spaNames[i] + (city != "Calgary" ? $" {city}" : "");

            var provider = new Provider
            {
                Id = Guid.NewGuid(),
                UserId = null, // Seeded providers don't have user accounts
                BusinessName = name,
                Description = "Escape to tranquility with our premium spa treatments. From massage to facials, we offer the ultimate relaxation experience.",
                Email = $"info@{name.ToLower().Replace(" ", "")}.ca",
                Phone = $"+1 ({GetAreaCode(city)}) {random.Next(200, 999)}-{random.Next(1000, 9999)}",
                RatingAverage = Math.Round(4.2m + (decimal)(random.NextDouble() * 0.8), 1),
                RatingCount = random.Next(100, 800),
                Verified = true,
                IsFeatured = i < 2,
                CreatedAt = DateTime.UtcNow.AddDays(-random.Next(30, 365)),
                Locations = new List<ProviderLocation>
                {
                    new ProviderLocation
                    {
                        Id = Guid.NewGuid(),
                        Name = $"{city} Location",
                        AddressLine1 = $"{random.Next(100, 9999)} {GetStreetName(random)} {GetStreetType(random)}",
                        City = city,
                        StateProvince = "AB",
                        PostalCode = GetPostalCode(random),
                        Country = "Canada",
                        Latitude = baseLat + (random.NextDouble() - 0.5) * 0.1,
                        Longitude = baseLng + (random.NextDouble() - 0.5) * 0.1,
                        IsPrimary = true
                    }
                },
                Services = new List<Service>
                {
                    new Service { Id = Guid.NewGuid(), Name = "Swedish Massage (60min)", CategoryId = category.Id, DurationMinutes = 60, Price = 120.00m, ImageUrl = "https://images.unsplash.com/photo-1544161515-4ab6ce6db874?w=400", IsActive = true },
                    new Service { Id = Guid.NewGuid(), Name = "Deep Tissue Massage (90min)", CategoryId = category.Id, DurationMinutes = 90, Price = 165.00m, ImageUrl = "https://images.unsplash.com/photo-1600334129128-685c5582fd35?w=400", IsActive = true },
                    new Service { Id = Guid.NewGuid(), Name = "Facial Treatment", CategoryId = category.Id, DurationMinutes = 75, Price = 140.00m, ImageUrl = "https://images.unsplash.com/photo-1570172619644-dfd03ed5d881?w=400", IsActive = true },
                    new Service { Id = Guid.NewGuid(), Name = "Body Scrub & Wrap", CategoryId = category.Id, DurationMinutes = 90, Price = 180.00m, DepositRequired = 60.00m, ImageUrl = "https://images.unsplash.com/photo-1582719366362-f49c26d52536?w=400", IsActive = true }
                }
            };

            providers.Add(provider);
        }

        return providers;
    }

    private static List<Provider> CreateNailSalons(ServiceCategory category, string city, double baseLat, double baseLng, int count, Random random)
    {
        var salonNames = new[] { "Polished Nail Studio", "Bella Nails & Spa", "The Nail Bar", "Luxe Nails", "Perfect Polish", "Gloss Nail Lounge" };
        var providers = new List<Provider>();

        for (int i = 0; i < count && i < salonNames.Length; i++)
        {
            var name = salonNames[i] + $" {city}";
            providers.Add(new Provider
            {
                Id = Guid.NewGuid(),
                UserId = null, // Seeded providers don't have user accounts
                BusinessName = name,
                Description = "Professional nail care and artistry. From classic manicures to intricate nail art designs.",
                Email = $"hello@{name.ToLower().Replace(" ", "")}.ca",
                Phone = $"+1 ({GetAreaCode(city)}) {random.Next(200, 999)}-{random.Next(1000, 9999)}",
                RatingAverage = Math.Round(4.3m + (decimal)(random.NextDouble() * 0.7), 1),
                RatingCount = random.Next(50, 400),
                Verified = true,
                IsFeatured = i == 0,
                CreatedAt = DateTime.UtcNow.AddDays(-random.Next(30, 365)),
                Locations = new List<ProviderLocation>
                {
                    new ProviderLocation
                    {
                        Id = Guid.NewGuid(),
                        Name = $"{city} Location",
                        AddressLine1 = $"{random.Next(100, 9999)} {GetStreetName(random)} {GetStreetType(random)}",
                        City = city,
                        StateProvince = "AB",
                        PostalCode = GetPostalCode(random),
                        Country = "Canada",
                        Latitude = baseLat + (random.NextDouble() - 0.5) * 0.1,
                        Longitude = baseLng + (random.NextDouble() - 0.5) * 0.1,
                        IsPrimary = true
                    }
                },
                Services = new List<Service>
                {
                    new Service { Id = Guid.NewGuid(), Name = "Gel Manicure", CategoryId = category.Id, DurationMinutes = 45, Price = 55.00m, ImageUrl = "https://images.unsplash.com/photo-1604654894610-df63bc536371?w=400", IsActive = true },
                    new Service { Id = Guid.NewGuid(), Name = "Spa Pedicure", CategoryId = category.Id, DurationMinutes = 60, Price = 70.00m, ImageUrl = "https://images.unsplash.com/photo-1519014816548-bf5fe059798b?w=400", IsActive = true },
                    new Service { Id = Guid.NewGuid(), Name = "Acrylic Full Set", CategoryId = category.Id, DurationMinutes = 90, Price = 85.00m, ImageUrl = "https://images.unsplash.com/photo-1610992015732-2449b76344bc?w=400", IsActive = true }
                }
            });
        }

        return providers;
    }

    private static List<Provider> CreateFitnessCenters(ServiceCategory category, string city, double baseLat, double baseLng, int count, Random random)
    {
        var gymNames = new[] { "Elevation Fitness", "Core Strength Athletics", "Peak Performance", "Iron Fitness Club", "Vitality Gym" };
        var providers = new List<Provider>();

        for (int i = 0; i < count && i < gymNames.Length; i++)
        {
            var name = gymNames[i] + $" {city}";
            providers.Add(new Provider
            {
                Id = Guid.NewGuid(),
                UserId = null, // Seeded providers don't have user accounts
                BusinessName = name,
                Description = "State-of-the-art fitness facility with personal training, group classes, and premium equipment.",
                Email = $"join@{name.ToLower().Replace(" ", "")}.ca",
                Phone = $"+1 ({GetAreaCode(city)}) {random.Next(200, 999)}-{random.Next(1000, 9999)}",
                RatingAverage = Math.Round(4.4m + (decimal)(random.NextDouble() * 0.6), 1),
                RatingCount = random.Next(100, 600),
                Verified = true,
                IsFeatured = i < 2,
                CreatedAt = DateTime.UtcNow.AddDays(-random.Next(30, 365)),
                Locations = new List<ProviderLocation>
                {
                    new ProviderLocation
                    {
                        Id = Guid.NewGuid(),
                        Name = $"{city} Location",
                        AddressLine1 = $"{random.Next(100, 9999)} {GetStreetName(random)} {GetStreetType(random)}",
                        City = city,
                        StateProvince = "AB",
                        PostalCode = GetPostalCode(random),
                        Country = "Canada",
                        Latitude = baseLat + (random.NextDouble() - 0.5) * 0.1,
                        Longitude = baseLng + (random.NextDouble() - 0.5) * 0.1,
                        IsPrimary = true
                    }
                },
                Services = new List<Service>
                {
                    new Service { Id = Guid.NewGuid(), Name = "Personal Training Session", CategoryId = category.Id, DurationMinutes = 60, Price = 95.00m, ImageUrl = "https://images.unsplash.com/photo-1571019614242-c5c5dee9f50b?w=400", IsActive = true },
                    new Service { Id = Guid.NewGuid(), Name = "Group Fitness Class", CategoryId = category.Id, DurationMinutes = 45, Price = 25.00m, ImageUrl = "https://images.unsplash.com/photo-1518611012118-696072aa579a?w=400", IsActive = true },
                    new Service { Id = Guid.NewGuid(), Name = "Fitness Assessment", CategoryId = category.Id, DurationMinutes = 30, Price = 50.00m, ImageUrl = "https://images.unsplash.com/photo-1517836357463-d25dfeac3438?w=400", IsActive = true }
                }
            });
        }

        return providers;
    }

    // Helper methods
    private static string GetAreaCode(string city) => city switch
    {
        "Calgary" => "403",
        "Edmonton" => "780",
        "Red Deer" => "403",
        "Lethbridge" => "403",
        _ => "403"
    };

    private static string GetStreetName(Random random)
    {
        var names = new[] { "Main", "1st", "2nd", "3rd", "4th", "5th", "Centre", "Stephen", "Edmonton Trail", "17th Ave", "Macleod Trail", "Crowchild Trail", "Deerfoot", "Bow", "Elbow" };
        return names[random.Next(names.Length)];
    }

    private static string GetStreetType(Random random)
    {
        var types = new[] { "Street", "Avenue", "Road", "Drive", "Way", "Boulevard" };
        return types[random.Next(types.Length)];
    }

    private static string GetPostalCode(Random random)
    {
        var letters = "ABCDEFGHJKLMNPRSTVWXYZ";
        return $"T{random.Next(1, 4)}{letters[random.Next(letters.Length)]} {random.Next(1, 10)}{letters[random.Next(letters.Length)]}{random.Next(0, 10)}";
    }

    private static (List<StaffMember>, List<StaffSchedule>) CreateStaffAndSchedules(List<Provider> providers)
    {
        var staffMembers = new List<StaffMember>();
        var schedules = new List<StaffSchedule>();
        var random = new Random(42);

        var firstNames = new[] { "Sarah", "Michael", "Jessica", "David", "Emily", "James", "Ashley", "Daniel", "Amanda", "Ryan", "Jennifer", "Matthew", "Nicole", "Christopher", "Melissa" };
        var lastNames = new[] { "Smith", "Johnson", "Williams", "Brown", "Jones", "Garcia", "Miller", "Davis", "Rodriguez", "Martinez", "Lee", "Wilson", "Anderson", "Taylor", "Thomas" };

        foreach (var provider in providers)
        {
            // Create 2-4 staff members per provider
            int staffCount = random.Next(2, 5);

            for (int i = 0; i < staffCount; i++)
            {
                var firstName = firstNames[random.Next(firstNames.Length)];
                var lastName = lastNames[random.Next(lastNames.Length)];
                var fullName = $"{firstName} {lastName}";

                var staff = new StaffMember
                {
                    Id = Guid.NewGuid(),
                    ProviderId = provider.Id,
                    Name = fullName,
                    Email = $"{firstName.ToLower()}.{lastName.ToLower()}@{provider.BusinessName.ToLower().Replace(" ", "")}.ca",
                    Phone = provider.Phone,
                    Bio = $"Experienced professional with {random.Next(3, 15)} years in the industry.",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                staffMembers.Add(staff);

                // Create weekly schedule for this staff member
                // Most staff work Mon-Fri, some also work weekends
                var workDays = new List<DayOfWeek> { DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday };

                // 30% chance to also work Saturday
                if (random.NextDouble() < 0.3)
                    workDays.Add(DayOfWeek.Saturday);

                foreach (var day in workDays)
                {
                    // Work hours: usually 9 AM to 5 PM or 10 AM to 6 PM
                    var startHour = random.Next(8, 11); // 8-10 AM
                    var endHour = random.Next(17, 19);  // 5-6 PM

                    var schedule = new StaffSchedule
                    {
                        Id = Guid.NewGuid(),
                        StaffMemberId = staff.Id,
                        DayOfWeek = day,
                        StartTime = new TimeOnly(startHour, 0),
                        EndTime = new TimeOnly(endHour, 0),
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    };

                    schedules.Add(schedule);
                }
            }
        }

        return (staffMembers, schedules);
    }

    private static List<TimeSlot> GenerateTimeSlots(List<StaffMember> staffMembers, List<StaffSchedule> schedules)
    {
        var timeSlots = new List<TimeSlot>();
        var today = DateOnly.FromDateTime(DateTime.Today);
        var endDate = today.AddDays(14); // Generate slots for next 2 weeks

        foreach (var staff in staffMembers)
        {
            var staffSchedules = schedules.Where(s => s.StaffMemberId == staff.Id).ToList();

            for (var date = today; date <= endDate; date = date.AddDays(1))
            {
                var dayOfWeek = date.DayOfWeek;
                var daySchedules = staffSchedules.Where(s => s.DayOfWeek == dayOfWeek && s.IsActive).ToList();

                foreach (var schedule in daySchedules)
                {
                    var currentTime = schedule.StartTime;
                    var slotDuration = 30; // 30-minute slots

                    while (currentTime.AddMinutes(slotDuration) <= schedule.EndTime)
                    {
                        var slot = new TimeSlot
                        {
                            Id = Guid.NewGuid(),
                            StaffMemberId = staff.Id,
                            Date = date,
                            StartTime = currentTime,
                            EndTime = currentTime.AddMinutes(slotDuration),
                            IsAvailable = true,
                            CreatedAt = DateTime.UtcNow,
                            UpdatedAt = DateTime.UtcNow
                        };

                        timeSlots.Add(slot);
                        currentTime = currentTime.AddMinutes(slotDuration);
                    }
                }
            }
        }

        return timeSlots;
    }
}
