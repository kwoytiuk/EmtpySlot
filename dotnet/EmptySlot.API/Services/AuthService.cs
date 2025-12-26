using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BCrypt.Net;
using EmptySlot.API.Data;
using EmptySlot.Shared.Models;
using EmptySlot.Shared.Enums;
using Microsoft.EntityFrameworkCore;

namespace EmptySlot.API.Services;

public class AuthService
{
    private readonly ApplicationDbContext _context;
    private readonly IConfiguration _configuration;

    public AuthService(ApplicationDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    public async Task<(bool Success, string? Token, Profile? Profile, string? Error)> RegisterAsync(
        string email, string password, string fullName)
    {
        // Check if user exists by email
        var existingProfile = await _context.Profiles.FirstOrDefaultAsync(p => p.Email == email);
        if (existingProfile != null)
        {
            return (false, null, null, "User already exists");
        }

        // Hash password with BCrypt
        var passwordHash = BCrypt.Net.BCrypt.HashPassword(password);

        // Create new profile
        var profile = new Profile
        {
            Id = Guid.NewGuid(),
            Email = email,
            PasswordHash = passwordHash,
            FullName = fullName,
            UserType = UserType.Customer,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Profiles.Add(profile);
        await _context.SaveChangesAsync();

        // Generate JWT token
        var token = GenerateJwtToken(profile);

        return (true, token, profile, null);
    }

    public async Task<(bool Success, string? Token, Profile? Profile, string? Error)> LoginAsync(
        string email, string password)
    {
        // Find user by email
        var profile = await _context.Profiles.FirstOrDefaultAsync(p => p.Email == email);
        if (profile == null)
        {
            return (false, null, null, "Invalid credentials");
        }

        // Verify password with BCrypt
        if (!BCrypt.Net.BCrypt.Verify(password, profile.PasswordHash))
        {
            return (false, null, null, "Invalid credentials");
        }

        // Generate JWT token
        var token = GenerateJwtToken(profile);

        return (true, token, profile, null);
    }

    private string GenerateJwtToken(Profile profile)
    {
        var jwtSettings = _configuration.GetSection("JwtSettings");
        var secretKey = jwtSettings["SecretKey"]!;
        var issuer = jwtSettings["Issuer"]!;
        var audience = jwtSettings["Audience"]!;
        var expirationMinutes = int.Parse(jwtSettings["ExpirationMinutes"]!);

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, profile.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Name, profile.FullName ?? ""),
            new Claim(ClaimTypes.Role, profile.UserType.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(expirationMinutes),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
