using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EmptySlot.API.Data;
using EmptySlot.Shared.Models;

namespace EmptySlot.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProvidersController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public ProvidersController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet("search")]
    public async Task<IActionResult> Search(
        [FromQuery] double? latitude,
        [FromQuery] double? longitude,
        [FromQuery] double? radiusKm,
        [FromQuery] Guid? categoryId,
        [FromQuery] decimal? minRating,
        [FromQuery] bool? verified,
        [FromQuery] int limit = 20,
        [FromQuery] int offset = 0)
    {
        var query = _context.Providers
            .Include(p => p.Locations)
            .Include(p => p.Services)
                .ThenInclude(s => s.Category)
            .AsQueryable();

        if (verified.HasValue)
        {
            query = query.Where(p => p.Verified == verified.Value);
        }

        if (minRating.HasValue)
        {
            query = query.Where(p => p.RatingAverage >= minRating.Value);
        }

        query = query.OrderByDescending(p => p.RatingAverage);

        var providers = await query
            .Skip(offset)
            .Take(limit)
            .ToListAsync();

        // Filter by category if specified
        if (categoryId.HasValue)
        {
            providers = providers
                .Where(p => p.Services.Any(s => s.CategoryId == categoryId.Value))
                .ToList();
        }

        // Filter by distance if location provided
        if (latitude.HasValue && longitude.HasValue && radiusKm.HasValue)
        {
            providers = providers
                .Where(p => p.Locations.Any(loc =>
                    CalculateDistance(latitude.Value, longitude.Value, loc.Latitude, loc.Longitude) <= radiusKm.Value))
                .ToList();

            // Add distance to each provider
            foreach (var provider in providers)
            {
                var closestLocation = provider.Locations
                    .Select(loc => new
                    {
                        Location = loc,
                        Distance = CalculateDistance(latitude.Value, longitude.Value, loc.Latitude, loc.Longitude)
                    })
                    .OrderBy(x => x.Distance)
                    .FirstOrDefault();

                // You can add distance as a custom property if needed
            }
        }

        return Ok(providers);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetProvider(Guid id)
    {
        var provider = await _context.Providers
            .Include(p => p.Locations)
            .Include(p => p.Services)
                .ThenInclude(s => s.Category)
            .Include(p => p.StaffMembers)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (provider == null)
        {
            return NotFound();
        }

        return Ok(provider);
    }

    [HttpGet("{id}/reviews")]
    public async Task<IActionResult> GetReviews(Guid id, [FromQuery] int limit = 10)
    {
        var reviews = await _context.Reviews
            .Where(r => r.ProviderId == id)
            .Include(r => r.Customer)
            .OrderByDescending(r => r.CreatedAt)
            .Take(limit)
            .ToListAsync();

        return Ok(reviews);
    }

    private static double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
    {
        const double R = 6371; // Earth's radius in kilometers
        var dLat = ToRad(lat2 - lat1);
        var dLon = ToRad(lon2 - lon1);

        var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Cos(ToRad(lat1)) * Math.Cos(ToRad(lat2)) *
                Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        return R * c;
    }

    private static double ToRad(double degrees) => degrees * (Math.PI / 180);
}
