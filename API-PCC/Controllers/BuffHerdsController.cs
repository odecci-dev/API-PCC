using API_PCC.ApplicationModels;
using API_PCC.ApplicationModels.Common;
using API_PCC.Data;
using API_PCC.DtoModels;
using API_PCC.EntityModels;
using API_PCC.Manager;
using API_PCC.Models;
using API_PCC.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq.Dynamic.Core;

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

        // POST: BuffHerds/search
        [HttpPost]
        public async Task<ActionResult<IEnumerable<HerdPagedModel>>> search(BuffHerdSearchFilterModel searchFilter)
        {
            validateDate(searchFilter);
            if (!searchFilter.sortBy.Field.IsNullOrEmpty())
            {
                if (searchFilter.sortBy.Field.ToLower().Equals("cowlevel"))
                {
                    searchFilter.sortBy.Field = "HerdSize";
                }
            }
            try
            {
                List<HBuffHerd> buffHerdList = await buildHerdSearchQuery(searchFilter).ToListAsync();
                var result = buildHerdPagedModel(searchFilter, buffHerdList);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Problem(ex.GetBaseException().ToString());
            }
        }

        private IQueryable<HBuffHerd> buildHerdSearchQuery(BuffHerdSearchFilterModel searchFilter)
        {
            IQueryable<HBuffHerd> query = _context.HBuffHerds;

            query = query
                .Include(herd => herd.buffaloType)
                .Include(herd => herd.feedingSystem);

            // assuming that you return all records when nothing is specified in the filter

            if (!searchFilter.searchValue.IsNullOrEmpty())
                query = query.Where(herd => 
                               herd.HerdCode.Contains(searchFilter.searchValue) ||
                               herd.HerdName.Contains(searchFilter.searchValue));

            if (!searchFilter.filterBy.BreedTypeCode.IsNullOrEmpty())
                query = query.Where(herd => herd.buffaloType.Any(buffaloType => buffaloType.BreedTypeCode.Equals(searchFilter.filterBy.BreedTypeCode)));

            if (!searchFilter.filterBy.HerdClassDesc.IsNullOrEmpty())
                query = query.Where(herd => herd.HerdClassDesc.Equals(searchFilter.filterBy.HerdClassDesc));

            if (!searchFilter.filterBy.feedingSystemCode.IsNullOrEmpty())
                query = query.Where(herd => herd.feedingSystem.Any(feedingSystem => feedingSystem.FeedingSystemCode.Equals(searchFilter.filterBy.feedingSystemCode)));

            if (!searchFilter.dateFrom.IsNullOrEmpty())
                query = query.Where(herd => herd.DateCreated >= DateTime.Parse(searchFilter.dateFrom));

            if (!searchFilter.dateTo.IsNullOrEmpty())
                query = query.Where(herd => herd.DateCreated <= DateTime.Parse(searchFilter.dateTo));


            if (!searchFilter.sortBy.Field.IsNullOrEmpty())
            {
                
                if (!searchFilter.sortBy.Sort.IsNullOrEmpty())
                {
                    query = query.OrderBy(searchFilter.sortBy.Field + " " + searchFilter.sortBy.Sort);
                } else
                {
                    query = query.OrderBy(searchFilter.sortBy.Field + " asc");

                }
            } else
            {
                query = query.OrderByDescending(herd => herd.Id);
            }

            return query;
        }

        // GET: BuffHerds/view/5
        [HttpGet("{herdCode}")]
        public async Task<ActionResult<BuffHerdViewResponseModel>> view(String herdCode)
        {
            var buffHerdModel = await _context.HBuffHerds
                .Include(herd => herd.buffaloType)
                .Include(herd => herd.feedingSystem)
                .Where(herd => !herd.DeleteFlag && herd.HerdCode.Equals(herdCode))
                .FirstOrDefaultAsync();

            if (buffHerdModel == null)
            {
                return Conflict("No records found!");
            }
            var viewResponseModel = populateViewResponseModel(buffHerdModel);
            return Ok(viewResponseModel);
        }

        private IQueryable<HBuffHerd> buildHerdArchiveQuery(BuffHerdSearchFilterModel searchFilter)
        {
            IQueryable<HBuffHerd> query = _context.HBuffHerds;

            query = query
                .Include(herd => herd.buffaloType)
                .Include(herd => herd.feedingSystem);

            query = query.Where(herd => herd.DeleteFlag);

            return query;
        }

        // GET: BuffHerds/archive
        [HttpPost]
        public async Task<ActionResult<IEnumerable<HBuffHerd>>> archive(BuffHerdSearchFilterModel searchFilter)
        {
            List<HBuffHerd> buffHerdList = await buildHerdArchiveQuery(searchFilter).ToListAsync();

            var result = buildHerdPagedModel(searchFilter, buffHerdList);
            return Ok(result);
        }

        // PUT: BuffHerds/update/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> update(int id, BuffHerdUpdateModel registrationModel)
        {
            var buffHerdRecord = _context.HBuffHerds.Where(buffHerd => !buffHerd.DeleteFlag &&
                                                                        buffHerd.Id.Equals(id)).FirstOrDefault();
            if (buffHerdRecord == null)
            {
                return Conflict("No records matched!");
            }

            var herdClassificationRecord = _context.HHerdClassifications.Where(herdClassification => !herdClassification.DeleteFlag &&
                                                                                                      herdClassification.HerdClassDesc.Equals(registrationModel.HerdClassDesc)).FirstOrDefault();
            if (herdClassificationRecord == null)
            {
                return Conflict("No Herd Classification records matched!");
            }

            var buffHerdDuplicateCheck = _context.HBuffHerds.Where(buffHerd => !buffHerd.DeleteFlag &&
                                                                                buffHerd.Id.Equals(id) &&
                                                                                buffHerd.HerdName.Equals(registrationModel.HerdName) &&
                                                                                buffHerd.HerdCode.Equals(registrationModel.HerdCode)).FirstOrDefault();

            // check for duplication
            if (buffHerdDuplicateCheck != null)
            {
                return Conflict("Entity already exists");   
            }

            var buffHerd = _context.HBuffHerds
                    .Include(x => x.buffaloType)
                    .Include(x => x.feedingSystem)
                    .Single(x => x.Id == id);

            var farmOwnerRecord = _context.TblFarmOwners.Where(farmOwner => farmOwner.Id.Equals(buffHerd.Owner)).FirstOrDefault();

            if (farmOwnerRecord == null)
            {
                return Conflict("Farm owner does not exists");
            }

            farmOwnerRecord.FirstName = registrationModel.Owner.FirstName;
            farmOwnerRecord.LastName = registrationModel.Owner.LastName;
            farmOwnerRecord.Address = registrationModel.Owner.Address;
            farmOwnerRecord.TelephoneNumber = registrationModel.Owner.TelNo;
            farmOwnerRecord.MobileNumber = registrationModel.Owner.MNo;
            farmOwnerRecord.Email = registrationModel.Owner.Email;

            _context.Entry(farmOwnerRecord).State = EntityState.Modified;
            await _context.SaveChangesAsync();


            try
            {
                buffHerd = populateBuffHerd(buffHerd, registrationModel);

                buffHerd.buffaloType.Clear();
                buffHerd.feedingSystem.Clear();

                populateFeedingSystemAndBuffaloType(buffHerd, registrationModel);

                buffHerd.Owner = farmOwnerRecord.Id;
                buffHerd.DateUpdated = DateTime.Now;
                buffHerd.UpdatedBy = registrationModel.UpdatedBy;

                _context.Entry(buffHerd).State = EntityState.Modified;
                _context.SaveChanges();

                return Ok("Update Successful!");
            }
            catch (Exception ex)
            {

                return Problem(ex.GetBaseException().ToString());
            }
        }

        // POST: BuffHerds/save
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<HBuffHerd>> save(BuffHerdRegistrationModel registrationModel)
        {

            try
            {
                var buffHerdDuplicateCheck = _context.HBuffHerds.Where(buffHerd => !buffHerd.DeleteFlag &&
                                                                                   (buffHerd.HerdName.Equals(registrationModel.HerdName) ||
                                                                                    buffHerd.HerdCode.Equals(registrationModel.HerdCode))).FirstOrDefault();
                if (buffHerdDuplicateCheck != null)
                {
                    return Conflict("Herd already exists");
                }

                var BuffHerdModel = buildBuffHerd(registrationModel);

                var farmOwnerRecordsCheck = _context.TblFarmOwners.Where(farmOwner => farmOwner.FirstName.Equals(registrationModel.Owner.FirstName) &&
                                                                                      farmOwner.LastName.Equals(registrationModel.Owner.LastName)).FirstOrDefault();

                if (farmOwnerRecordsCheck == null)
                {
                    // Create new Farm Owner Record
                    var farmOwner = new TblFarmOwner()
                    {
                        FirstName = registrationModel.Owner.FirstName,
                        LastName = registrationModel.Owner.LastName,
                        Address = registrationModel.Owner.Address,
                        TelephoneNumber = registrationModel.Owner.TelNo,
                        MobileNumber = registrationModel.Owner.MNo,
                        Email = registrationModel.Owner.Email
                    };

                    _context.Entry(farmOwner).State = EntityState.Modified;
                    _context.SaveChanges();

                    BuffHerdModel.Owner = farmOwner.Id;
                } else
                {
                    BuffHerdModel.Owner = farmOwnerRecordsCheck.Id;
                }

                populateFeedingSystemAndBuffaloType(BuffHerdModel, registrationModel);

                BuffHerdModel.CreatedBy = registrationModel.CreatedBy;
                BuffHerdModel.DateCreated = DateTime.Now;

                _context.HBuffHerds.Add(BuffHerdModel);
                await _context.SaveChangesAsync();

                return Ok("Herd successfully registered!");
            }
            catch (Exception ex)
            {

                return Problem(ex.GetBaseException().ToString());
            }
        }

        private void populateFeedingSystemAndBuffaloType(HBuffHerd buffHerd, BuffHerdBaseModel baseModel)
        {
            var buffaloTypes = new List<HBuffaloType>();
            var feedingSystems = new List<HFeedingSystem>();

            foreach (string breedTypeCode in baseModel.BreedTypeCodes)
            {
                var buffaloType = _context.HBuffaloTypes.Where(buffaloType => !buffaloType.DeleteFlag &&
                                                                               buffaloType.BreedTypeCode.Equals(breedTypeCode)).FirstOrDefault();
                if (buffaloType == null)
                {
                    break;
                }
                _context.Attach(buffaloType);
                buffHerd.buffaloType.Add(buffaloType);
            }

            foreach (string feedingSystemCode in baseModel.FeedingSystemCodes)
            {
                var feedingSystem = _context.HFeedingSystems.Where(feedingSystem => !feedingSystem.DeleteFlag &&
                                                                                     feedingSystem.FeedingSystemCode.Equals(feedingSystemCode)).FirstOrDefault();
                if (feedingSystem == null)
                {
                    break;
                }
                _context.Attach(feedingSystem);
                buffHerd.feedingSystem.Add(feedingSystem);
            }
        }

        private TblFarmOwner convertDataRowToFarmOwnerEntity(DataRow dataRow)
        {
            var farmOwner = DataRowToObject.ToObject<TblFarmOwner>(dataRow);

            return farmOwner;
        }

        // DELETE: BuffHerds/delete/5
        [HttpPost]
        public async Task<IActionResult> delete(DeletionModel deletionModel)
        {
            if (_context.HBuffHerds == null)
            {
                return NotFound();
            }
            var hBuffHerd = await _context.HBuffHerds.FindAsync(deletionModel.id);
            if (hBuffHerd == null || hBuffHerd.DeleteFlag)
            {
                return Conflict("No records matched!");
            }

            try
            {
                hBuffHerd.DeleteFlag = true;
                hBuffHerd.DateDeleted = DateTime.Now;
                hBuffHerd.DeletedBy = deletionModel.deletedBy;
                hBuffHerd.DateRestored = null;
                hBuffHerd.RestoredBy = "";
                _context.Entry(hBuffHerd).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                return Ok("Deletion Successful!");
            }
            catch (Exception ex)
            {

                return Problem(ex.GetBaseException().ToString());
            }
        }

        // POST: BuffHerds/restore/
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<IActionResult> restore(RestorationModel restorationModel)
        {

            var buffHerd = _context.HBuffHerds.Where(buffHerd => buffHerd.DeleteFlag && 
                                                                 buffHerd.Id.Equals(restorationModel.id)).FirstOrDefault();
            if (buffHerd == null)
            {
                return Conflict("No deleted records matched!");
            }

            var buffHerdDuplicateCheck = _context.HBuffHerds.Where(buffHerd => !buffHerd.DeleteFlag &&
                                                                               (buffHerd.HerdName.Equals(buffHerd.HerdName) ||
                                                                                buffHerd.HerdCode.Equals(buffHerd.HerdCode))).FirstOrDefault();
            if (buffHerdDuplicateCheck != null)
            {
                return Conflict("Entity already exists!!");
            }

            try
            {
                buffHerd.DeleteFlag = !buffHerd.DeleteFlag;
                buffHerd.DateDeleted = null;
                buffHerd.DeletedBy = "";
                buffHerd.DateRestored = DateTime.Now;
                buffHerd.RestoredBy = restorationModel.restoredBy;

                _context.Entry(buffHerd).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return Ok("Restoration Successful!");
            }
            catch (Exception ex)
            {

                return Problem(ex.GetBaseException().ToString());
            }
        }

        private List<HerdPagedModel> buildHerdPagedModel(BuffHerdSearchFilterModel searchFilter, List<HBuffHerd> buffHerdList)
        {

            int pagesize = searchFilter.pageSize == 0 ? 10 : searchFilter.pageSize;
            int page = searchFilter.page == 0 ? 1 : searchFilter.page;
            var items = (dynamic)null;

            int totalItems = buffHerdList.Count;
            int totalPages = (int)Math.Ceiling((double)totalItems / pagesize);
            items = buffHerdList.Skip((page - 1) * pagesize).Take(pagesize).ToList();

            List<BuffHerdListResponseModel> buffHerdBaseModels = convertBuffHerdToResponseModelList(buffHerdList);

            var result = new List<HerdPagedModel>();
            var item = new HerdPagedModel();

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
            item.items = buffHerdBaseModels;
            result.Add(item);

            return result;
        }

        private List<HBuffHerd> convertDataRowListToHerdModelList(List<DataRow> dataRowList)
        {
            var herdModelList = new List<HBuffHerd>();

            foreach (DataRow dataRow in dataRowList)
            {
                var herdModel = DataRowToObject.ToObject<HBuffHerd>(dataRow);
                herdModelList.Add(herdModel);
            }

            return herdModelList;
        }

        private HBuffHerd convertDataRowToHerdModel(DataRow dataRow)
        {
            return DataRowToObject.ToObject<HBuffHerd>(dataRow);
        }

        private HHerdClassification convertDataRowToHerdClassification(DataRow dataRow)
        {
            return DataRowToObject.ToObject<HHerdClassification>(dataRow);
        }

        private HBuffaloType convertDataRowToBuffaloType(DataRow dataRow)
        {
            return DataRowToObject.ToObject<HBuffaloType>(dataRow);
        }

        private HFeedingSystem convertDataRowToFeedingSystem(DataRow dataRow)
        {
            return DataRowToObject.ToObject<HFeedingSystem>(dataRow);
        }


        private HBuffHerd populateBuffHerd(HBuffHerd buffHerd, BuffHerdUpdateModel updateModel)
        {

            if (updateModel.HerdName != null && updateModel.HerdName != "")
            {
                buffHerd.HerdName = updateModel.HerdName;
            }
            if (updateModel.HerdCode != null && updateModel.HerdCode != "")
            {
                buffHerd.HerdCode = updateModel.HerdCode;
            }
            if (updateModel.FarmAffilCode != null && updateModel.FarmAffilCode != "")
            {
                buffHerd.FarmAffilCode = updateModel.FarmAffilCode;
            }
            if (updateModel.HerdClassDesc != null && updateModel.HerdClassDesc != "")
            {
                buffHerd.HerdClassDesc = updateModel.HerdClassDesc;
            }
            if (updateModel.FarmManager != null && updateModel.FarmManager != "")
            {
                buffHerd.FarmManager = updateModel.FarmManager;
            }
            if (updateModel.FarmAddress != null && updateModel.FarmAddress != "")
            {
                buffHerd.FarmAddress = updateModel.FarmAddress;
            }
            if (updateModel.OrganizationName != null && updateModel.OrganizationName != "")
            {
                buffHerd.OrganizationName = updateModel.OrganizationName;
            }
            return buffHerd;
        }


        private HBuffHerd buildBuffHerd(BuffHerdBaseModel registrationModel)
        {
            var BuffHerdModel = new HBuffHerd()
            {
                HerdName = registrationModel.HerdName,
                HerdCode = registrationModel.HerdCode,
                HerdSize = registrationModel.HerdSize,
                FarmAffilCode = registrationModel.FarmAffilCode,
                HerdClassDesc = registrationModel.HerdClassDesc,
                FarmManager = registrationModel.FarmManager,
                FarmAddress = registrationModel.FarmAddress,
                OrganizationName = registrationModel.OrganizationName,
                Center = registrationModel.Center,
                Photo = registrationModel.Photo
            };

            return BuffHerdModel;
        }

        private List<BuffHerdListResponseModel> convertBuffHerdToResponseModelList(List<HBuffHerd> buffHerdList)
        {
            var buffHerdResponseModels = new List<BuffHerdListResponseModel>();
            foreach (HBuffHerd buffHerd in buffHerdList)
            {
                var buffHerdResponseModel = new BuffHerdListResponseModel()
                {
                    HerdName = buffHerd.HerdName,
                    HerdClassification = buffHerd.HerdClassDesc,
                    CowLevel = buffHerd.HerdSize.ToString(),
                    FarmManager = buffHerd.FarmManager,
                    HerdCode = buffHerd.HerdCode,
                    Photo = buffHerd.Photo,
                    DateOfApplication = buffHerd.DateCreated.ToString("yyyy-MM-dd")
                };
                buffHerdResponseModels.Add(buffHerdResponseModel);
            }
           
            return buffHerdResponseModels;
        }   

        private BuffHerdListResponseModel convertBuffHerdToResponseModel(HBuffHerd buffHerd)
        {
            var buffHerdResponseModel = new BuffHerdListResponseModel()
            {
                HerdName = buffHerd.HerdName,
                HerdClassification = buffHerd.HerdClassDesc,
                CowLevel = buffHerd.HerdSize.ToString(),
                FarmManager = buffHerd.FarmManager,
                HerdCode = buffHerd.HerdCode,
                
                Photo = buffHerd.Photo,
                DateOfApplication = buffHerd.DateCreated.ToString("yyyy-MM-dd")
            };
            return buffHerdResponseModel;
        }

        private Owner populateOwner(int ownerId)
        {
            var farmOwner = _context.TblFarmOwners.Where(farmOwner => farmOwner.Id.Equals(ownerId)).FirstOrDefault();
            if (farmOwner == null)
            {
                return new Owner()
                {
                    FirstName = string.Empty,
                    LastName = string.Empty,
                    Address = string.Empty,
                    Email = string.Empty,
                    MNo = string.Empty,
                    TelNo = string.Empty
                };
            }
            var owner = new Owner()
            {
                FirstName = farmOwner.FirstName,
                LastName = farmOwner.LastName,
                Address = farmOwner.Address,
                Email = farmOwner.Email,
                MNo = farmOwner.MobileNumber,
                TelNo = farmOwner.TelephoneNumber
            };

            return owner;
        }

        private HHerdClassification populateHerdClassification(string herdClassDesc)
        {
            var herdClassification = _context.HHerdClassifications.Where(herdClassification => !herdClassification.DeleteFlag &&
                                                                                                herdClassification.HerdClassDesc.Equals(herdClassDesc)).FirstOrDefault();
            if (herdClassification == null)
            {
                return new HHerdClassification()
                {
                    HerdClassCode = string.Empty,
                    HerdClassDesc = string.Empty,
                    Status = new int(),
                    LevelFrom = string.Empty,
                    LevelTo = string.Empty,
                };
            }

            return herdClassification;
        }

        private BuffHerdViewResponseModel populateViewResponseModel(HBuffHerd buffHerd)
        {
            var herdClassification = populateHerdClassification(buffHerd.HerdClassDesc);
            var viewResponseModel = new BuffHerdViewResponseModel()
            {
                id = buffHerd.Id,
                HerdName = buffHerd.HerdName,
                HerdClassDesc = herdClassification.HerdClassDesc,
                HerdClassCode = herdClassification.HerdClassCode,
                HerdSize = buffHerd.HerdSize,
                FarmManager = buffHerd.FarmManager,
                HerdCode = buffHerd.HerdCode,
                FarmAffilCode = buffHerd.FarmAffilCode,
                FarmAddress = buffHerd.FarmAddress,
                Owner = populateOwner(buffHerd.Owner),
                Status = buffHerd.Status,
                OrganizationName = buffHerd.OrganizationName,
                Center = buffHerd.Center,
                Photo = buffHerd.Photo,
                DateCreated = buffHerd.DateCreated,
                CreatedBy = buffHerd.CreatedBy,
                DeleteFlag = buffHerd.DeleteFlag,
                DateUpdated = buffHerd.DateUpdated,
                UpdatedBy = buffHerd.UpdatedBy,
                DateDeleted = buffHerd.DateDeleted,
                DeletedBy = buffHerd.DeletedBy,
                DateRestored = buffHerd.DateRestored,
                RestoredBy = buffHerd.RestoredBy
            };

            var buffaloTypeList = new List<string>();
            var feedingSystemList = new List<string>();
            foreach (HBuffaloType buffaloType in buffHerd.buffaloType)
            {
                buffaloTypeList.Add(buffaloType.BreedTypeCode);
            }

            foreach (HFeedingSystem feedingSystem in buffHerd.feedingSystem)
            {
                feedingSystemList.Add(feedingSystem.FeedingSystemCode);
            }

            viewResponseModel.BreedTypeCodeList.AddRange(buffaloTypeList);
            viewResponseModel.FeedingSystemCodeList.AddRange(feedingSystemList);
            return viewResponseModel;
        }

        private void validateDate(BuffHerdSearchFilterModel searchFilter)
        {

            if (!searchFilter.dateFrom.IsNullOrEmpty())
            {
                if (!DateTime.TryParse(searchFilter.dateFrom, out DateTime dateTimeFrom))
                {
                    throw new System.FormatException("Date From is not a valid Date!");
                }
            }

            if (!searchFilter.dateTo.IsNullOrEmpty())
            {
                if (!DateTime.TryParse(searchFilter.dateTo, out DateTime dateTimeTo))
                {
                    throw new System.FormatException("Date To is not a valid Date!");
                }
            }
        }

    }

}
