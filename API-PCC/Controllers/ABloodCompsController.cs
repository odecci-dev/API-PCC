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
    [Route("api/[controller]")]
    [ApiController]
    public class ABloodCompsController : ControllerBase
    {
        private readonly PCC_DEVContext _context;

        public ABloodCompsController(PCC_DEVContext context)
        {
            _context = context;
        }

        // GET: api/ABloodComps
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ABloodComp>>> GetABloodComps()
        {
          if (_context.ABloodComps == null)
          {
              return NotFound();
          }
            return await _context.ABloodComps.ToListAsync();
        }

        // GET: api/ABloodComps/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ABloodComp>> GetABloodComp(int id)
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

        // PUT: api/ABloodComps/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutABloodComp(int id, ABloodComp aBloodComp)
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

        // POST: api/ABloodComps
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<ABloodComp>> PostABloodComp(ABloodComp aBloodComp)
        {
          if (_context.ABloodComps == null)
          {
              return Problem("Entity set 'PCC_DEVContext.ABloodComps'  is null.");
          }
            _context.ABloodComps.Add(aBloodComp);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetABloodComp", new { id = aBloodComp.Id }, aBloodComp);
        }

        // DELETE: api/ABloodComps/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteABloodComp(int id)
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
