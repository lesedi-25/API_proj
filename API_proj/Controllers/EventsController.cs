
using API_proj.Models;
using API_proj.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API_proj.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EventsController : ControllerBase
    {
        private readonly ApplicationDBContext _context;

        public EventsController(ApplicationDBContext context)
        {
            _context = context;
        }

        // GET: api/Events
        // View all events.
        [HttpGet]
        public async Task<IActionResult> GetEvents()
        {
            var events = await _context.Events.ToListAsync();

            return Ok(events);
        }

        // GET: api/Events/1
        // View one event by its ID.
        [HttpGet("{id}")]
        public async Task<IActionResult> GetEvent(int id)
        {
            var eventItem = await _context.Events.FindAsync(id);

            if (eventItem == null)
            {
                return NotFound(new
                {
                    message = "Event not found."
                });
            }

            return Ok(eventItem);
        }

        // POST: api/Events
        // Create an event.
        [HttpPost]
        public async Task<IActionResult> CreateEvent(Events eventItem)
        {
            // Check that the organiser exists and has the correct role.
            var organiser = await _context.Users
                .FirstOrDefaultAsync(u =>
                    u.UserId == eventItem.OrganiserId &&
                    u.Role == "Organiser");

            if (organiser == null)
            {
                return BadRequest(new
                {
                    message = "A valid Organiser account is required."
                });
            }

            // Check that the event type is supported.
            string[] validTypes = { "Run", "Walk", "Cycle" };

            if (!validTypes.Contains(eventItem.EventType,
                    StringComparer.OrdinalIgnoreCase))
            {
                return BadRequest(new
                {
                    message = "EventType must be Run, Walk or Cycle."
                });
            }

            // Store the event type in a consistent format.
            eventItem.EventType = validTypes.First(t =>
                t.Equals(eventItem.EventType,
                    StringComparison.OrdinalIgnoreCase));

            if (eventItem.DistanceKm <= 0)
            {
                return BadRequest(new
                {
                    message = "DistanceKm must be greater than zero."
                });
            }

            if (eventItem.EntryFee < 0)
            {
                return BadRequest(new
                {
                    message = "EntryFee cannot be negative."
                });
            }

            _context.Events.Add(eventItem);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetEvent),
                new { id = eventItem.EventId },
                eventItem);
        }

        // PUT: api/Events/1
        // Update an existing event.
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEvent(
            int id, Events updatedEvent)
        {
            if (id != updatedEvent.EventId)
            {
                return BadRequest(new
                {
                    message = "The event ID in the URL and body must match."
                });
            }

            var existingEvent = await _context.Events.FindAsync(id);

            if (existingEvent == null)
            {
                return NotFound(new
                {
                    message = "Event not found."
                });
            }

            // Validate the organiser assigned to the event.
            var organiser = await _context.Users
                .FirstOrDefaultAsync(u =>
                    u.UserId == updatedEvent.OrganiserId &&
                    u.Role == "Organiser");

            if (organiser == null)
            {
                return BadRequest(new
                {
                    message = "A valid Organiser account is required."
                });
            }

            string[] validTypes = { "Run", "Walk", "Cycle" };

            if (!validTypes.Contains(updatedEvent.EventType,
                    StringComparer.OrdinalIgnoreCase))
            {
                return BadRequest(new
                {
                    message = "EventType must be Run, Walk or Cycle."
                });
            }

            if (updatedEvent.DistanceKm <= 0 ||
                updatedEvent.EntryFee < 0)
            {
                return BadRequest(new
                {
                    message = "Distance must be positive and entry fee cannot be negative."
                });
            }

            updatedEvent.EventType = validTypes.First(t =>
                t.Equals(updatedEvent.EventType,
                    StringComparison.OrdinalIgnoreCase));

            _context.Entry(existingEvent)
                .CurrentValues.SetValues(updatedEvent);

            await _context.SaveChangesAsync();

            return Ok(existingEvent);
        }

        // DELETE: api/Events/1
        // Delete an event.
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEvent(int id)
        {
            var eventItem = await _context.Events.FindAsync(id);

            if (eventItem == null)
            {
                return NotFound(new
                {
                    message = "Event not found."
                });
            }

            _context.Events.Remove(eventItem);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Event deleted successfully."
            });
        }
    }
}
