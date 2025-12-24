using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EmptySlot.API.Data;

namespace EmptySlot.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoriesController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public CategoriesController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetCategories()
    {
        var categories = await _context.ServiceCategories
            .Where(c => c.ParentId == null)
            .Include(c => c.Children)
            .OrderBy(c => c.DisplayOrder)
            .ToListAsync();

        return Ok(categories);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetCategory(Guid id)
    {
        var category = await _context.ServiceCategories
            .Include(c => c.Children)
            .Include(c => c.Parent)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (category == null)
        {
            return NotFound();
        }

        return Ok(category);
    }
}
