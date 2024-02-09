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
    public class ABuffAnimalsController : ControllerBase
    {
        private readonly PCC_DEVContext _context;

        public ABuffAnimalsController(PCC_DEVContext context)
        {
            _context = context;
        }

        // GET: api/ABuffAnimals
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ABuffAnimal>>> GetABuffAnimals()
        {
          if (_context.ABuffAnimals == null)
          {
              return NotFound();
          }
            return await _context.ABuffAnimals.ToListAsync();
        }

        // GET: api/ABuffAnimals/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ABuffAnimal>> GetABuffAnimal(int id)
        {
          if (_context.ABuffAnimals == null)
          {
              return NotFound();
          }
            var aBuffAnimal = await _context.ABuffAnimals.FindAsync(id);

            if (aBuffAnimal == null)
            {
                return NotFound();
            }

            return aBuffAnimal;
        }

        // PUT: api/ABuffAnimals/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutABuffAnimal(int id, ABuffAnimal aBuffAnimal)
        {
            if (id != aBuffAnimal.Id)
            {
                return BadRequest();
            }

            _context.Entry(aBuffAnimal).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ABuffAnimalExists(id))
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

        // POST: api/ABuffAnimals
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<ABuffAnimal>> PostABuffAnimal(ABuffAnimal aBuffAnimal)
        {
          if (_context.ABuffAnimals == null)
          {
              return Problem("Entity set 'PCC_DEVContext.ABuffAnimals'  is null.");
          }
            _context.ABuffAnimals.Add(aBuffAnimal);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetABuffAnimal", new { id = aBuffAnimal.Id }, aBuffAnimal);
        }

        // DELETE: api/ABuffAnimals/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteABuffAnimal(int id)
        {
            if (_context.ABuffAnimals == null)
            {
                return NotFound();
            }
            var aBuffAnimal = await _context.ABuffAnimals.FindAsync(id);
            if (aBuffAnimal == null)
            {
                return NotFound();
            }

            _context.ABuffAnimals.Remove(aBuffAnimal);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ABuffAnimalExists(int id)
        {
            return (_context.ABuffAnimals?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
