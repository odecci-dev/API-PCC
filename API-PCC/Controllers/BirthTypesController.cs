
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
    public class BirthTypesController : ControllerBase
    {
        private readonly PCC_DEVContext _context;

        public BirthTypesController(PCC_DEVContext context)
        {
            _context = context;
        }

        // GET: BirthTypes/list
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ABirthType>>> list()
        {
          if (_context.ABirthTypes == null)
          {
              return NotFound();
          }
            return await _context.ABirthTypes.ToListAsync();
        }

        // GET: BirthTypes/search/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ABirthType>> search(int id)
        {
          if (_context.ABirthTypes == null)
          {
              return NotFound();
          }
            var aBirthType = await _context.ABirthTypes.FindAsync(id);

            if (aBirthType == null)
            {
                return NotFound();
            }

            return aBirthType;
        }

        // PUT: BirthTypes/update/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> update(int id, ABirthType aBirthType)
        {
            if (id != aBirthType.Id)
            {
                return BadRequest();
            }

            _context.Entry(aBirthType).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ABirthTypeExists(id))
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

        // POST: BirthTypes/save
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<ABirthType>> save(ABirthType aBirthType)
        {
          if (_context.ABirthTypes == null)
          {
              return Problem("Entity set 'PCC_DEVContext.ABirthTypes'  is null.");
          }
            _context.ABirthTypes.Add(aBirthType);
            await _context.SaveChangesAsync();

            return CreatedAtAction("save", new { id = aBirthType.Id }, aBirthType);
        }

        // DELETE: BirthTypes/delete/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> delete(int id)
        {
            if (_context.ABirthTypes == null)
            {
                return NotFound();
            }
            var aBirthType = await _context.ABirthTypes.FindAsync(id);
            if (aBirthType == null)
            {
                return NotFound();
            }

            _context.ABirthTypes.Remove(aBirthType);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ABirthTypeExists(int id)
        {
            return (_context.ABirthTypes?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
