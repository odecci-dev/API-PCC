using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using API_PCC.Data;
using API_PCC.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authorization;
using System.Data;
using System.Linq;

namespace API_PCC.Controllers
{
    //[Authorize("ApiKey")]
    [Route("[controller]/[action]")]
    [ApiController]
    public class FeedingSystemsController : ControllerBase
    {

        private readonly PCC_DEVContext _context;

        public class PaginationModel
        {
            public string? CurrentPage { get; set; }
            public string? NextPage { get; set; }
            public string? PrevPage { get; set; }
            public string? TotalPage { get; set; }
            public string? PageSize { get; set; }
            public string? TotalRecord { get; set; }
            public List<HFeedingSystem> items { get; set; }
        }
        public class FeedingSystemSearchFilter
        {
            public string? feedCode {  get; set; }
            public string? feedDesc { get; set; }
            public int page { get; set; }
            public int pageSize { get; set; }
        }

        public FeedingSystemsController(PCC_DEVContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> List(FeedingSystemSearchFilter searchFilter)
        {
            int pagesize = searchFilter.pageSize == 0 ? 10: searchFilter.pageSize;
            int page = searchFilter.page == 0? 1: searchFilter.page;
            var items = (dynamic)null;
            int totalItems = 0;
            int totalPages = 0;

            var feedingSystemList = _context.HFeedingSystems.AsNoTracking();
            try
            {
                if (searchFilter.feedCode != null && searchFilter.feedCode != "")
                {
                    feedingSystemList = feedingSystemList.Where(feedingSystem => feedingSystem.FeedCode.Contains(searchFilter.feedCode));
                }

                if (searchFilter.feedDesc != null && searchFilter.feedDesc != "")
                {
                    feedingSystemList = feedingSystemList.Where(feedingSystem => feedingSystem.FeedDesc.Contains(searchFilter.feedDesc));

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

        // GET: FeedingSystems/search/5
        [HttpGet("{id}")]
        public async Task<ActionResult<HFeedingSystem>> Search(int id)
        {
          if (_context.HFeedingSystems == null)
          {
              return NotFound();
          }
            var hFeedingSystem = await _context.HFeedingSystems.FindAsync(id);

            if (hFeedingSystem == null)
            {
                return NotFound();
            }

            return hFeedingSystem;
        }

        // PUT: FeedingSystems/update/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, HFeedingSystem hFeedingSystem)
        {
            if (id != hFeedingSystem.Id)
            {
                return BadRequest();
            }

            _context.Entry(hFeedingSystem).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!HFeedingSystemExists(id))
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

        // POST: FeedingSystems/save
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<HFeedingSystem>> save(HFeedingSystem hFeedingSystem)
        {
          if (_context.HFeedingSystems == null)
          {
              return Problem("Entity set 'PCC_DEVContext.HFeedingSystems'  is null.");
          }

          if(HFeedingSystemExists(hFeedingSystem.Id) || (HFeedingSystemFeedCodeExists(hFeedingSystem.FeedCode) && HFeedingSystemFeedDescExists(hFeedingSystem.FeedDesc)))
            {
                return Conflict("Entity already exists");
            }

            _context.HFeedingSystems.Add(hFeedingSystem);
            await _context.SaveChangesAsync();

            return CreatedAtAction("save", new { id = hFeedingSystem.Id }, hFeedingSystem);
        }

        // DELETE: FeedingSystems/delete/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> delete(int id)
        {
            if (_context.HFeedingSystems == null)
            {
                return NotFound();
            }
            var hFeedingSystem = await _context.HFeedingSystems.FindAsync(id);
            if (hFeedingSystem == null)
            {
                return NotFound();
            }

            _context.HFeedingSystems.Remove(hFeedingSystem);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool HFeedingSystemExists(int id)
        {
            return (_context.HFeedingSystems?.Any(e => e.Id == id)).GetValueOrDefault();
        }
        
        private bool HFeedingSystemFeedCodeExists(String feedCode)
        {
            return (_context.HFeedingSystems?.Any(e => e.FeedCode == feedCode)).GetValueOrDefault();
        }

        private bool HFeedingSystemFeedDescExists(String feedDesc)
        {
            return (_context.HFeedingSystems?.Any(e => e.FeedDesc == feedDesc)).GetValueOrDefault();
        }
    }
}
