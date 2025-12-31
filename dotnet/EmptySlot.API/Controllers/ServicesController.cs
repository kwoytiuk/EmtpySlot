using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EmptySlot.API.Data;
using EmptySlot.Shared.Models;

namespace EmptySlot.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ServicesController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public ServicesController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: api/services/provider/{providerId}
    [HttpGet("provider/{providerId}")]
    public async Task<IActionResult> GetServicesByProvider(Guid providerId)
    {
        var services = await _context.Services
            .Where(s => s.ProviderId == providerId)
            .OrderBy(s => s.Name)
            .ToListAsync();

        return Ok(services);
    }

    // GET: api/services/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetService(Guid id)
    {
        var service = await _context.Services.FindAsync(id);

        if (service == null)
            return NotFound();

        return Ok(service);
    }

    // POST: api/services
    [HttpPost]
    public async Task<IActionResult> CreateService([FromBody] CreateServiceDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var service = new Service
        {
            Id = Guid.NewGuid(),
            ProviderId = dto.ProviderId,
            CategoryId = dto.CategoryId,
            Name = dto.Name,
            Description = dto.Description,
            DurationMinutes = dto.DurationMinutes,
            Price = dto.Price,
            DepositRequired = dto.DepositRequired,
            ImageUrl = dto.ImageUrl,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        _context.Services.Add(service);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetService), new { id = service.Id }, service);
    }

    // PUT: api/services/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateService(Guid id, [FromBody] UpdateServiceDto dto)
    {
        var service = await _context.Services.FindAsync(id);
        if (service == null)
            return NotFound();

        service.Name = dto.Name;
        service.Description = dto.Description;
        service.DurationMinutes = dto.DurationMinutes;
        service.Price = dto.Price;
        service.DepositRequired = dto.DepositRequired;
        service.ImageUrl = dto.ImageUrl;
        service.IsActive = dto.IsActive;

        await _context.SaveChangesAsync();

        return Ok(service);
    }

    // DELETE: api/services/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteService(Guid id)
    {
        var service = await _context.Services.FindAsync(id);
        if (service == null)
            return NotFound();

        _context.Services.Remove(service);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}

public record CreateServiceDto(
    Guid ProviderId,
    Guid CategoryId,
    string Name,
    string? Description,
    int DurationMinutes,
    decimal Price,
    decimal? DepositRequired,
    string? ImageUrl);

public record UpdateServiceDto(
    string Name,
    string? Description,
    int DurationMinutes,
    decimal Price,
    decimal? DepositRequired,
    string? ImageUrl,
    bool IsActive);
