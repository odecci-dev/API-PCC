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
    public class BreedsController : ControllerBase
    {
        private readonly PCC_DEVContext _context;

        public BreedsController(PCC_DEVContext context)
        {
            _context = context;
        }

        // GET: Breeds/list
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ABreed>>> list()
        {
          if (_context.ABreeds == null)
          {
              return NotFound();
          }
            return await _context.ABreeds.ToListAsync();
        }

        // GET: Breeds/search/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ABreed>> search(int id)
        {
          if (_context.ABreeds == null)
          {
              return NotFound();
          }
            var aBreed = await _context.ABreeds.FindAsync(id);

            if (aBreed == null)
            {
                return NotFound();
            }

            return aBreed;
        }

        // PUT: Breeds/update/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> update(int id, ABreed aBreed)
        {
            if (id != aBreed.Id)
            {
                return BadRequest();
            }

            _context.Entry(aBreed).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ABreedExists(id))
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

        // POST: Breeds/save
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<ABreed>> save(ABreed aBreed)
        {
          if (_context.ABreeds == null)
          {
              return Problem("Entity set 'PCC_DEVContext.ABreeds'  is null.");
          }
            _context.ABreeds.Add(aBreed);
            await _context.SaveChangesAsync();

            return CreatedAtAction("save", new { id = aBreed.Id }, aBreed);
        }

        // DELETE: Breeds/delete/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> delete(int id)
        {
            if (_context.ABreeds == null)
            {
                return NotFound();
            }
            var aBreed = await _context.ABreeds.FindAsync(id);
            if (aBreed == null)
            {
                return NotFound();
            }

            _context.ABreeds.Remove(aBreed);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ABreedExists(int id)
        {
            return (_context.ABreeds?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
