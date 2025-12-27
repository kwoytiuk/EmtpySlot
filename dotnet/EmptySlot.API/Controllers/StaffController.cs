using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EmptySlot.API.Data;
using EmptySlot.Shared.Models;

namespace EmptySlot.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StaffController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public StaffController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: api/staff/provider/{providerId}
    [HttpGet("provider/{providerId}")]
    public async Task<IActionResult> GetStaffByProvider(Guid providerId)
    {
        var staff = await _context.StaffMembers
            .Include(s => s.Schedules)
            .Include(s => s.TimeSlots)
            .Where(s => s.ProviderId == providerId)
            .OrderBy(s => s.Name)
            .ToListAsync();

        return Ok(staff);
    }

    // GET: api/staff/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetStaffMember(Guid id)
    {
        var staff = await _context.StaffMembers
            .Include(s => s.Schedules)
            .Include(s => s.TimeSlots)
            .FirstOrDefaultAsync(s => s.Id == id);

        if (staff == null)
            return NotFound();

        return Ok(staff);
    }

    // POST: api/staff
    [HttpPost]
    public async Task<IActionResult> CreateStaffMember([FromBody] StaffMember staff)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        staff.Id = Guid.NewGuid();
        staff.CreatedAt = DateTime.UtcNow;
        staff.UpdatedAt = DateTime.UtcNow;

        _context.StaffMembers.Add(staff);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetStaffMember), new { id = staff.Id }, staff);
    }

    // PUT: api/staff/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateStaffMember(Guid id, [FromBody] StaffMember staff)
    {
        if (id != staff.Id)
            return BadRequest("ID mismatch");

        var existingStaff = await _context.StaffMembers.FindAsync(id);
        if (existingStaff == null)
            return NotFound();

        existingStaff.Name = staff.Name;
        existingStaff.Email = staff.Email;
        existingStaff.Phone = staff.Phone;
        existingStaff.AvatarUrl = staff.AvatarUrl;
        existingStaff.Bio = staff.Bio;
        existingStaff.IsActive = staff.IsActive;
        existingStaff.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return Ok(existingStaff);
    }

    // DELETE: api/staff/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteStaffMember(Guid id)
    {
        var staff = await _context.StaffMembers.FindAsync(id);
        if (staff == null)
            return NotFound();

        _context.StaffMembers.Remove(staff);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    // GET: api/staff/{staffId}/schedules
    [HttpGet("{staffId}/schedules")]
    public async Task<IActionResult> GetStaffSchedules(Guid staffId)
    {
        var schedules = await _context.StaffSchedules
            .Where(s => s.StaffMemberId == staffId && s.IsActive)
            .OrderBy(s => s.DayOfWeek)
            .ThenBy(s => s.StartTime)
            .ToListAsync();

        return Ok(schedules);
    }

    // POST: api/staff/{staffId}/schedules
    [HttpPost("{staffId}/schedules")]
    public async Task<IActionResult> CreateSchedule(Guid staffId, [FromBody] StaffSchedule schedule)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var staff = await _context.StaffMembers.FindAsync(staffId);
        if (staff == null)
            return NotFound("Staff member not found");

        schedule.Id = Guid.NewGuid();
        schedule.StaffMemberId = staffId;
        schedule.CreatedAt = DateTime.UtcNow;
        schedule.UpdatedAt = DateTime.UtcNow;

        _context.StaffSchedules.Add(schedule);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetStaffSchedules), new { staffId }, schedule);
    }

    // PUT: api/staff/schedules/{scheduleId}
    [HttpPut("schedules/{scheduleId}")]
    public async Task<IActionResult> UpdateSchedule(Guid scheduleId, [FromBody] StaffSchedule schedule)
    {
        if (scheduleId != schedule.Id)
            return BadRequest("ID mismatch");

        var existingSchedule = await _context.StaffSchedules.FindAsync(scheduleId);
        if (existingSchedule == null)
            return NotFound();

        existingSchedule.DayOfWeek = schedule.DayOfWeek;
        existingSchedule.StartTime = schedule.StartTime;
        existingSchedule.EndTime = schedule.EndTime;
        existingSchedule.IsActive = schedule.IsActive;
        existingSchedule.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return Ok(existingSchedule);
    }

    // DELETE: api/staff/schedules/{scheduleId}
    [HttpDelete("schedules/{scheduleId}")]
    public async Task<IActionResult> DeleteSchedule(Guid scheduleId)
    {
        var schedule = await _context.StaffSchedules.FindAsync(scheduleId);
        if (schedule == null)
            return NotFound();

        _context.StaffSchedules.Remove(schedule);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    // POST: api/staff/{staffId}/generate-slots
    [HttpPost("{staffId}/generate-slots")]
    public async Task<IActionResult> GenerateTimeSlots(
        Guid staffId,
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null,
        [FromQuery] int slotDurationMinutes = 30)
    {
        var staff = await _context.StaffMembers
            .Include(s => s.Schedules)
            .FirstOrDefaultAsync(s => s.Id == staffId);

        if (staff == null)
            return NotFound("Staff member not found");

        var start = DateOnly.FromDateTime(startDate ?? DateTime.Today);
        var end = DateOnly.FromDateTime(endDate ?? DateTime.Today.AddDays(14));

        var generatedSlots = new List<TimeSlot>();

        for (var date = start; date <= end; date = date.AddDays(1))
        {
            var dayOfWeek = date.DayOfWeek;
            var schedules = staff.Schedules
                .Where(s => s.DayOfWeek == dayOfWeek && s.IsActive)
                .ToList();

            foreach (var schedule in schedules)
            {
                var currentTime = schedule.StartTime;
                while (currentTime < schedule.EndTime)
                {
                    var endTime = currentTime.AddMinutes(slotDurationMinutes);
                    if (endTime > schedule.EndTime)
                        break;

                    // Check if slot already exists
                    var existingSlot = await _context.TimeSlots
                        .FirstOrDefaultAsync(ts =>
                            ts.StaffMemberId == staffId &&
                            ts.Date == date &&
                            ts.StartTime == currentTime &&
                            ts.EndTime == endTime);

                    if (existingSlot == null)
                    {
                        var slot = new TimeSlot
                        {
                            Id = Guid.NewGuid(),
                            StaffMemberId = staffId,
                            Date = date,
                            StartTime = currentTime,
                            EndTime = endTime,
                            IsAvailable = true,
                            CreatedAt = DateTime.UtcNow,
                            UpdatedAt = DateTime.UtcNow
                        };

                        _context.TimeSlots.Add(slot);
                        generatedSlots.Add(slot);
                    }

                    currentTime = endTime;
                }
            }
        }

        await _context.SaveChangesAsync();

        return Ok(new {
            message = $"Generated {generatedSlots.Count} time slots",
            slots = generatedSlots.Count
        });
    }
}
