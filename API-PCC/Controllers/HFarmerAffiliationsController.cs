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
    public class HFarmerAffiliationsController : ControllerBase
    {
        private readonly PCC_DEVContext _context;

        public HFarmerAffiliationsController(PCC_DEVContext context)
        {
            _context = context;
        }

        // GET: api/HFarmerAffiliations
        [HttpGet]
        public async Task<ActionResult<IEnumerable<HFarmerAffiliation>>> GetHFarmerAffiliations()
        {
          if (_context.HFarmerAffiliations == null)
          {
              return NotFound();
          }
            return await _context.HFarmerAffiliations.ToListAsync();
        }

        // GET: api/HFarmerAffiliations/5
        [HttpGet("{id}")]
        public async Task<ActionResult<HFarmerAffiliation>> GetHFarmerAffiliation(int id)
        {
          if (_context.HFarmerAffiliations == null)
          {
              return NotFound();
          }
            var hFarmerAffiliation = await _context.HFarmerAffiliations.FindAsync(id);

            if (hFarmerAffiliation == null)
            {
                return NotFound();
            }

            return hFarmerAffiliation;
        }

        // PUT: api/HFarmerAffiliations/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutHFarmerAffiliation(int id, HFarmerAffiliation hFarmerAffiliation)
        {
            if (id != hFarmerAffiliation.Id)
            {
                return BadRequest();
            }

            _context.Entry(hFarmerAffiliation).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!HFarmerAffiliationExists(id))
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

        // POST: api/HFarmerAffiliations
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<HFarmerAffiliation>> PostHFarmerAffiliation(HFarmerAffiliation hFarmerAffiliation)
        {
          if (_context.HFarmerAffiliations == null)
          {
              return Problem("Entity set 'PCC_DEVContext.HFarmerAffiliations'  is null.");
          }
            _context.HFarmerAffiliations.Add(hFarmerAffiliation);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetHFarmerAffiliation", new { id = hFarmerAffiliation.Id }, hFarmerAffiliation);
        }

        // DELETE: api/HFarmerAffiliations/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteHFarmerAffiliation(int id)
        {
            if (_context.HFarmerAffiliations == null)
            {
                return NotFound();
            }
            var hFarmerAffiliation = await _context.HFarmerAffiliations.FindAsync(id);
            if (hFarmerAffiliation == null)
            {
                return NotFound();
            }

            _context.HFarmerAffiliations.Remove(hFarmerAffiliation);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool HFarmerAffiliationExists(int id)
        {
            return (_context.HFarmerAffiliations?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
