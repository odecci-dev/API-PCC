using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using API_PCC.Data;
using API_PCC.Models;

namespace API_PCC.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HFeedingSystemsController : ControllerBase
    {
        private readonly PCC_DEVContext _context;

        public HFeedingSystemsController(PCC_DEVContext context)
        {
            _context = context;
        }

        // GET: api/HFeedingSystems
        [HttpGet]
        public async Task<ActionResult<IEnumerable<HFeedingSystem>>> GetHFeedingSystems()
        {
          if (_context.HFeedingSystems == null)
          {
              return NotFound();
          }
            return await _context.HFeedingSystems.ToListAsync();
        }

        // GET: api/HFeedingSystems/5
        [HttpGet("{id}")]
        public async Task<ActionResult<HFeedingSystem>> GetHFeedingSystem(int id)
        {
          if (_context.HFeedingSystems == null)
          {
              return NotFound();
          }
            var hFeedingSystem = await _context.HFeedingSystems.FindAsync(id);

            if (hFeedingSystem == null)
            {
                return NotFound();
            }

            return hFeedingSystem;
        }

        // PUT: api/HFeedingSystems/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutHFeedingSystem(int id, HFeedingSystem hFeedingSystem)
        {
            if (id != hFeedingSystem.Id)
            {
                return BadRequest();
            }

            _context.Entry(hFeedingSystem).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!HFeedingSystemExists(id))
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

        // POST: api/HFeedingSystems
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<HFeedingSystem>> PostHFeedingSystem(HFeedingSystem hFeedingSystem)
        {
          if (_context.HFeedingSystems == null)
          {
              return Problem("Entity set 'PCC_DEVContext.HFeedingSystems'  is null.");
          }
            _context.HFeedingSystems.Add(hFeedingSystem);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetHFeedingSystem", new { id = hFeedingSystem.Id }, hFeedingSystem);
        }

        // DELETE: api/HFeedingSystems/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteHFeedingSystem(int id)
        {
            if (_context.HFeedingSystems == null)
            {
                return NotFound();
            }
            var hFeedingSystem = await _context.HFeedingSystems.FindAsync(id);
            if (hFeedingSystem == null)
            {
                return NotFound();
            }

            _context.HFeedingSystems.Remove(hFeedingSystem);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool HFeedingSystemExists(int id)
        {
            return (_context.HFeedingSystems?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
