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
    public class HHerdTypesController : ControllerBase
    {
        private readonly PCC_DEVContext _context;

        public HHerdTypesController(PCC_DEVContext context)
        {
            _context = context;
        }

        // GET: api/HHerdTypes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<HHerdType>>> GetHHerdTypes()
        {
          if (_context.HHerdTypes == null)
          {
              return NotFound();
          }
            return await _context.HHerdTypes.ToListAsync();
        }

        // GET: api/HHerdTypes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<HHerdType>> GetHHerdType(int id)
        {
          if (_context.HHerdTypes == null)
          {
              return NotFound();
          }
            var hHerdType = await _context.HHerdTypes.FindAsync(id);

            if (hHerdType == null)
            {
                return NotFound();
            }

            return hHerdType;
        }

        // PUT: api/HHerdTypes/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutHHerdType(int id, HHerdType hHerdType)
        {
            if (id != hHerdType.Id)
            {
                return BadRequest();
            }

            _context.Entry(hHerdType).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!HHerdTypeExists(id))
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

        // POST: api/HHerdTypes
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<HHerdType>> PostHHerdType(HHerdType hHerdType)
        {
          if (_context.HHerdTypes == null)
          {
              return Problem("Entity set 'PCC_DEVContext.HHerdTypes'  is null.");
          }
            _context.HHerdTypes.Add(hHerdType);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetHHerdType", new { id = hHerdType.Id }, hHerdType);
        }

        // DELETE: api/HHerdTypes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteHHerdType(int id)
        {
            if (_context.HHerdTypes == null)
            {
                return NotFound();
            }
            var hHerdType = await _context.HHerdTypes.FindAsync(id);
            if (hHerdType == null)
            {
                return NotFound();
            }

            _context.HHerdTypes.Remove(hHerdType);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool HHerdTypeExists(int id)
        {
            return (_context.HHerdTypes?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
