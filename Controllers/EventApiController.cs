using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CbsStudents.Data;
using cbsStudents.Models.Entities;

namespace cbsStudents.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventApiController : ControllerBase
    {
        private readonly CbsStudentsContext _context;

        public EventApiController(CbsStudentsContext context)
        {
            _context = context;
        }

        // GET: api/EventApi
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Event>>> GetEvent()
        {
            return await _context.Event.Include(x => x.Comments).ThenInclude(x => x.User).Include(x => x.User).ToListAsync();
        }

        // GET: api/EventApi/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Event>> GetEvent(int id)
        {
            // var @event = await _context.Event.FindAsync(id);
            var @event1 = await _context.Event.Include(x => x.Comments).ThenInclude(x => x.User).Include(x => x.User).FirstOrDefaultAsync(x => x.EventId == id );

            if (@event1 == null)
            {
                return NotFound();
            }

            return @event1;
        }
        // GET: api/EventApi/Custom/5
        [HttpGet("Custom/{id}")]
        public async Task<ActionResult<Object>> GetEventCustom(int id)
        {
            // var @event = await _context.Event.FindAsync(id);
            var @event1 =from p in _context.Event select new {
                EventId = p.EventId,
                EventName = p.EventName,
                Date = p.Date
            };
            @event1 = @event1.Where(x => x.EventId == id);

            if (@event1 == null)
            {
                return NotFound();
            }

            return await @event1.ToListAsync();
        }
        // PUT: api/EventApi/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutEvent(int id, Event @event)
        {
            if (id != @event.EventId)
            {
                return BadRequest();
            }

            _context.Entry(@event).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EventExists(id))
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

        // POST: api/EventApi
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Event>> PostEvent(Event @event)
        {
            _context.Event.Add(@event);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetEvent", new { id = @event.EventId }, @event);
        }

        // DELETE: api/EventApi/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEvent(int id)
        {
            var @event = await _context.Event.FindAsync(id);
            if (@event == null)
            {
                return NotFound();
            }

            _context.Event.Remove(@event);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool EventExists(int id)
        {
            return _context.Event.Any(e => e.EventId == id);
        }
    }
}
