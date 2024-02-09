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
    public class ATypeOwnershipsController : ControllerBase
    {
        private readonly PCC_DEVContext _context;

        public ATypeOwnershipsController(PCC_DEVContext context)
        {
            _context = context;
        }

        // GET: api/ATypeOwnerships
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ATypeOwnership>>> GetATypeOwnerships()
        {
          if (_context.ATypeOwnerships == null)
          {
              return NotFound();
          }
            return await _context.ATypeOwnerships.ToListAsync();
        }

        // GET: api/ATypeOwnerships/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ATypeOwnership>> GetATypeOwnership(int id)
        {
          if (_context.ATypeOwnerships == null)
          {
              return NotFound();
          }
            var aTypeOwnership = await _context.ATypeOwnerships.FindAsync(id);

            if (aTypeOwnership == null)
            {
                return NotFound();
            }

            return aTypeOwnership;
        }

        // PUT: api/ATypeOwnerships/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutATypeOwnership(int id, ATypeOwnership aTypeOwnership)
        {
            if (id != aTypeOwnership.Id)
            {
                return BadRequest();
            }

            _context.Entry(aTypeOwnership).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ATypeOwnershipExists(id))
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

        // POST: api/ATypeOwnerships
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<ATypeOwnership>> PostATypeOwnership(ATypeOwnership aTypeOwnership)
        {
          if (_context.ATypeOwnerships == null)
          {
              return Problem("Entity set 'PCC_DEVContext.ATypeOwnerships'  is null.");
          }
            _context.ATypeOwnerships.Add(aTypeOwnership);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetATypeOwnership", new { id = aTypeOwnership.Id }, aTypeOwnership);
        }

        // DELETE: api/ATypeOwnerships/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteATypeOwnership(int id)
        {
            if (_context.ATypeOwnerships == null)
            {
                return NotFound();
            }
            var aTypeOwnership = await _context.ATypeOwnerships.FindAsync(id);
            if (aTypeOwnership == null)
            {
                return NotFound();
            }

            _context.ATypeOwnerships.Remove(aTypeOwnership);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ATypeOwnershipExists(int id)
        {
            return (_context.ATypeOwnerships?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
