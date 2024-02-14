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
    public class BloodCompsController : ControllerBase
    {
        private readonly PCC_DEVContext _context;

        public BloodCompsController(PCC_DEVContext context)
        {
            _context = context;
        }

        // GET: BloodComps/list
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ABloodComp>>> list()
        {
          if (_context.ABloodComps == null)
          {
              return NotFound();
          }
            return await _context.ABloodComps.ToListAsync();
        }

        // GET: BloodComps/search/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ABloodComp>> search(int id)
        {
          if (_context.ABloodComps == null)
          {
              return NotFound();
          }
            var aBloodComp = await _context.ABloodComps.FindAsync(id);

            if (aBloodComp == null)
            {
                return NotFound();
            }

            return aBloodComp;
        }

        // PUT: BloodComps/update/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> update(int id, ABloodComp aBloodComp)
        {
            if (id != aBloodComp.Id)
            {
                return BadRequest();
            }

            _context.Entry(aBloodComp).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ABloodCompExists(id))
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

        // POST: BloodComps/save
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<ABloodComp>> save(ABloodComp aBloodComp)
        {
          if (_context.ABloodComps == null)
          {
              return Problem("Entity set 'PCC_DEVContext.ABloodComps'  is null.");
          }
            _context.ABloodComps.Add(aBloodComp);
            await _context.SaveChangesAsync();

            return CreatedAtAction("save", new { id = aBloodComp.Id }, aBloodComp);
        }

        // DELETE: BloodComps/delete/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> delete(int id)
        {
            if (_context.ABloodComps == null)
            {
                return NotFound();
            }
            var aBloodComp = await _context.ABloodComps.FindAsync(id);
            if (aBloodComp == null)
            {
                return NotFound();
            }

            _context.ABloodComps.Remove(aBloodComp);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ABloodCompExists(int id)
        {
            return (_context.ABloodComps?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
