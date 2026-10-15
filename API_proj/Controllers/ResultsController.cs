
using API_proj.Data;
using API_proj.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API_proj.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ResultsController : ControllerBase
    {
        private readonly ApplicationDBContext _context;

        public ResultsController(ApplicationDBContext context)
        {
            _context = context;
        }

        // GET: api/Results
        // View all results.
        [HttpGet]
        public async Task<IActionResult> GetResults()
        {
            var results = await _context.Results.ToListAsync();

            return Ok(results);
        }

        // GET: api/Results/1
        // View a result by ID.
        [HttpGet("{id}")]
        public async Task<IActionResult> GetResult(int id)
        {
            var result = await _context.Results.FindAsync(id);

            if (result == null)
            {
                return NotFound(new
                {
                    message = "Result not found."
                });
            }

            return Ok(result);
        }

        // GET: api/Results/enrolment/1
        // View results for an enrolment.
        [HttpGet("enrolment/{enrolmentId}")]
        public async Task<IActionResult> GetResultByEnrolment(
            int enrolmentId)
        {
            var enrolmentExists = await _context.Enrolments
                .AnyAsync(e => e.EnrolmentId == enrolmentId);

            if (!enrolmentExists)
            {
                return NotFound(new
                {
                    message = "Enrolment not found."
                });
            }

            var results = await _context.Results
                .Where(r => r.EnrolmentId == enrolmentId)
                .ToListAsync();

            return Ok(results);
        }

        // POST: api/Results
        // Record a result for an enrolment.
        [HttpPost]
        public async Task<IActionResult> CreateResult(Models.Results result)
        {
            var enrolment = await _context.Enrolments
                .FirstOrDefaultAsync(e =>
                    e.EnrolmentId == result.EnrolmentId);

            if (enrolment == null)
            {
                return BadRequest(new
                {
                    message = "The specified enrolment does not exist."
                });
            }

            if (result.Position.HasValue && result.Position.Value <= 0)
            {
                return BadRequest(new
                {
                    message = "Position must be greater than zero."
                });
            }

            string[] validStatuses =
            {
                "Finished", "DidNotFinish", "Disqualified"
            };

            string? matchedStatus = validStatuses.FirstOrDefault(s =>
                s.Equals(result.Status?.Trim(),
                    StringComparison.OrdinalIgnoreCase));

            if (matchedStatus == null)
            {
                return BadRequest(new
                {
                    message = "Status must be Finished, DidNotFinish or Disqualified."
                });
            }

            result.Status = matchedStatus;
            result.ResultId = 0;

            _context.Results.Add(result);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetResult),
                new { id = result.ResultId },
                result);
        }

        // PUT: api/Results/1
        // Update an existing result.
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateResult(
            int id, Models.Results updatedResult)
        {
            if (id != updatedResult.ResultId)
            {
                return BadRequest(new
                {
                    message = "The result ID in the URL and body must match."
                });
            }

            var existingResult = await _context.Results.FindAsync(id);

            if (existingResult == null)
            {
                return NotFound(new
                {
                    message = "Result not found."
                });
            }

            var enrolmentExists = await _context.Enrolments
                .AnyAsync(e =>
                    e.EnrolmentId == updatedResult.EnrolmentId);

            if (!enrolmentExists)
            {
                return BadRequest(new
                {
                    message = "The specified enrolment does not exist."
                });
            }

            if (updatedResult.Position.HasValue &&
                updatedResult.Position.Value <= 0)
            {
                return BadRequest(new
                {
                    message = "Position must be greater than zero."
                });
            }

            string[] validStatuses =
            {
                "Finished", "DidNotFinish", "Disqualified"
            };

            string? matchedStatus = validStatuses.FirstOrDefault(s =>
                s.Equals(updatedResult.Status?.Trim(),
                    StringComparison.OrdinalIgnoreCase));

            if (matchedStatus == null)
            {
                return BadRequest(new
                {
                    message = "Status must be Finished, DidNotFinish or Disqualified."
                });
            }

            updatedResult.Status = matchedStatus;

            _context.Entry(existingResult)
                .CurrentValues.SetValues(updatedResult);

            await _context.SaveChangesAsync();

            return Ok(existingResult);
        }

        // DELETE: api/Results/1
        // Delete a result.
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteResult(int id)
        {
            var result = await _context.Results.FindAsync(id);

            if (result == null)
            {
                return NotFound(new
                {
                    message = "Result not found."
                });
            }

            _context.Results.Remove(result);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Result deleted successfully."
            });
        }
    }
}
