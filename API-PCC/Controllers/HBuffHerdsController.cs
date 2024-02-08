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
    public class HBuffHerdsController : ControllerBase
    {
        private readonly PCC_DEVContext _context;

        public HBuffHerdsController(PCC_DEVContext context)
        {
            _context = context;
        }

        // GET: api/HBuffHerds
        [HttpGet]
        public async Task<ActionResult<IEnumerable<HBuffHerd>>> GetHBuffHerds()
        {
          if (_context.HBuffHerds == null)
          {
              return NotFound();
          }
            return await _context.HBuffHerds.ToListAsync();
        }

        // GET: api/HBuffHerds/5
        [HttpGet("{id}")]
        public async Task<ActionResult<HBuffHerd>> GetHBuffHerd(int id)
        {
          if (_context.HBuffHerds == null)
          {
              return NotFound();
          }
            var hBuffHerd = await _context.HBuffHerds.FindAsync(id);

            if (hBuffHerd == null)
            {
                return NotFound();
            }

            return hBuffHerd;
        }

        // PUT: api/HBuffHerds/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutHBuffHerd(int id, HBuffHerd hBuffHerd)
        {
            if (id != hBuffHerd.Id)
            {
                return BadRequest();
            }

            _context.Entry(hBuffHerd).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!HBuffHerdExists(id))
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

        // POST: api/HBuffHerds
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<HBuffHerd>> PostHBuffHerd(HBuffHerd hBuffHerd)
        {
          if (_context.HBuffHerds == null)
          {
              return Problem("Entity set 'PCC_DEVContext.HBuffHerds'  is null.");
          }
            _context.HBuffHerds.Add(hBuffHerd);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetHBuffHerd", new { id = hBuffHerd.Id }, hBuffHerd);
        }

        // DELETE: api/HBuffHerds/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteHBuffHerd(int id)
        {
            if (_context.HBuffHerds == null)
            {
                return NotFound();
            }
            var hBuffHerd = await _context.HBuffHerds.FindAsync(id);
            if (hBuffHerd == null)
            {
                return NotFound();
            }

            _context.HBuffHerds.Remove(hBuffHerd);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool HBuffHerdExists(int id)
        {
            return (_context.HBuffHerds?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
