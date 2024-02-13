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
    public class TypeOwnershipsController : ControllerBase
    {
        private readonly PCC_DEVContext _context;

        public TypeOwnershipsController(PCC_DEVContext context)
        {
            _context = context;
        }

        // GET: TypeOwnerships/list
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ATypeOwnership>>> list()
        {
          if (_context.ATypeOwnerships == null)
          {
              return NotFound();
          }
            return await _context.ATypeOwnerships.ToListAsync();
        }

        // GET: TypeOwnerships/search/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ATypeOwnership>> update(int id)
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

        // PUT: TypeOwnerships/update/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> update(int id, ATypeOwnership aTypeOwnership)
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

        // POST: TypeOwnerships/save
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<ATypeOwnership>> save(ATypeOwnership aTypeOwnership)
        {
          if (_context.ATypeOwnerships == null)
          {
              return Problem("Entity set 'PCC_DEVContext.ATypeOwnerships'  is null.");
          }
            _context.ATypeOwnerships.Add(aTypeOwnership);
            await _context.SaveChangesAsync();

            return CreatedAtAction("save", new { id = aTypeOwnership.Id }, aTypeOwnership);
        }

        // DELETE: TypeOwnerships/delete/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> delete(int id)
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
