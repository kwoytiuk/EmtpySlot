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
    }

    private static List<ServiceCategory> CreateCategories()
    {
        return new List<ServiceCategory>
        {
            new ServiceCategory
            {
                Id = Guid.NewGuid(),
                Name = "Restaurants",
                Slug = "restaurants",
                Icon = "🍽️",
                DisplayOrder = 1,
                CreatedAt = DateTime.UtcNow
            },
            new ServiceCategory
            {
                Id = Guid.NewGuid(),
                Name = "Hair Salons",
                Slug = "hair-salons",
                Icon = "✂️",
                DisplayOrder = 2,
                CreatedAt = DateTime.UtcNow
            },
            new ServiceCategory
            {
                Id = Guid.NewGuid(),
                Name = "Spa & Wellness",
                Slug = "spa-wellness",
                Icon = "💆",
                DisplayOrder = 3,
                CreatedAt = DateTime.UtcNow
            },
            new ServiceCategory
            {
                Id = Guid.NewGuid(),
                Name = "Nail Salons",
                Slug = "nail-salons",
                Icon = "💅",
                DisplayOrder = 4,
                CreatedAt = DateTime.UtcNow
            },
            new ServiceCategory
            {
                Id = Guid.NewGuid(),
                Name = "Fitness & Gym",
                Slug = "fitness-gym",
                Icon = "💪",
                DisplayOrder = 5,
                CreatedAt = DateTime.UtcNow
            },
            new ServiceCategory
            {
                Id = Guid.NewGuid(),
                Name = "Medical & Dental",
                Slug = "medical-dental",
                Icon = "🏥",
                DisplayOrder = 6,
                CreatedAt = DateTime.UtcNow
            },
            new ServiceCategory
            {
                Id = Guid.NewGuid(),
                Name = "Beauty & Esthetics",
                Slug = "beauty-esthetics",
                Icon = "✨",
                DisplayOrder = 7,
                CreatedAt = DateTime.UtcNow
            },
            new ServiceCategory
            {
                Id = Guid.NewGuid(),
                Name = "Automotive",
                Slug = "automotive",
                Icon = "🚗",
                DisplayOrder = 8,
                CreatedAt = DateTime.UtcNow
            }
        };
    }

    private static List<Provider> CreateProviders(List<ServiceCategory> categories)
    {
        var providers = new List<Provider>();
        var random = new Random(42); // Seed for consistent data

        // Get category references
        var restaurantCat = categories.First(c => c.Slug == "restaurants");
        var hairCat = categories.First(c => c.Slug == "hair-salons");
        var spaCat = categories.First(c => c.Slug == "spa-wellness");
        var nailsCat = categories.First(c => c.Slug == "nail-salons");
        var fitnessCat = categories.First(c => c.Slug == "fitness-gym");
        var medicalCat = categories.First(c => c.Slug == "medical-dental");
        var beautyCat = categories.First(c => c.Slug == "beauty-esthetics");
        var autoCat = categories.First(c => c.Slug == "automotive");

        // Calgary Restaurants (30)
        providers.AddRange(CreateRestaurants(restaurantCat, "Calgary", 51.0447, -114.0719, 30, random));

        // Edmonton Restaurants (20)
        providers.AddRange(CreateRestaurants(restaurantCat, "Edmonton", 53.5461, -113.4938, 20, random));

        // Red Deer Restaurants (10)
        providers.AddRange(CreateRestaurants(restaurantCat, "Red Deer", 52.2681, -113.8111, 10, random));

        // Calgary Hair Salons (15)
        providers.AddRange(CreateHairSalons(hairCat, "Calgary", 51.0447, -114.0719, 15, random));

        // Edmonton Hair Salons (10)
        providers.AddRange(CreateHairSalons(hairCat, "Edmonton", 53.5461, -113.4938, 10, random));

        // Calgary Spas (10)
        providers.AddRange(CreateSpas(spaCat, "Calgary", 51.0447, -114.0719, 10, random));

        // Edmonton Spas (5)
        providers.AddRange(CreateSpas(spaCat, "Edmonton", 53.5461, -113.4938, 5, random));

        // Nail Salons (10 total)
        providers.AddRange(CreateNailSalons(nailsCat, "Calgary", 51.0447, -114.0719, 6, random));
        providers.AddRange(CreateNailSalons(nailsCat, "Edmonton", 53.5461, -113.4938, 4, random));

        // Fitness Centers (8 total)
        providers.AddRange(CreateFitnessCenters(fitnessCat, "Calgary", 51.0447, -114.0719, 5, random));
        providers.AddRange(CreateFitnessCenters(fitnessCat, "Edmonton", 53.5461, -113.4938, 3, random));

        return providers;
    }

    private static List<Provider> CreateRestaurants(ServiceCategory category, string city, double baseLat, double baseLng, int count, Random random)
    {
        var restaurants = new List<Provider>();
        var restaurantNames = new[]
        {
            "The Keg Steakhouse", "Cactus Club Cafe", "Earls Kitchen + Bar", "Moxies Grill & Bar",
            "State & Main", "Original Joe's", "The Sawmill", "Craft Beer Market", "Vintage Chophouse",
            "River Cafe", "Teatro Restaurant", "Sky 360", "Charbar Restaurant", "Model Milk",
            "Bridgette Bar", "Major Tom", "Foreign Concept", "Nupo", "Gaucho Brazilian BBQ",
            "Sukiyaki House", "Jinzakaya", "Anju Restaurant", "Ten Foot Henry", "Pigeonhole",
            "The Nash", "Shokunin", "Mercato", "Calcutta Cricket Club", "Alloy Dining",
            "Bow Valley Ranche", "Q Haute Cuisine", "Rouge Restaurant", "Deane House",
            "Bow Valley BBQ", "Sushi Hiro", "Wa's Japanese Restaurant", "Zipang Sushi"
        };

        var cuisineTypes = new[]
        {
            "Steakhouse", "Contemporary", "American", "Italian", "Japanese", "French",
            "Mediterranean", "Asian Fusion", "Brazilian", "Indian", "Vietnamese"
        };

        for (int i = 0; i < count && i < restaurantNames.Length; i++)
        {
            var name = restaurantNames[i] + (city != "Calgary" ? $" {city}" : "");
            var cuisine = cuisineTypes[random.Next(cuisineTypes.Length)];
            var rating = 3.5m + (decimal)(random.NextDouble() * 1.5);
            var reviewCount = random.Next(50, 3000);
            var priceLevel = random.Next(1, 5);
            var isFeatured = i < count / 3; // Top third are featured

            var provider = new Provider
            {
                Id = Guid.NewGuid(),
                UserId = Guid.NewGuid(),
                BusinessName = name,
                Description = $"Experience {cuisine} cuisine at its finest. {name} offers an unforgettable dining experience with fresh, locally-sourced ingredients and exceptional service.",
                Email = $"info@{name.ToLower().Replace(" ", "").Replace("&", "and")}.ca",
                Phone = $"+1 ({GetAreaCode(city)}) {random.Next(200, 999)}-{random.Next(1000, 9999)}",
                Website = $"https://{name.ToLower().Replace(" ", "")}.ca",
                RatingAverage = Math.Round(rating, 1),
                RatingCount = reviewCount,
                Verified = random.Next(100) > 20, // 80% verified
                IsFeatured = isFeatured,
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
                        Phone = $"+1 ({GetAreaCode(city)}) {random.Next(200, 999)}-{random.Next(1000, 9999)}",
                        Latitude = baseLat + (random.NextDouble() - 0.5) * 0.1,
                        Longitude = baseLng + (random.NextDouble() - 0.5) * 0.1,
                        IsPrimary = true
                    }
                },
                Services = new List<Service>
                {
                    new Service
                    {
                        Id = Guid.NewGuid(),
                        Name = "Dinner Reservation",
                        Description = $"Reserve your table for an exceptional {cuisine} dining experience",
                        CategoryId = category.Id,
                        DurationMinutes = 120,
                        Price = priceLevel * 25.00m,
                        DepositRequired = priceLevel * 10.00m,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    },
                    new Service
                    {
                        Id = Guid.NewGuid(),
                        Name = "Lunch Reservation",
                        Description = "Enjoy our lunch menu in a relaxed atmosphere",
                        CategoryId = category.Id,
                        DurationMinutes = 90,
                        Price = priceLevel * 18.00m,
                        DepositRequired = 0m,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    }
                }
            };

            restaurants.Add(provider);
        }

        return restaurants;
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
                UserId = Guid.NewGuid(),
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
                    new Service { Id = Guid.NewGuid(), Name = "Women's Cut & Style", CategoryId = category.Id, DurationMinutes = 60, Price = 85.00m, IsActive = true },
                    new Service { Id = Guid.NewGuid(), Name = "Men's Cut", CategoryId = category.Id, DurationMinutes = 30, Price = 45.00m, IsActive = true },
                    new Service { Id = Guid.NewGuid(), Name = "Color Treatment", CategoryId = category.Id, DurationMinutes = 120, Price = 150.00m, DepositRequired = 50.00m, IsActive = true },
                    new Service { Id = Guid.NewGuid(), Name = "Balayage/Highlights", CategoryId = category.Id, DurationMinutes = 180, Price = 250.00m, DepositRequired = 75.00m, IsActive = true }
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
                UserId = Guid.NewGuid(),
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
                    new Service { Id = Guid.NewGuid(), Name = "Swedish Massage (60min)", CategoryId = category.Id, DurationMinutes = 60, Price = 120.00m, IsActive = true },
                    new Service { Id = Guid.NewGuid(), Name = "Deep Tissue Massage (90min)", CategoryId = category.Id, DurationMinutes = 90, Price = 165.00m, IsActive = true },
                    new Service { Id = Guid.NewGuid(), Name = "Facial Treatment", CategoryId = category.Id, DurationMinutes = 75, Price = 140.00m, IsActive = true },
                    new Service { Id = Guid.NewGuid(), Name = "Body Scrub & Wrap", CategoryId = category.Id, DurationMinutes = 90, Price = 180.00m, DepositRequired = 60.00m, IsActive = true }
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
                UserId = Guid.NewGuid(),
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
                    new Service { Id = Guid.NewGuid(), Name = "Gel Manicure", CategoryId = category.Id, DurationMinutes = 45, Price = 55.00m, IsActive = true },
                    new Service { Id = Guid.NewGuid(), Name = "Spa Pedicure", CategoryId = category.Id, DurationMinutes = 60, Price = 70.00m, IsActive = true },
                    new Service { Id = Guid.NewGuid(), Name = "Acrylic Full Set", CategoryId = category.Id, DurationMinutes = 90, Price = 85.00m, IsActive = true }
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
                UserId = Guid.NewGuid(),
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
                    new Service { Id = Guid.NewGuid(), Name = "Personal Training Session", CategoryId = category.Id, DurationMinutes = 60, Price = 95.00m, IsActive = true },
                    new Service { Id = Guid.NewGuid(), Name = "Group Fitness Class", CategoryId = category.Id, DurationMinutes = 45, Price = 25.00m, IsActive = true },
                    new Service { Id = Guid.NewGuid(), Name = "Fitness Assessment", CategoryId = category.Id, DurationMinutes = 30, Price = 50.00m, IsActive = true }
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
}
