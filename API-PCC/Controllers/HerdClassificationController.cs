﻿using API_PCC.ApplicationModels;
using API_PCC.ApplicationModels.Common;
using API_PCC.Data;
using API_PCC.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API_PCC.Controllers
{
    [Authorize("ApiKey")]
    [Route("[controller]/[action]")]
    [ApiController]
    public class HerdClassificationController : ControllerBase
    {

        private readonly PCC_DEVContext _context;

        public HerdClassificationController(PCC_DEVContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> List(HerdClassificationSearchFilterModel searchFilter)
        {
            if (_context.HHerdClassifications == null)
            {
                return Problem("Entity set 'PCC_DEVContext.HerdClassification' is null!");
            }

            int pagesize = searchFilter.pageSize == 0 ? 10 : searchFilter.pageSize;
            int page = searchFilter.page == 0 ? 1 : searchFilter.page;
            var items = (dynamic)null;
            int totalItems = 0;
            int totalPages = 0;

            var herdClassificationList = _context.HHerdClassifications.AsNoTracking();
            herdClassificationList = herdClassificationList.Where(herdClassification => !herdClassification.DeleteFlag);
            try
            {
                if (searchFilter.HerdClassCode != null && searchFilter.HerdClassCode != "")
                {
                    herdClassificationList = herdClassificationList.Where(herdClassification => herdClassification.HerdClassCode.Contains(searchFilter.HerdClassCode));
                }

                if (searchFilter.HerdClassDesc != null && searchFilter.HerdClassDesc != "")
                {
                    herdClassificationList = herdClassificationList.Where(herdClassification => herdClassification.HerdClassDesc.Contains(searchFilter.HerdClassDesc));

                }

                totalItems = herdClassificationList.ToList().Count();
                totalPages = (int)Math.Ceiling((double)totalItems / pagesize);
                items = herdClassificationList.Skip((page - 1) * pagesize).Take(pagesize).ToList();

                var result = new List<HerdClassificationPagedModel>();
                var item = new HerdClassificationPagedModel();

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
                return Ok(result);
            }

            catch (Exception ex)
            {
                
                return Problem(ex.GetBaseException().ToString());
            }
        }

        // GET: HerdClassification/search/5
        [HttpGet("{id}")]
        public async Task<ActionResult<HHerdClassification>> search(int id)
        {
            if (_context.HHerdClassifications == null)
            {
                return Problem("Entity set 'PCC_DEVContext.HerdClassification' is null!");
            }
            var HHerdClassification = await _context.HHerdClassifications.FindAsync(id);

            if (HHerdClassification == null || HHerdClassification.DeleteFlag)
            {
                return Conflict("No records found!");
            }

            return HHerdClassification;
        }

        // PUT: HerdClassification/update/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> update(int id, HHerdClassification HHerdClassification)
        {
            if (_context.HHerdClassifications == null)
            {
                return Problem("Entity set 'PCC_DEVContext.HerdClassification' is null!");
            }

            var herdType = _context.HHerdClassifications.AsNoTracking().Where(herdType => !herdType.DeleteFlag && herdType.Id == id).FirstOrDefault();

            if (herdType == null)
            {
                return Conflict("No records matched!");
            }

            if (id != HHerdClassification.Id)
            {
                return Conflict("Ids mismatched!");
            }

            bool hasDuplicateOnUpdate = (_context.HHerdClassifications.Any(hs => !hs.DeleteFlag && hs.HerdClassCode == HHerdClassification.HerdClassCode && hs.HerdClassDesc == HHerdClassification.HerdClassDesc && hs.Id != id));

            // check for duplication
            if (hasDuplicateOnUpdate)
            {
                return Conflict("Entity already exists");
            }

            try
            {
                _context.Entry(HHerdClassification).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return Ok("Update Successful!");
            }
            catch (Exception ex)
            {
                
                return Problem(ex.GetBaseException().ToString());
            }

        }

        // POST: HerdClassification/save
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<HHerdClassification>> save(HHerdClassification HHerdClassification)
        {
          if (_context.HHerdClassifications == null)
          {
            return Problem("Entity set 'PCC_DEVContext.HerdClassification' is null!");
          }

            bool hasDuplicateOnSave = (_context.HHerdClassifications?.Any(hs => !hs.DeleteFlag && hs.HerdClassCode == hs.HerdClassCode 
                                        && hs.HerdClassDesc == hs.HerdClassDesc)).GetValueOrDefault();

            // check for duplication
          if (hasDuplicateOnSave)
          {
              return Conflict("Entity already exists");
          }
          try
          {
                _context.HHerdClassifications.Add(HHerdClassification);
                await _context.SaveChangesAsync();
                return CreatedAtAction("save", new { id = HHerdClassification.Id }, HHerdClassification);
          }
          catch (Exception ex) 
          { 
                
                return Problem(ex.GetBaseException().ToString());
          }
        }

        // POST: HerdClassification/delete/5
        [HttpPost]
        public async Task<IActionResult> delete(DeletionModel deletionModel)
        {
            if (_context.HHerdClassifications == null)
            {
                return Problem("Entity set 'PCC_DEVContext.HerdClassification' is null!");
            }
            var HHerdClassification = await _context.HHerdClassifications.FindAsync(deletionModel.id);
            if (HHerdClassification == null || HHerdClassification.DeleteFlag)
            {
                return Conflict("No records found!");
            }

            bool typeCodeExistsInBuffHerd = _context.HBuffHerds.Any(buffHerd => !buffHerd.DeleteFlag && buffHerd.HerdClassCode == HHerdClassification.HerdClassCode);

            if (typeCodeExistsInBuffHerd)
            {
                return Conflict("Used by other table!");
            }

            try
            {
                HHerdClassification.DeleteFlag = true;
                HHerdClassification.DateDeleted = DateTime.Now;
                HHerdClassification.DeletedBy = deletionModel.deletedBy;
                HHerdClassification.DateRestored = null;
                HHerdClassification.RestoredBy = "";
                _context.Entry(HHerdClassification).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return Ok("Deletion Successful!");
            }
            catch(Exception ex)
            {
                
                return Problem(ex.GetBaseException().ToString());
            }
        }

        // GET: HerdClassification/view
        [HttpGet]
        public async Task<ActionResult<IEnumerable<HHerdClassification>>> view()
        {
            if (_context.HHerdClassifications == null)
            {
                return Problem("Entity set 'PCC_DEVContext.HerdClassification' is null!");
            }
            return await _context.HHerdClassifications.Where(HerdClassification => !HerdClassification.DeleteFlag).ToListAsync();
        }

        // POST: HerdClassification/restore/
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<IActionResult> restore(RestorationModel restorationModel)
        {

            if(_context.HHerdClassifications == null)
            {
                return Problem("Entity set 'PCC_DEVContext.HerdClassification' is null!");
            }

            var HHerdClassification = await _context.HHerdClassifications.FindAsync(restorationModel.id);
            if (HHerdClassification == null || !HHerdClassification.DeleteFlag)
            {
                return Conflict("No deleted records matched!");
            }

            try
            {
                HHerdClassification.DeleteFlag = !HHerdClassification.DeleteFlag;
                HHerdClassification.DateDeleted = null;
                HHerdClassification.DeletedBy = "";
                HHerdClassification.DateRestored = DateTime.Now;
                HHerdClassification.RestoredBy = restorationModel.restoredBy;

                _context.Entry(HHerdClassification).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return Ok("Restoration Successful!");
            }
            catch (Exception ex) 
            {
                
                return Problem(ex.GetBaseException().ToString());
            }
        }

        private bool HHerdClassificationExists(int id)
        {
            return (_context.HHerdClassifications?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
