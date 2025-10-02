using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EventManagementSystem.Data;
using EventManagementSystem.Models;

namespace EventManagementSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EventBookingsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public EventBookingsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/EventBookings
        [HttpGet]
        public async Task<ActionResult<IEnumerable<EventBookingDto>>> GetEventBookings()
        {
            return await _context.EventBookings
                .Select(eb => new EventBookingDto
                {
                    Id = eb.Id,
                    EventName = eb.EventName,
                    UserName = eb.UserName,
                    BookingDate = eb.BookingDate,
                    EventId = eb.EventId,
                    UserId = eb.UserId
                })
                .ToListAsync();
        }

        // GET: api/EventBookings/5
        [HttpGet("{id}")]
        public async Task<ActionResult<EventBookingDto>> GetEventBooking(int id)
        {
            var eventBooking = await _context.EventBookings
                .Where(eb => eb.Id == id)
                .Select(eb => new EventBookingDto
                {
                    Id = eb.Id,
                    EventName = eb.EventName,
                    UserName = eb.UserName,
                    BookingDate = eb.BookingDate,
                    EventId = eb.EventId,
                    UserId = eb.UserId
                })
                .FirstOrDefaultAsync();

            if (eventBooking == null)
            {
                return NotFound();
            }

            return eventBooking;
        }

        // POST: api/EventBookings
        [HttpPost]
        public async Task<ActionResult<EventBookingDto>> PostEventBooking(CreateEventBookingDto createDto)
        {
            // Validate event and user exist
            var eventItem = await _context.Events.FindAsync(createDto.EventId);
            var user = await _context.Users.FindAsync(createDto.UserId);

            if (eventItem == null || user == null)
            {
                return BadRequest("Invalid Event ID or User ID");
            }

            // Check for existing booking
            var existingBooking = await _context.EventBookings
                .FirstOrDefaultAsync(eb => eb.EventId == createDto.EventId && eb.UserId == createDto.UserId);

            if (existingBooking != null)
            {
                return BadRequest("User has already booked this event");
            }

            var eventBooking = new EventBooking
            {
                EventId = createDto.EventId,
                UserId = createDto.UserId,
                EventName = eventItem.Name,
                UserName = user.Username,
                BookingDate = DateTime.UtcNow
            };

            _context.EventBookings.Add(eventBooking);
            await _context.SaveChangesAsync();

            var resultDto = new EventBookingDto
            {
                Id = eventBooking.Id,
                EventName = eventBooking.EventName,
                UserName = eventBooking.UserName,
                BookingDate = eventBooking.BookingDate,
                EventId = eventBooking.EventId,
                UserId = eventBooking.UserId
            };

            return CreatedAtAction("GetEventBooking", new { id = eventBooking.Id }, resultDto);
        }

        // PUT: api/EventBookings/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutEventBooking(int id, UpdateEventBookingDto updateDto)
        {
            var eventBooking = await _context.EventBookings.FindAsync(id);
            if (eventBooking == null)
            {
                return NotFound();
            }

            // Validate event and user exist
            var eventItem = await _context.Events.FindAsync(updateDto.EventId);
            var user = await _context.Users.FindAsync(updateDto.UserId);

            if (eventItem == null || user == null)
            {
                return BadRequest("Invalid Event ID or User ID");
            }

            // Check for existing booking with different ID
            var existingBooking = await _context.EventBookings
                .FirstOrDefaultAsync(eb => eb.EventId == updateDto.EventId &&
                                         eb.UserId == updateDto.UserId &&
                                         eb.Id != id);

            if (existingBooking != null)
            {
                return BadRequest("User has already booked this event");
            }

            eventBooking.EventId = updateDto.EventId;
            eventBooking.UserId = updateDto.UserId;
            eventBooking.EventName = eventItem.Name;
            eventBooking.UserName = user.Username;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EventBookingExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: api/EventBookings/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEventBooking(int id)
        {
            var eventBooking = await _context.EventBookings.FindAsync(id);
            if (eventBooking == null)
            {
                return NotFound();
            }

            _context.EventBookings.Remove(eventBooking);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool EventBookingExists(int id)
        {
            return _context.EventBookings.Any(e => e.Id == id);
        }
    }
}