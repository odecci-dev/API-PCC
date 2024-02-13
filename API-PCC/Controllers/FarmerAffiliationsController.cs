using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using API_PCC.Data;
using API_PCC.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authorization;

namespace API_PCC.Controllers
{
    [Authorize("ApiKey")]
    [Route("[controller]/[action]")]
    [ApiController]
    public class FarmerAffiliationsController : ControllerBase
    {
        private readonly PCC_DEVContext _context;

        public FarmerAffiliationsController(PCC_DEVContext context)
        {
            _context = context;
        }

        // GET: FarmerAffiliations/list
        [HttpGet]
        public async Task<ActionResult<IEnumerable<HFarmerAffiliation>>> list()
        {
          if (_context.HFarmerAffiliations == null)
          {
              return NotFound();
          }
            return await _context.HFarmerAffiliations.ToListAsync();
        }

        // GET: FarmerAffiliations/search/5
        [HttpGet("{id}")]
        public async Task<ActionResult<HFarmerAffiliation>> search(int id)
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

        // PUT: FarmerAffiliations/update/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> update(int id, HFarmerAffiliation hFarmerAffiliation)
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

        // POST: FarmerAffiliations/save
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<HFarmerAffiliation>> save(HFarmerAffiliation hFarmerAffiliation)
        {
          if (_context.HFarmerAffiliations == null)
          {
              return Problem("Entity set 'PCC_DEVContext.HFarmerAffiliations'  is null.");
          }
            _context.HFarmerAffiliations.Add(hFarmerAffiliation);
            await _context.SaveChangesAsync();

            return CreatedAtAction("save", new { id = hFarmerAffiliation.Id }, hFarmerAffiliation);
        }

        // DELETE: FarmerAffiliations/delete/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> delete(int id)
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
