
using API_proj.Data;
using API_proj.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API_proj.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EnrolmentsController : ControllerBase
    {
        private readonly ApplicationDBContext _context;

        public EnrolmentsController(ApplicationDBContext context)
        {
            _context = context;
        }

        // GET: api/Enrolments
        [HttpGet]
        public async Task<IActionResult> GetEnrolments()
        {
            var enrolments = await _context.Enrolments.ToListAsync();

            return Ok(enrolments);
        }

        // GET: api/Enrolments/1
        [HttpGet("{id}")]
        public async Task<IActionResult> GetEnrolment(int id)
        {
            var enrolment = await _context.Enrolments.FindAsync(id);

            if (enrolment == null)
            {
                return NotFound(new
                {
                    message = "Enrolment not found."
                });
            }

            return Ok(enrolment);
        }

        // GET: api/Enrolments/participant/1
        [HttpGet("participant/{participantId}")]
        public async Task<IActionResult> GetParticipantEnrolments(
            int participantId)
        {
            var participantExists = await _context.Users
                .AnyAsync(u => u.UserId == participantId &&
                               u.Role == "Participant");

            if (!participantExists)
            {
                return NotFound(new
                {
                    message = "Participant not found."
                });
            }

            var enrolments = await _context.Enrolments
                .Where(e => e.ParticipantId == participantId)
                .ToListAsync();

            return Ok(enrolments);
        }

        // POST: api/Enrolments
        [HttpPost]
        public async Task<IActionResult> CreateEnrolment(
            Enrolments enrolment)
        {
            var participantExists = await _context.Users
                .AnyAsync(u => u.UserId == enrolment.ParticipantId &&
                               u.Role == "Participant");

            if (!participantExists)
            {
                return BadRequest(new
                {
                    message = "A valid Participant account is required."
                });
            }

            var eventExists = await _context.Events
                .AnyAsync(e => e.EventId == enrolment.EventId);

            if (!eventExists)
            {
                return BadRequest(new
                {
                    message = "The specified event does not exist."
                });
            }

            var category = await _context.Categories
                .FirstOrDefaultAsync(c =>
                    c.CategoryId == enrolment.CategoryId &&
                    c.EventId == enrolment.EventId);

            if (category == null)
            {
                return BadRequest(new
                {
                    message = "The selected category does not belong to this event."
                });
            }

            bool alreadyEnrolled = await _context.Enrolments
                .AnyAsync(e =>
                    e.ParticipantId == enrolment.ParticipantId &&
                    e.EventId == enrolment.EventId);

            if (alreadyEnrolled)
            {
                return Conflict(new
                {
                    message = "This participant is already enrolled in this event."
                });
            }

            enrolment.EnrolmentId = 0;
            enrolment.EnrolmentDate = DateTime.UtcNow;
            enrolment.Status = "Active";

            _context.Enrolments.Add(enrolment);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetEnrolment),
                new { id = enrolment.EnrolmentId },
                enrolment);
        }

        // PATCH: api/Enrolments/1/status
        [HttpPatch("{id}/status")]
        public async Task<IActionResult> UpdateEnrolmentStatus(
            int id, [FromBody] string status)
        {
            string[] validStatuses =
            {
                "Active", "Cancelled", "Completed"
            };

            string? matchedStatus = validStatuses.FirstOrDefault(s =>
                s.Equals(status?.Trim(),
                    StringComparison.OrdinalIgnoreCase));

            if (matchedStatus == null)
            {
                return BadRequest(new
                {
                    message = "Status must be Active, Cancelled or Completed."
                });
            }

            var enrolment = await _context.Enrolments.FindAsync(id);

            if (enrolment == null)
            {
                return NotFound(new
                {
                    message = "Enrolment not found."
                });
            }

            enrolment.Status = matchedStatus;
            await _context.SaveChangesAsync();

            return Ok(enrolment);
        }

        // DELETE: api/Enrolments/1
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEnrolment(int id)
        {
            var enrolment = await _context.Enrolments.FindAsync(id);

            if (enrolment == null)
            {
                return NotFound(new
                {
                    message = "Enrolment not found."
                });
            }

            _context.Enrolments.Remove(enrolment);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Enrolment deleted successfully."
            });
        }
    }
}
