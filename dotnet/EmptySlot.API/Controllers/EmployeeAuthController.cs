using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EmptySlot.API.Data;
using EmptySlot.Shared.Models;

namespace EmptySlot.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EmployeeAuthController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public EmployeeAuthController(ApplicationDbContext context)
    {
        _context = context;
    }

    // POST: api/employeeauth/login
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] EmployeeLoginDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        // Find staff member by email
        var staff = await _context.StaffMembers
            .Include(s => s.Provider)
            .Include(s => s.Schedules)
            .FirstOrDefaultAsync(s => s.Email == dto.Email && s.IsActive);

        if (staff == null)
            return Unauthorized("Invalid email or password");

        // For now, use a simple password check (in production, use proper hashing)
        // Password is staff member's name without spaces, lowercase
        var expectedPassword = staff.Name.Replace(" ", "").ToLower();
        if (dto.Password != expectedPassword)
            return Unauthorized("Invalid email or password");

        return Ok(new
        {
            staffId = staff.Id,
            providerId = staff.ProviderId,
            name = staff.Name,
            email = staff.Email,
            businessName = staff.Provider.BusinessName,
            staff = staff
        });
    }
}

public record EmployeeLoginDto(
    string Email,
    string Password);
