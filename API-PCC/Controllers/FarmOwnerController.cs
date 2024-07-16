using API_PCC.ApplicationModels;
using API_PCC.Data;
using API_PCC.EntityModels;
using API_PCC.Manager;
using API_PCC.Models;
using API_PCC.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Utilities;
using System.Data;
using System.Drawing.Printing;
using System.Data;
using System.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;

namespace API_PCC.Controllers
{
    [Authorize("ApiKey")]
    [Route("[controller]/[action]")]
    [ApiController]
    public class FarmOwnerController : ControllerBase
    {
        private readonly PCC_DEVContext _context;

        public FarmOwnerController(PCC_DEVContext context)
        {
            _context = context;
        }

        // POST: farmOwners/list
        [HttpPost]
        public async Task<ActionResult<IEnumerable<TblFarmOwner>>> list(FarmOwnerSearchFilterModel searchFilter)
        {
            searchFilter.searchValue = StringSanitizer.sanitizeString(searchFilter.searchValue);
            try
            {
                List<TblFarmOwner> farmOwnerList = await buildFarmOwnerSearchQuery(searchFilter).ToListAsync();

                var result = buildFarmOwnerPagedModel(searchFilter, farmOwnerList);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Problem(ex.GetBaseException().ToString());
            }
        }

        private IQueryable<TblFarmOwner> buildFarmOwnerSearchQuery(FarmOwnerSearchFilterModel searchFilter)
        {
            IQueryable<TblFarmOwner> query = _context.TblFarmOwners;

            // assuming that you return all records when nothing is specified in the filter

            if (!searchFilter.searchValue.IsNullOrEmpty())
                query = query.Where(farmOwner => farmOwner.FirstName.Equals(searchFilter.searchValue) &&
                                                 farmOwner.LastName.Equals(searchFilter.searchValue));

            return query;
        }

        // GET: farmOwners/search/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TblFarmOwner>> search(int id)
        {
            if (_context.TblFarmOwners == null)
            {
                return Problem("Entity set 'PCC_DEVContext.TblFarmOwners' is null!");
            }
            var farmOwner = await _context.TblFarmOwners.FindAsync(id);

            if (farmOwner == null)
            {
                return Conflict("No records found!");
            }

            return farmOwner;
        }

        // PUT: farmOwners/update/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> update(int id, TblFarmOwner TblFarmOwner)
        {
            if (_context.TblFarmOwners == null)
            {
                return Problem("Entity set 'PCC_DEVContext.TblFarmOwners' is null!");
            }

            var farmOwner = _context.TblFarmOwners.AsNoTracking().Where(farmOwner => farmOwner.Id == id).FirstOrDefault();

            if (farmOwner == null)
            {
                return Conflict("No records matched!");
            }

            if (id != TblFarmOwner.Id)
            {
                return Conflict("Ids mismatched!");
            }

            bool hasDuplicateOnUpdate = (_context.TblFarmOwners?.Any(farmOwner => farmOwner.FirstName == TblFarmOwner.FirstName && farmOwner.LastName == TblFarmOwner.LastName && farmOwner.Id != id)).GetValueOrDefault();

            // check for duplication
            if (hasDuplicateOnUpdate)
            {
                return Conflict("Entity already exists");
            }

            try
            {
                _context.Entry(TblFarmOwner).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                return Ok("Update Successful!");
            }
            catch (Exception ex)
            {

                return Problem(ex.GetBaseException().ToString());
            }
        }

        // POST: farmOwners/save
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<TblFarmOwner>> save(TblFarmOwner TblFarmOwner)
        {
            if (_context.TblFarmOwners == null)
            {
                return Problem("Entity set 'PCC_DEVContext.TblFarmOwners' is null!");
            }

            bool hasDuplicateOnSave = (_context.TblFarmOwners?.Any(farmOwner => farmOwner.FirstName == TblFarmOwner.FirstName && farmOwner.LastName == TblFarmOwner.LastName)).GetValueOrDefault();

            if (hasDuplicateOnSave)
            {
                return Conflict("Entity already exists");
            }

            try
            {
                _context.TblFarmOwners.Add(TblFarmOwner);
                await _context.SaveChangesAsync();

                return CreatedAtAction("save", new { id = TblFarmOwner.Id }, TblFarmOwner);
            }
            catch (Exception ex)
            {

                return Problem(ex.GetBaseException().ToString());
            }
        }
        private List<FarmOwnerPagedModel> buildFarmOwnerPagedModel(FarmOwnerSearchFilterModel searchFilter, List<TblFarmOwner> farmOwners)
        {
            int pagesize = searchFilter.pageSize == 0 ? 10 : searchFilter.pageSize;
            int page = searchFilter.page == 0 ? 1 : searchFilter.page;
            var items = (dynamic)null;
            int totalItems = 0;
            int totalPages = 0;

            totalItems = farmOwners.Count;
            totalPages = (int)Math.Ceiling((double)totalItems / pagesize);
            items = farmOwners.Skip((page - 1) * pagesize).Take(pagesize).ToList();

            var result = new List<FarmOwnerPagedModel>();
            var item = new FarmOwnerPagedModel();

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
            item.items = farmOwners;
            result.Add(item);

            return result;
        }

    }
}
