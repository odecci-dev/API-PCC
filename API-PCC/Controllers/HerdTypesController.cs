using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using API_PCC.Data;
using API_PCC.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authorization;
using static API_PCC.Controllers.FeedingSystemsController;

namespace API_PCC.Controllers
{
    [Authorize("ApiKey")]
    [Route("[controller]/[action]")]
    [ApiController]
    public class HerdTypesController : ControllerBase
    {
        public class HerdTypesSearchFilter
        {
            public string? typeCode { get; set; }
            public string? typeDesc { get; set; }
            public int page { get; set; }
            public int pageSize { get; set; }
        }

        private readonly PCC_DEVContext _context;

        public HerdTypesController(PCC_DEVContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> List(HerdTypesSearchFilter searchFilter)
        {
            int pagesize = searchFilter.pageSize == 0 ? 10 : searchFilter.pageSize;
            int page = searchFilter.page == 0 ? 1 : searchFilter.page;
            var items = (dynamic)null;
            int totalItems = 0;
            int totalPages = 0;

            var feedingSystemList = _context.HHerdTypes.AsNoTracking();
            try
            {
                if (searchFilter.typeCode != null && searchFilter.typeCode != "")
                {
                    feedingSystemList = feedingSystemList.Where(feedingSystem => feedingSystem.HTypeCode.Contains(searchFilter.typeCode));
                }

                if (searchFilter.typeDesc != null && searchFilter.typeDesc != "")
                {
                    feedingSystemList = feedingSystemList.Where(feedingSystem => feedingSystem.HTypeDesc.Contains(searchFilter.typeDesc));

                }

                totalItems = feedingSystemList.ToList().Count();
                totalPages = (int)Math.Ceiling((double)totalItems / pagesize);
                items = feedingSystemList.Skip((page - 1) * pagesize).Take(pagesize).ToList();

                var result = new List<PaginationModel>();
                var item = new PaginationModel();

                int pages = searchFilter.page == 0 ? 1 : searchFilter.page;
                item.CurrentPage = searchFilter.page == 0 ? "1" : searchFilter.page.ToString();

                int page_prev = pages - 1;

                double t_records = Math.Ceiling(Convert.ToDouble(totalItems) / Convert.ToDouble(pagesize));
                int page_next = searchFilter.page >= t_records ? 0 : pages + 1;
                item.NextPage = items.Count % pagesize >= 0 ? page_next.ToString() : "0";
                item.PrevPage = pages == 1 ? "0" : page_prev.ToString();
                item.TotalPage = t_records.ToString();
                item.PageSize = pagesize.ToString();
                item.TotalRecord = totalItems.ToString();
                item.items = items;
                result.Add(item);
                return Ok(items);
            }

            catch (Exception)
            {
                return BadRequest("ERROR");
            }
        }

        // GET: HerdTypes/search/5
        [HttpGet("{id}")]
        public async Task<ActionResult<HHerdType>> search(int id)
        {
          if (_context.HHerdTypes == null)
          {
              return NotFound();
          }
            var hHerdType = await _context.HHerdTypes.FindAsync(id);

            if (hHerdType == null)
            {
                return NotFound();
            }

            return hHerdType;
        }

        // PUT: HerdTypes/update/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> update(int id, HHerdType hHerdType)
        {
            if (id != hHerdType.Id)
            {
                return BadRequest();
            }

            _context.Entry(hHerdType).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!HHerdTypeExists(id))
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

        // POST: HerdTypes/save
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public  ActionResult<HHerdType> save(HHerdType hHerdType)
        {
          if (_context.HHerdTypes == null)
          {
              return Problem("Entity set 'PCC_DEVContext.HHerdTypes'  is null.");
          }

          if (HHerdTypeExists(hHerdType.Id) || (HHerdTypeCodeExists(hHerdType.HTypeDesc) && HHerdTypeDescExists(hHerdType.HTypeDesc)))
          {
              return Conflict("Entity already exists");
          }
            _context.HHerdTypes.Add(hHerdType);
             _context.SaveChangesAsync();

            return CreatedAtAction("save", new { id = hHerdType.Id }, hHerdType);
        }

        // DELETE: HerdTypes/delete/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> delete(int id)
        {
            if (_context.HHerdTypes == null)
            {
                return NotFound();
            }
            var hHerdType = await _context.HHerdTypes.FindAsync(id);
            if (hHerdType == null)
            {
                return NotFound();
            }

            _context.HHerdTypes.Remove(hHerdType);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool HHerdTypeExists(int id)
        {
            return (_context.HHerdTypes?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        private bool HHerdTypeCodeExists(String typeCode)
        {
            return (_context.HHerdTypes?.Any(e => e.HTypeCode == typeCode)).GetValueOrDefault();
        }

        private bool HHerdTypeDescExists(String typeDesc)
        {
            return (_context.HHerdTypes?.Any(e => e.HTypeDesc == typeDesc)).GetValueOrDefault();
        }
    }
}
