
using API_PCC.ApplicationModels;
using API_PCC.ApplicationModels.Common;
using API_PCC.Data;
using API_PCC.EntityModels;
using API_PCC.Manager;
using API_PCC.Models;
using API_PCC.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.Data.SqlClient;
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

        // POST: BirthTypes/list
        [HttpPost]
        public async Task<ActionResult<IEnumerable<ABirthType>>> list(BirthTypesSearchFilterModel searchFilter)
        {
            try
            {
                List<ABirthType> birthTypeList = await buildBirthTypeSearchQuery(searchFilter).ToListAsync();

                var result = buildBirthTypesPagedModel(searchFilter, birthTypeList);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Problem(ex.GetBaseException().ToString());
            }
        }

        private IQueryable<ABirthType> buildBirthTypeSearchQuery(BirthTypesSearchFilterModel searchFilter)
        {
            IQueryable<ABirthType> query = _context.ABirthTypes.Where(birthType => !birthType.DeleteFlag);

            // assuming that you return all records when nothing is specified in the filter

            if (!searchFilter.searchParam.IsNullOrEmpty())
                query = query.Where(birthType =>
                               birthType.BirthTypeCode.Equals(searchFilter.searchParam) ||
                               birthType.BirthTypeDesc.Equals(searchFilter.searchParam));

            return query;
        }

        private List<BirthTypesPagedModel> buildBirthTypesPagedModel(BirthTypesSearchFilterModel searchFilter, List<ABirthType> birthTypes)
        {
            int pagesize = searchFilter.pageSize == 0 ? 10 : searchFilter.pageSize;
            int page = searchFilter.page == 0 ? 1 : searchFilter.page;
            var items = (dynamic)null;
            int totalItems = 0;
            int totalPages = 0;

            totalItems = birthTypes.Count;
            totalPages = (int)Math.Ceiling((double)totalItems / pagesize);
            items = birthTypes.Skip((page - 1) * pagesize).Take(pagesize).ToList();

            var result = new List<BirthTypesPagedModel>();
            var item = new BirthTypesPagedModel();

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
            item.items = birthTypes;
            result.Add(item);

            return result;
        }

        private List<ABirthType> convertDataRowListToBirthTypeList(List<DataRow> dataRowList)
        {
            var birthTypeList = new List<ABirthType>();

            foreach (DataRow dataRow in dataRowList)
            {
                var birthType = DataRowToObject.ToObject<ABirthType>(dataRow);
                birthTypeList.Add(birthType);
            }

            return birthTypeList;
        }

        // GET: BirthTypes/search/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ABirthType>> search(int id)
        {
            if (_context.ABirthTypes == null)
            {
                return Problem("Entity set 'PCC_DEVContext.BirthTypes' is null!");
            }
            var birthType = await _context.ABirthTypes.FindAsync(id);

            if (birthType == null || birthType.DeleteFlag)
            {
                return Conflict("No records found!");
            }
            return Ok(birthType);
        }

        // PUT: BirthTypes/update/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> update(int id, ABirthType aBirthType)
        {
            if (_context.ABirthTypes == null)
            {
                return Problem("Entity set 'PCC_DEVContext.BirthTypes' is null!");
            }

            var birthType = _context.ABirthTypes.AsNoTracking().Where(birthType => !birthType.DeleteFlag && birthType.Id == id).FirstOrDefault();

            if (birthType == null)
            {
                return Conflict("No records matched!");
            }

            if (id != birthType.Id)
            {
                return Conflict("Ids mismatched!");
            }

            bool hasDuplicateOnUpdate = (_context.ABirthTypes?.Any(birthType => !birthType.DeleteFlag && birthType.BirthTypeCode == aBirthType.BirthTypeCode && birthType.BirthTypeDesc == aBirthType.BirthTypeDesc && birthType.Id != id)).GetValueOrDefault();

            // check for duplication
            if (hasDuplicateOnUpdate)
            {
                return Conflict("Entity already exists");
            }

            try
            {
                _context.Entry(aBirthType).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                return Ok("Update Successful!");
            }
            catch (Exception ex)
            {
                
                return Problem(ex.GetBaseException().ToString());
            }
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
            bool hasDuplicateOnSave = (_context.ABirthTypes?.Any(birthType => !birthType.DeleteFlag && birthType.BirthTypeCode == aBirthType.BirthTypeCode && birthType.BirthTypeDesc == aBirthType.BirthTypeDesc)).GetValueOrDefault();


            if (hasDuplicateOnSave)
            {
                return Conflict("Entity already exists");
            }

            try
            {
                _context.ABirthTypes.Add(aBirthType);
                await _context.SaveChangesAsync();

                return CreatedAtAction("save", new { id = aBirthType.Id }, aBirthType);
            }
            catch (Exception ex)
            {
                
                return Problem(ex.GetBaseException().ToString());
            }
        }

        // POST: BirthTypes/delete/
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<IActionResult> delete(DeletionModel deletionModel)
        {

            if (_context.ABirthTypes == null)
            {
                return Problem("Entity set 'PCC_DEVContext.BirthTypes' is null!");
            }

            var birthType = await _context.ABirthTypes.FindAsync(deletionModel.id);
            if (birthType == null || birthType.DeleteFlag)
            {
                return Conflict("No records matched!");
            }

            try
            {
                birthType.DeleteFlag = true;
                birthType.DateDeleted = DateTime.Now;
                birthType.DeletedBy = deletionModel.deletedBy;
                birthType.DateRestored = null;
                birthType.RestoredBy = "";
                _context.Entry(birthType).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return Ok("Deletion Successful!");
            }
            catch (Exception ex)
            {
                
                return Problem(ex.GetBaseException().ToString());
            }
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ABirthType>>> view()
        {
            if (_context.ABirthTypes == null)
            {
                return Problem("Entity set 'PCC_DEVContext.ABirthTypes' is null.");
            }
            return await _context.ABirthTypes.Where(birthType => !birthType.DeleteFlag).ToListAsync();
        }

        // POST: BirthTypes/restore/
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<IActionResult> restore(RestorationModel restorationModel)
        {

            if (_context.ABirthTypes == null)
            {
                return Problem("Entity set 'PCC_DEVContext.BirthTypes' is null!");
            }

            var birthType = await _context.ABirthTypes.FindAsync(restorationModel.id);
            if (birthType == null || !birthType.DeleteFlag)
            {
                return Conflict("No deleted records matched!");
            }

            try
            {
                birthType.DeleteFlag = !birthType.DeleteFlag;
                birthType.DateDeleted = null;
                birthType.DeletedBy = "";
                birthType.DateRestored = DateTime.Now;
                birthType.RestoredBy = restorationModel.restoredBy;

                _context.Entry(birthType).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return Ok("Restoration Successful!");
            }
            catch (Exception ex)
            {
                
                return Problem(ex.GetBaseException().ToString());
            }
        }

    }
}
