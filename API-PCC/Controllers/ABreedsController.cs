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
    public class ABreedsController : ControllerBase
    {
        private readonly PCC_DEVContext _context;

        public ABreedsController(PCC_DEVContext context)
        {
            _context = context;
        }

        // GET: api/ABreeds
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ABreed>>> GetABreeds()
        {
          if (_context.ABreeds == null)
          {
              return NotFound();
          }
            return await _context.ABreeds.ToListAsync();
        }

        // GET: api/ABreeds/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ABreed>> GetABreed(int id)
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

        // PUT: api/ABreeds/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutABreed(int id, ABreed aBreed)
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

        // POST: api/ABreeds
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<ABreed>> PostABreed(ABreed aBreed)
        {
          if (_context.ABreeds == null)
          {
              return Problem("Entity set 'PCC_DEVContext.ABreeds'  is null.");
          }
            _context.ABreeds.Add(aBreed);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetABreed", new { id = aBreed.Id }, aBreed);
        }

        // DELETE: api/ABreeds/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteABreed(int id)
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
