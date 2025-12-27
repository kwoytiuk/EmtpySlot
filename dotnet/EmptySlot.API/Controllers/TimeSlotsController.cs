using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EmptySlot.API.Data;
using EmptySlot.Shared.Models;

namespace EmptySlot.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TimeSlotsController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public TimeSlotsController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: api/timeslots/provider/{providerId}/available
    [HttpGet("provider/{providerId}/available")]
    public async Task<IActionResult> GetAvailableSlots(
        Guid providerId,
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null)
    {
        var start = DateOnly.FromDateTime(startDate ?? DateTime.Today);
        var end = DateOnly.FromDateTime(endDate ?? DateTime.Today.AddDays(7));

        var slots = await _context.TimeSlots
            .Include(ts => ts.Staff)
            .Where(ts => ts.Staff.ProviderId == providerId 
                      && ts.Date >= start 
                      && ts.Date <= end
                      && ts.IsAvailable)
            .OrderBy(ts => ts.Date)
            .ThenBy(ts => ts.StartTime)
            .Select(ts => new
            {
                ts.Id,
                ts.Date,
                ts.StartTime,
                ts.EndTime,
                StaffMemberName = ts.Staff.Name,
                DisplayTime = ts.StartTime.ToString("h:mm tt")
            })
            .ToListAsync();

        return Ok(slots);
    }

    // POST: api/timeslots/{id}/block
    [HttpPost("{id}/block")]
    public async Task<IActionResult> BlockTimeSlot(Guid id)
    {
        var slot = await _context.TimeSlots.FindAsync(id);
        if (slot == null)
            return NotFound();

        slot.IsAvailable = false;
        slot.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return Ok();
    }

    // POST: api/timeslots/{id}/unblock
    [HttpPost("{id}/unblock")]
    public async Task<IActionResult> UnblockTimeSlot(Guid id)
    {
        var slot = await _context.TimeSlots.FindAsync(id);
        if (slot == null)
            return NotFound();

        if (slot.AppointmentId.HasValue)
            return BadRequest("Cannot unblock a slot with an active appointment");

        slot.IsAvailable = true;
        slot.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return Ok();
    }
}
