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
    public class BuffaloTypesController : ControllerBase
    {
        private readonly PCC_DEVContext _context;

        public BuffaloTypesController(PCC_DEVContext context)
        {
            _context = context;
        }

        // GET: BuffaloTypes/list
        [HttpGet]
        public async Task<ActionResult<IEnumerable<HBuffaloType>>> list()
        {
          if (_context.HBuffaloTypes == null)
          {
              return NotFound();
          }
            return await _context.HBuffaloTypes.ToListAsync();
        }

        // GET: BuffaloTypes/search/5
        [HttpGet("{id}")]
        public async Task<ActionResult<HBuffaloType>> search(int id)
        {
          if (_context.HBuffaloTypes == null)
          {
              return NotFound();
          }
            var hBuffaloType = await _context.HBuffaloTypes.FindAsync(id);

            if (hBuffaloType == null)
            {
                return NotFound();
            }

            return hBuffaloType;
        }

        // PUT: BuffaloTypes/update/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> update(int id, HBuffaloType hBuffaloType)
        {
            if (id != hBuffaloType.Id)
            {
                return BadRequest();
            }

            _context.Entry(hBuffaloType).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!HBuffaloTypeExists(id))
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

        // POST: BuffaloTypes/save
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<HBuffaloType>> save(HBuffaloType hBuffaloType)
        {
          if (_context.HBuffaloTypes == null)
          {
              return Problem("Entity set 'PCC_DEVContext.HBuffaloTypes'  is null.");
          }
            _context.HBuffaloTypes.Add(hBuffaloType);
            await _context.SaveChangesAsync();

            return CreatedAtAction("save", new { id = hBuffaloType.Id }, hBuffaloType);
        }

        // DELETE: BuffaloTypes/delete/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> delete(int id)
        {
            if (_context.HBuffaloTypes == null)
            {
                return NotFound();
            }
            var hBuffaloType = await _context.HBuffaloTypes.FindAsync(id);
            if (hBuffaloType == null)
            {
                return NotFound();
            }

            _context.HBuffaloTypes.Remove(hBuffaloType);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool HBuffaloTypeExists(int id)
        {
            return (_context.HBuffaloTypes?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
