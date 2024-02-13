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
    public class HBuffHerdsController : ControllerBase
    {
        private readonly PCC_DEVContext _context;

        public HBuffHerdsController(PCC_DEVContext context)
        {
            _context = context;
        }

        // GET: BuffHerds/list
        [HttpGet]
        public async Task<ActionResult<IEnumerable<HBuffHerd>>> list()
        {
          if (_context.HBuffHerds == null)
          {
              return NotFound();
          }
            return await _context.HBuffHerds.ToListAsync();
        }

        // GET: BuffHerds/search/5
        [HttpGet("{id}")]
        public async Task<ActionResult<HBuffHerd>> search(int id)
        {
          if (_context.HBuffHerds == null)
          {
              return NotFound();
          }
            var hBuffHerd = await _context.HBuffHerds.FindAsync(id);

            if (hBuffHerd == null)
            {
                return NotFound();
            }

            return hBuffHerd;
        }

        // PUT: BuffHerds/update/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> update(int id, HBuffHerd hBuffHerd)
        {
            if (id != hBuffHerd.Id)
            {
                return BadRequest();
            }

            _context.Entry(hBuffHerd).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!HBuffHerdExists(id))
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

        // POST: BuffHerds/save
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<HBuffHerd>> save(HBuffHerd hBuffHerd)
        {
          if (_context.HBuffHerds == null)
          {
              return Problem("Entity set 'PCC_DEVContext.HBuffHerds'  is null.");
          }
            _context.HBuffHerds.Add(hBuffHerd);
            await _context.SaveChangesAsync();

            return CreatedAtAction("save", new { id = hBuffHerd.Id }, hBuffHerd);
        }

        // DELETE: BuffHerds/delete/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> delete(int id)
        {
            if (_context.HBuffHerds == null)
            {
                return NotFound();
            }
            var hBuffHerd = await _context.HBuffHerds.FindAsync(id);
            if (hBuffHerd == null)
            {
                return NotFound();
            }

            _context.HBuffHerds.Remove(hBuffHerd);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool HBuffHerdExists(int id)
        {
            return (_context.HBuffHerds?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
