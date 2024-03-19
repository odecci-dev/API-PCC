﻿using API_PCC.ApplicationModels;
using API_PCC.ApplicationModels.Common;
using API_PCC.Data;
using API_PCC.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static API_PCC.Manager.DBMethods;
using System.Data;
using API_PCC.Manager;
using API_PCC.Utils;

namespace API_PCC.Controllers
{
    [Authorize("ApiKey")]
    [Route("[controller]/[action]")]
    [ApiController]
    public class BuffAnimalsController : ControllerBase
    {
        private readonly PCC_DEVContext _context;
        DbManager db = new DbManager();

        public BuffAnimalsController(PCC_DEVContext context)
        {
            _context = context;
        }
        public class BuffAnimalSearchFilter
        {
            public string? AnimalId { get; set; }
            public string? Name { get; set; }
            public int page { get; set; }
            public int pageSize { get; set; }
        }

        // POST: BuffAnimals/list
        [HttpPost]
        public async Task<ActionResult<IEnumerable<ABuffAnimal>>> list(BuffAnimalSearchFilter searchFilter)
        {
            if (_context.ABuffAnimals == null)
            {
                return Problem("Entity set 'PCC_DEVContext.BuffAnimal' is null!");
            }
            int pagesize = searchFilter.pageSize == 0 ? 10 : searchFilter.pageSize;
            int page = searchFilter.page == 0 ? 1 : searchFilter.page;
            var items = (dynamic)null;
            int totalItems = 0;
            int totalPages = 0;


            var buffAnimalList = _context.ABuffAnimals.AsNoTracking();
            buffAnimalList = buffAnimalList.Where(buffAnimal => !buffAnimal.DeleteFlag);
            try
            {
                if (searchFilter.AnimalId != null && searchFilter.AnimalId != "")
                {
                    buffAnimalList = buffAnimalList.Where(buffAnimal => buffAnimal.AnimalId.Contains(searchFilter.AnimalId));
                }

                if (searchFilter.Name != null && searchFilter.Name != "")
                {
                    buffAnimalList = buffAnimalList.Where(buffAnimal => buffAnimal.Name.Contains(searchFilter.Name));
                }

                totalItems = buffAnimalList.ToList().Count();
                totalPages = (int)Math.Ceiling((double)totalItems / pagesize);
                items = buffAnimalList.Skip((page - 1) * pagesize).Take(pagesize).ToList();

                var result = new List<BuffAnimalPagedModel>();
                var item = new BuffAnimalPagedModel();

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

        // GET: BuffAnimals/search/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ABuffAnimal>> search(int id)
        {
            if (_context.ABuffAnimals == null)
            {
                return Problem("Entity set 'PCC_DEVContext.BuffAnimal' is null!");
            }
            var aBuffAnimal = await _context.ABuffAnimals.FindAsync(id);

            if (aBuffAnimal == null || aBuffAnimal.DeleteFlag)
            {
                return Conflict("No records found!");
            }
            return Ok(aBuffAnimal);
        }

        // PUT: BuffAnimals/update/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> update(int id, ABuffAnimal aBuffAnimal)
        {
            if (id != aBuffAnimal.Id)
            {
                return BadRequest();
            }

            var buffAnimal = _context.ABuffAnimals.AsNoTracking().Where(buffAnimal => !buffAnimal.DeleteFlag && buffAnimal.Id == id).FirstOrDefault();

            if (buffAnimal == null)
            {
                return Conflict("No records matched!");
            }

            if (id != aBuffAnimal.Id)
            {
                return Conflict("Ids mismatched!");
            }

            bool hasDuplicateOnUpdate = (_context.ABuffAnimals?.Any(buffAnimal => !buffAnimal.DeleteFlag && buffAnimal.AnimalId == aBuffAnimal.AnimalId && buffAnimal.Name == aBuffAnimal.Name && buffAnimal.Id != id)).GetValueOrDefault();

            // check for duplication
            if (hasDuplicateOnUpdate)
            {
                return Conflict("Entity already exists");
            }

            try
            {
                _context.Entry(aBuffAnimal).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                return Ok("Update Successful!");
            }
            catch (Exception ex)
            {
                
                return Problem(ex.GetBaseException().ToString());
            }
        }

        // POST: BuffAnimals/save
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<ABuffAnimal>> save(BuffAnimalRegistrationModel buffAnimalRegistrationModel)
        {
            try
            {
                DataTable duplicateCheck = db.SelectDb(QueryBuilder.buildBuffAnimalSearch(buffAnimalRegistrationModel.AnimalId, buffAnimalRegistrationModel.Name)).Tables[0];

                if (duplicateCheck.Rows.Count > 0)
                {
                    return Conflict("Buff Animal already exists");
                }

                var buffAnimal = buildBuffAnimal(buffAnimalRegistrationModel);

                _context.ABuffAnimals.Add(buffAnimal);
                await _context.SaveChangesAsync();

                return CreatedAtAction("save", new { id = buffAnimal.AnimalId }, buffAnimal);
            }
            catch (Exception ex)
            {
                
                return Problem(ex.GetBaseException().ToString());
            }
        }

        private ABuffAnimal buildBuffAnimal(BuffAnimalRegistrationModel registrationModel)
        {
            var buffAnimal = new ABuffAnimal()
            {
                AnimalId = registrationModel.AnimalId,
                Name = registrationModel.Name,
                Rfid = registrationModel.Rfid,
                HerdCode = registrationModel.HerdCode,
                DateOfBirth = registrationModel.DateOfBirth,
                Sex = registrationModel.Sex,
                BuffaloType = registrationModel.BuffaloType,
                IdSystem = registrationModel.IdSystem,
                PedigreeRecords = registrationModel.PedigreeRecords,
                Photo = registrationModel.Photo,
                CountryBirth = registrationModel.CountryBirth,
                OriginAcquisition = registrationModel.OriginAcquisition,
                DateAcquisition = registrationModel.DateAcquisition,
                Marking = registrationModel.Marking,
                SireIdNum = registrationModel.Sire.SireRegNum,
                BreedCode = registrationModel.BreedCode,
                BloodCode = registrationModel.BloodCode,
                BirthTypeCode = registrationModel.BirthTypeCode,
                TypeOwnCode = registrationModel.TypeOwnCode,
                CreatedBy = registrationModel.CreatedBy
            };
            return buffAnimal;
        }
        // POST: BuffAnimals/delete/5
        [HttpPost]
        public async Task<IActionResult> delete(DeletionModel deletionModel)
        {
            if (_context.ABuffAnimals == null)
            {
                return NotFound();
            }
            var aBuffAnimal = await _context.ABuffAnimals.FindAsync(deletionModel.id);
            if (aBuffAnimal == null || aBuffAnimal.DeleteFlag)
            {
                return Conflict("No records matched!");
            }

            try
            {
                aBuffAnimal.DeleteFlag = true;
                aBuffAnimal.DateDeleted = DateTime.Now;
                aBuffAnimal.DeletedBy = deletionModel.deletedBy;
                aBuffAnimal.DateRestored = null;
                aBuffAnimal.RestoredBy = "";
                _context.Entry(aBuffAnimal).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                return Ok("Deletion Successful!");
            }
            catch (Exception ex)
            {
                
                return Problem(ex.GetBaseException().ToString());
            }
        }

        // GET: FeedingSystems/view
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ABuffAnimal>>> view()
        {
            if (_context.ABuffAnimals == null)
            {
                return Problem("Entity set 'PCC_DEVContext.ABuffAnimals' is null.");
            }
            return await _context.ABuffAnimals.Where(buffAnimal => !buffAnimal.DeleteFlag).ToListAsync();
        }


        // POST: BuffAnimals/restore/
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<IActionResult> restore(RestorationModel restorationModel)
        {

            if (_context.ABuffAnimals == null)
            {
                return Problem("Entity set 'PCC_DEVContext.BuffAnimal' is null!");
            }

            var aBuffAnimal = await _context.ABuffAnimals.FindAsync(restorationModel.id);
            if (aBuffAnimal == null || !aBuffAnimal.DeleteFlag)
            {
                return Conflict("No deleted records matched!");
            }

            try
            {
                aBuffAnimal.DeleteFlag = !aBuffAnimal.DeleteFlag;
                aBuffAnimal.DateDeleted = null;
                aBuffAnimal.DeletedBy = "";
                aBuffAnimal.DateRestored = DateTime.Now;
                aBuffAnimal.RestoredBy = restorationModel.restoredBy;

                _context.Entry(aBuffAnimal).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return Ok("Restoration Successful!");
            }
            catch (Exception ex)
            {
                
                return Problem(ex.GetBaseException().ToString());
            }
        }

            private bool ABuffAnimalExists(int id)
        {
            return (_context.ABuffAnimals?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
