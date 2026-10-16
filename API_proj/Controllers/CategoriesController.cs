
using API_proj.Models;
using API_proj.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API_proj.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriesController : ControllerBase
    {
        private readonly ApplicationDBContext _context;

        public CategoriesController(ApplicationDBContext context)
        {
            _context = context;
        }

        // GET: api/Categories
        // View all categories.
        [HttpGet]
        public async Task<IActionResult> GetCategories()
        {
            var categories = await _context.Categories.ToListAsync();

            return Ok(categories);
        }

        // GET: api/Categories/1
        // View a category by ID.
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCategory(int id)
        {
            var category = await _context.Categories.FindAsync(id);

            if (category == null)
            {
                return NotFound(new
                {
                    message = "Category not found."
                });
            }

            return Ok(category);
        }

        // GET: api/Categories/event/1
        // View all categories for a specific event.
        [HttpGet("event/{eventId}")]
        public async Task<IActionResult> GetCategoriesByEvent(int eventId)
        {
            var eventExists = await _context.Events
                .AnyAsync(e => e.EventId == eventId);

            if (!eventExists)
            {
                return NotFound(new
                {
                    message = "Event not found."
                });
            }

            var categories = await _context.Categories
                .Where(c => c.EventId == eventId)
                .ToListAsync();

            return Ok(categories);
        }

        // POST: api/Categories
        // Create a category for an existing event.
        [HttpPost]
        public async Task<IActionResult> CreateCategory(
            Categories category)
        {
            var eventExists = await _context.Events
                .AnyAsync(e => e.EventId == category.EventId);

            if (!eventExists)
            {
                return BadRequest(new
                {
                    message = "The specified event does not exist."
                });
            }

            if (category.MinimumAge < 0 ||
                category.MaximumAge < 0)
            {
                return BadRequest(new
                {
                    message = "Age limits cannot be negative."
                });
            }

            if (category.MinimumAge.HasValue &&
                category.MaximumAge.HasValue &&
                category.MinimumAge > category.MaximumAge)
            {
                return BadRequest(new
                {
                    message = "Minimum age cannot exceed maximum age."
                });
            }

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetCategory),
                new { id = category.CategoryId },
                category);
        }

        // PUT: api/Categories/1
        // Update an existing category.
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCategory(
            int id, Categories updatedCategory)
        {
            if (id != updatedCategory.CategoryId)
            {
                return BadRequest(new
                {
                    message = "The category ID in the URL and body must match."
                });
            }

            var existingCategory = await _context.Categories
                .FindAsync(id);

            if (existingCategory == null)
            {
                return NotFound(new
                {
                    message = "Category not found."
                });
            }

            var eventExists = await _context.Events
                .AnyAsync(e => e.EventId == updatedCategory.EventId);

            if (!eventExists)
            {
                return BadRequest(new
                {
                    message = "The specified event does not exist."
                });
            }

            if (updatedCategory.MinimumAge < 0 ||
                updatedCategory.MaximumAge < 0)
            {
                return BadRequest(new
                {
                    message = "Age limits cannot be negative."
                });
            }

            if (updatedCategory.MinimumAge.HasValue &&
                updatedCategory.MaximumAge.HasValue &&
                updatedCategory.MinimumAge > updatedCategory.MaximumAge)
            {
                return BadRequest(new
                {
                    message = "Minimum age cannot exceed maximum age."
                });
            }

            _context.Entry(existingCategory)
                .CurrentValues.SetValues(updatedCategory);

            await _context.SaveChangesAsync();

            return Ok(existingCategory);
        }

        // DELETE: api/Categories/1
        // Delete a category.
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var category = await _context.Categories.FindAsync(id);

            if (category == null)
            {
                return NotFound(new
                {
                    message = "Category not found."
                });
            }

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Category deleted successfully."
            });
        }
    }
}
