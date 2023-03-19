using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Speakers.API.Data;
using Speakers.API.Models;

namespace Speakers.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SpeakersController : ControllerBase
    {
        private readonly SpeakersContext _context;

        public SpeakersController(SpeakersContext context)
        {
            _context = context;
        }

        // GET: api/Speakers
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<Speaker>>> GetSpeaker()
        {
            return await _context.Speakers.ToListAsync();
        }

        // GET: api/Speakers/[GUID]
        [HttpGet("{id}")]
        public async Task<ActionResult<Speaker>> GetSpeaker(Guid id)
        {
            var speaker = await _context.Speakers.FindAsync(id);

            if (speaker == null)
            {
                return NotFound();
            }

            return speaker;
        }

        // GET: api/Speakers?search=john
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<Speaker>>> SearchSpeaker([FromQuery] string search)
        {
            var speakers = await _context.Speakers.Where(
                s => s.FirstName.Contains(search) || s.LastName.Contains(search)).ToListAsync();

            if (speakers == null)
            {
                return NotFound();
            }

            return speakers;
        }


        // PUT: api/Speakers/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSpeaker(Guid id, Speaker speaker)
        {
            if (id != speaker.SpeakerId)
            {
                return BadRequest();
            }

            _context.Entry(speaker).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SpeakerExists(id))
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

        // POST: api/Speakers
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Speaker>> PostSpeaker([FromBody] Speaker speaker)
        {
            _context.Speakers.Add(speaker);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetSpeaker", new { id = speaker.SpeakerId }, speaker);
        }

        // DELETE: api/Speakers/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSpeaker(Guid id)
        {
            var speaker = await _context.Speakers.FindAsync(id);
            if (speaker == null)
            {
                return NotFound();
            }

            _context.Speakers.Remove(speaker);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool SpeakerExists(Guid id)
        {
            return _context.Speakers.Any(e => e.SpeakerId == id);
        }
    }
}
