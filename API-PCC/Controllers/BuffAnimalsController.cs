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
using NuGet.Protocol.Core.Types;
using System;

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

        // POST: BuffAnimals/list
        [HttpPost]
        public async Task<ActionResult<IEnumerable<ABuffAnimal>>> list(BuffAnimalSearchFilterModel searchFilter)
        {
            try
            {
                DataTable dt = db.SelectDb(QueryBuilder.buildBuffAnimalSearch(searchFilter)).Tables[0];
                var result = buildBuffAnimalPagedModel(searchFilter, dt);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Problem(ex.GetBaseException().ToString());
            }
        }

        // GET: BuffAnimals/search/5
        // search by registrationNumber and RFID number
        [HttpGet("{referenceNumber}")]
        public async Task<ActionResult<ABuffAnimal>> view(String referenceNumber)
        {
            DataTable dt = db.SelectDb(QueryBuilder.buildBuffAnimalSearchByReferenceNumber(referenceNumber)).Tables[0];

            if (dt.Rows.Count == 0)
            {
                return Conflict("No records found!");
            }

            return Ok(DataRowToObject.ToObject<ABuffAnimal>(dt.Rows[0]));
        }

        // PUT: BuffAnimals/update/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> update(int id, BuffAnimalUpdateModel updateModel)
        {
            DataTable buffAnimalDataTable = db.SelectDb(QueryBuilder.buildBuffAnimalSearchById(id)).Tables[0];

            if (buffAnimalDataTable.Rows.Count == 0)
            {
                return Conflict("No records matched!");
            }

            DataTable buffAnimalDuplicateCheck = db.SelectDb(QueryBuilder.buildBuffAnimalSelectDuplicateQueryByIdAnimalIdNumberName(id, updateModel.AnimalIdNumber, updateModel.AnimalName)).Tables[0];

            // check for duplication
            if (buffAnimalDuplicateCheck.Rows.Count > 0)
            {
                return Conflict("Entity already exists");
            }

            DataTable sireRecordsCheck = db.SelectDb(QueryBuilder.buildSireSearchQueryById(updateModel.Sire.id)).Tables[0];

            if (sireRecordsCheck.Rows.Count == 0)
            {
                return Conflict("Sire does not exists");
            }

            string sire_update = $@"UPDATE [dbo].[tbl_SireModel] SET 
                                             [Sire_Registration_Number] = '" + updateModel.Sire.SireRegistrationNumber + "'" +
                                            ",[Sire_Id_Number] = '" + updateModel.Sire.SireIdNumber + "'" +
                                            ",[Sire_Name] = '" + updateModel.Sire.SireName + "'" +
                                            ",[Breed_Code] = '" + updateModel.Sire.BreedCode + "'" +
                                            ",[Blood_Code] = '" + updateModel.Sire.BloodCode + "'" +
                                            " WHERE id = " + updateModel.Sire.id;
            string sireUpdateResult = db.DB_WithParam(sire_update);

            DataTable damRecordsCheck = db.SelectDb(QueryBuilder.buildSireSearchQueryById(updateModel.Dam.id)).Tables[0];

            if (damRecordsCheck.Rows.Count == 0)
            {
                return Conflict("Dam does not exists");
            }

            string dam_update = $@"UPDATE [dbo].[tbl_DamModel] SET 
                                             [Dam_Registration_Number] = '" + updateModel.Dam.DamRegistrationNumber + "'" +
                                            ",[Dam_Id_Number] = '" + updateModel.Dam.DamIdNumber + "'" +
                                            ",[Dam_Name] = '" + updateModel.Dam.DamName + "'" +
                                            ",[Breed_Code] = '" + updateModel.Dam.BreedCode + "'" +
                                            ",[Blood_Code] = '" + updateModel.Dam.BloodCode + "'" +
                                            " WHERE id = " + updateModel.Dam.id;
            string damUpdateResult = db.DB_WithParam(dam_update);

            var buffAnimal = convertDataRowToBuffAnimalModel(buffAnimalDataTable.Rows[0]);

            try
            {
                buffAnimal = populateBuffAnimal(buffAnimal, updateModel);
                buffAnimal.UpdateDate = DateTime.Now;
                buffAnimal.UpdatedBy = updateModel.UpdatedBy;

                _context.Entry(buffAnimal).State = EntityState.Modified;
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

                DataTable duplicateCheck = db.SelectDb(QueryBuilder.buildBuffAnimalDuplicateQuery(buffAnimalRegistrationModel)).Tables[0];

                if (duplicateCheck.Rows.Count > 0)
                {
                    return Conflict("Buff Animal already exists");
                }

                var buffAnimal = buildBuffAnimal(buffAnimalRegistrationModel);

                _context.ABuffAnimals.Add(buffAnimal);
                await _context.SaveChangesAsync();

                return CreatedAtAction("save", new { id = buffAnimal.Id }, buffAnimal);
            }
            catch (Exception ex)
            {

                return Problem(ex.GetBaseException().ToString());
            }
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

        private List<BuffAnimalPagedModel> buildBuffAnimalPagedModel(BuffAnimalSearchFilterModel searchFilter, DataTable dt)
        {

            int pagesize = searchFilter.pageSize == 0 ? 10 : searchFilter.pageSize;
            int page = searchFilter.page == 0 ? 1 : searchFilter.page;
            var items = (dynamic)null;

            int totalItems = dt.Rows.Count;
            int totalPages = (int)Math.Ceiling((double)totalItems / pagesize);
            items = dt.AsEnumerable().Skip((page - 1) * pagesize).Take(pagesize).ToList();

            var buffAnimal = convertDataRowListToBuffAnimalModelList(items);

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
            item.items = buffAnimal;
            result.Add(item);

            return result;
        }

        private List<ABuffAnimal> convertDataRowListToBuffAnimalModelList(List<DataRow> dataRowList)
        {
            var buffAnimalList = new List<ABuffAnimal>();

            foreach (DataRow row in dataRowList)
            {
                var buuffAnimalModel = DataRowToObject.ToObject<ABuffAnimal>(row);
                buffAnimalList.Add(buuffAnimalModel);
            }

            return buffAnimalList;
        }

        private ABuffAnimal convertDataRowToBuffAnimalModel(DataRow dataRow)
        {
            var buuffAnimalModel = DataRowToObject.ToObject<ABuffAnimal>(dataRow);
            return buuffAnimalModel;
        }

        private ABuffAnimal populateBuffAnimal(ABuffAnimal buffAnimal, BuffAnimalUpdateModel updateModel)
        {
            if (updateModel.AnimalIdNumber != null && updateModel.AnimalIdNumber != "")
            {
                buffAnimal.AnimalIdNumber = updateModel.AnimalIdNumber;
            }
            if (updateModel.AnimalName != null && updateModel.AnimalName != "")
            {
                buffAnimal.AnimalName = updateModel.AnimalName;
            }
            if (updateModel.Photo != null && updateModel.Photo != "")
            {
                buffAnimal.Photo = updateModel.Photo;
            }
            if (updateModel.HerdCode != null && updateModel.HerdCode != "")
            {
                buffAnimal.HerdCode = updateModel.HerdCode;
            }
            if (updateModel.RfidNumber != null && updateModel.RfidNumber != "")
            {
                buffAnimal.RfidNumber = updateModel.RfidNumber;
            }
            if (updateModel.DateOfBirth != null)
            {
                buffAnimal.DateOfBirth = updateModel.DateOfBirth;
            }
            if (updateModel.Sex != null && updateModel.Sex != "")
            {
                buffAnimal.Sex = updateModel.Sex;
            }
            if (updateModel.BreedCode != null && updateModel.BreedCode != "")
            {
                buffAnimal.BreedCode = updateModel.BreedCode;
            }
            if (updateModel.BirthType != null && updateModel.BirthType != "")
            {
                buffAnimal.BirthType = updateModel.BirthType;
            }
            if (updateModel.CountryOfBirth != null && updateModel.CountryOfBirth != "")
            {
                buffAnimal.CountryOfBirth = updateModel.CountryOfBirth;
            }
            if (updateModel.OriginOfAcquisition != null && updateModel.OriginOfAcquisition != "")
            {
                buffAnimal.OriginOfAcquisition = updateModel.OriginOfAcquisition;
            }
            if (updateModel.DateOfAcquisition != null)
            {
                buffAnimal.DateOfAcquisition = updateModel.DateOfAcquisition;
            }
            if (updateModel.Marking != null && updateModel.Marking != "")
            {
                buffAnimal.Marking = updateModel.Marking;
            }
            if (updateModel.TypeOfOwnership != null && updateModel.TypeOfOwnership != "")
            {
                buffAnimal.TypeOfOwnership = updateModel.TypeOfOwnership;
            }
            if (updateModel.BloodCode != null && updateModel.BloodCode != "")
            {
                buffAnimal.BloodCode = updateModel.BloodCode;
            }
            return buffAnimal;
        }


        private ABuffAnimal buildBuffAnimal(BuffAnimalRegistrationModel registrationModel)
        {
            var buffAnimal = new ABuffAnimal()
            {
                AnimalIdNumber = registrationModel.AnimalIdNumber,
                AnimalName = registrationModel.AnimalName,
                Photo = registrationModel.Photo,
                HerdCode = registrationModel.HerdCode,
                RfidNumber = registrationModel.RfidNumber,
                DateOfBirth = registrationModel.DateOfBirth,
                Sex = registrationModel.Sex,
                BreedCode = registrationModel.BreedCode,
                BirthType = registrationModel.BirthType,
                CountryOfBirth = registrationModel.CountryOfBirth,
                OriginOfAcquisition = registrationModel.OriginOfAcquisition,
                DateOfAcquisition = registrationModel.DateOfAcquisition,
                Marking = registrationModel.Marking,
                SireId = registrationModel.Sire.id,
                DamId = registrationModel.Dam.id
            };
            return buffAnimal;
        }

    }
}
