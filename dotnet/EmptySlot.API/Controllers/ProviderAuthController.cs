using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EmptySlot.API.Data;
using EmptySlot.Shared.Models;
using EmptySlot.Shared.Enums;
using BCrypt.Net;

namespace EmptySlot.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProviderAuthController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public ProviderAuthController(ApplicationDbContext context)
    {
        _context = context;
    }

    // POST: api/providerauth/register
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] ProviderRegistrationDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        // Check if email already exists
        var existingProfile = await _context.Profiles
            .FirstOrDefaultAsync(p => p.Email == dto.Email);

        if (existingProfile != null)
            return BadRequest("Email already registered");

        // Create profile
        var profile = new Profile
        {
            Id = Guid.NewGuid(),
            UserType = UserType.Provider,
            FullName = dto.FullName,
            Email = dto.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Profiles.Add(profile);
        await _context.SaveChangesAsync();

        // Create provider
        var provider = new Provider
        {
            Id = Guid.NewGuid(),
            UserId = profile.Id,
            BusinessName = dto.BusinessName,
            Description = dto.Description,
            Email = dto.Email,
            Phone = dto.Phone,
            RatingAverage = 0,
            RatingCount = 0,
            Verified = false,
            IsFeatured = false,
            CreatedAt = DateTime.UtcNow
        };

        _context.Providers.Add(provider);
        await _context.SaveChangesAsync();

        // Add primary location if provided
        if (!string.IsNullOrEmpty(dto.AddressLine1) && !string.IsNullOrEmpty(dto.City))
        {
            var location = new ProviderLocation
            {
                Id = Guid.NewGuid(),
                ProviderId = provider.Id,
                Name = "Primary Location",
                AddressLine1 = dto.AddressLine1,
                AddressLine2 = dto.AddressLine2,
                City = dto.City,
                StateProvince = dto.StateProvince ?? "AB",
                PostalCode = dto.PostalCode,
                Country = "Canada",
                Latitude = dto.Latitude ?? 51.0447,
                Longitude = dto.Longitude ?? -114.0719,
                IsPrimary = true
            };

            _context.ProviderLocations.Add(location);
            await _context.SaveChangesAsync();
        }

        return Ok(new
        {
            profileId = profile.Id,
            providerId = provider.Id,
            message = "Provider registered successfully"
        });
    }

    // POST: api/providerauth/login
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var profile = await _context.Profiles
            .FirstOrDefaultAsync(p => p.Email == dto.Email && p.UserType == UserType.Provider);

        if (profile == null)
            return Unauthorized("Invalid email or password");

        if (!BCrypt.Net.BCrypt.Verify(dto.Password, profile.PasswordHash))
            return Unauthorized("Invalid email or password");

        var provider = await _context.Providers
            .Include(p => p.Locations)
            .Include(p => p.Services)
            .Include(p => p.StaffMembers)
            .FirstOrDefaultAsync(p => p.UserId == profile.Id);

        if (provider == null)
            return NotFound("Provider account not found");

        return Ok(new
        {
            profileId = profile.Id,
            providerId = provider.Id,
            fullName = profile.FullName,
            email = profile.Email,
            businessName = provider.BusinessName,
            provider = provider
        });
    }
}

public record ProviderRegistrationDto(
    string FullName,
    string Email,
    string Password,
    string BusinessName,
    string? Description,
    string Phone,
    string? AddressLine1,
    string? AddressLine2,
    string? City,
    string? StateProvince,
    string? PostalCode,
    double? Latitude,
    double? Longitude);

public record LoginDto(
    string Email,
    string Password);
