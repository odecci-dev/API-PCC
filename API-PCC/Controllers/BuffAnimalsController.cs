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
    public class BuffAnimalsController : ControllerBase
    {
        private readonly PCC_DEVContext _context;

        public BuffAnimalsController(PCC_DEVContext context)
        {
            _context = context;
        }

        // GET: BuffAnimals/list
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ABuffAnimal>>> list()
        {
          if (_context.ABuffAnimals == null)
          {
              return NotFound();
          }
            return await _context.ABuffAnimals.ToListAsync();
        }

        // GET: BuffAnimals/search/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ABuffAnimal>> search(int id)
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

        // PUT: BuffAnimals/update/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> update(int id, ABuffAnimal aBuffAnimal)
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

        // POST: BuffAnimals/save
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<ABuffAnimal>> save(ABuffAnimal aBuffAnimal)
        {
          if (_context.ABuffAnimals == null)
          {
              return Problem("Entity set 'PCC_DEVContext.ABuffAnimals'  is null.");
          }
            _context.ABuffAnimals.Add(aBuffAnimal);
            await _context.SaveChangesAsync();

            return CreatedAtAction("save", new { id = aBuffAnimal.Id }, aBuffAnimal);
        }

        // DELETE: BuffAnimals/delete/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> delete(int id)
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
