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
    public class BuffaloTypesController : ControllerBase
    {
        private readonly PCC_DEVContext _context;

        public BuffaloTypesController(PCC_DEVContext context)
        {
            _context = context;
        }

        // POST: BuffaloTypes/list
        [HttpPost]
        public async Task<ActionResult<IEnumerable<BuffaloTypePagedModel>>> list(CommonSearchFilterModel searchFilter)
        {
            try
            {
                List<HBuffaloType> buffaloTypeList = await buildBuffaloTypeSearchQuery(searchFilter).ToListAsync();
                var result = buildBuffaloTypesPagedModel(searchFilter, buffaloTypeList);
                return Ok(result); ;
            }
            catch (Exception ex)
            {
                return Problem(ex.GetBaseException().ToString());
            }
        }

        private IQueryable<HBuffaloType> buildBuffaloTypeSearchQuery(CommonSearchFilterModel searchFilter)
        {
            IQueryable<HBuffaloType> query = _context.HBuffaloTypes.Where(buffaloType => !buffaloType.DeleteFlag);

            // assuming that you return all records when nothing is specified in the filter

            if (!searchFilter.searchParam.IsNullOrEmpty())
                query = query.Where(buffaloType =>
                                buffaloType.BreedTypeCode.Equals(searchFilter.searchParam) ||
                                buffaloType.BreedTypeDesc.Equals(searchFilter.searchParam));
            return query;
        }

        // GET: BuffaloTypes/search/5
        [HttpGet("{breedTypeCode}")]
        public async Task<ActionResult<IEnumerable<BuffaloTypeResponseModel>>> search(string breedTypeCode)
        {
            var buffaloTypeRecords = await _context.HBuffaloTypes.Where(buffaloType => !buffaloType.DeleteFlag && buffaloType.BreedTypeCode.Equals(breedTypeCode)).ToListAsync();
            if (buffaloTypeRecords.Count == 0)
            {
                return Conflict("No records found!");
            }
            var buffaloTypeResponseModel = convertBuffaloTypeListToResponseModelList(buffaloTypeRecords);
            return buffaloTypeResponseModel;
        }

        private HBuffaloType convertDataRowToBuffaloType(DataRow dataRow)
        {
            return DataRowToObject.ToObject<HBuffaloType>(dataRow);
        }

        // GET: buffaloTypes/view
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BuffaloTypeResponseModel>>> view()
        {
            var buffaloTypeRecords = await _context.HBuffaloTypes.Where(buffaloType => !buffaloType.DeleteFlag).ToListAsync();
            if (buffaloTypeRecords.Count == 0)
            {
                return Conflict("No records found!");
            }

            var buffaloTypeResponseModel = convertBuffaloTypeListToResponseModelList(buffaloTypeRecords);
            return buffaloTypeResponseModel;
        }

        // PUT: BuffaloTypes/update/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> update(int id, BuffaloTypeUpdateModel buffaloTypeUpdateModel)
        {
            var buffaloTypeRecord =  _context.HBuffaloTypes.Where(buffaloType => !buffaloType.DeleteFlag && buffaloType.Id.Equals(id)).FirstOrDefault();
            if (buffaloTypeRecord == null)
            {
                return Conflict("No records matched!");
            }

            var buffaloTypeDuplicateCheck = _context.HBuffaloTypes.Where(buffaloType => !buffaloType.DeleteFlag && 
                                                                                        !buffaloType.Id.Equals(id) && 
                                                                                        buffaloType.BreedTypeCode.Equals(buffaloTypeUpdateModel.BreedTypeCode) &&
                                                                                        buffaloType.BreedTypeDesc.Equals(buffaloTypeUpdateModel.BreedTypeDesc)).FirstOrDefault();
            // check for duplication
            if (buffaloTypeDuplicateCheck != null)
            {
                return Conflict("Entity already exists");
            }

            try
            {
                populateBuffaloType(buffaloTypeRecord, buffaloTypeUpdateModel);
                _context.Entry(buffaloTypeRecord).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                return Ok("Update Successful!");
            }
            catch (Exception ex)
            {
                
                return Problem(ex.GetBaseException().ToString());
            }
        }

        // POST: BuffaloTypes/save
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<HBuffaloType>> save(BuffaloTypeRegistrationModel buffaloTypeRegistrationModel)
        {
            var buffaloTypeDuplicateCheck = _context.HBuffaloTypes.Where(buffaloType => !buffaloType.DeleteFlag &&
                                                                                         buffaloType.BreedTypeCode.Equals(buffaloTypeRegistrationModel.BreedTypeCode) &&
                                                                                         buffaloType.BreedTypeDesc.Equals(buffaloTypeRegistrationModel.BreedTypeDesc)).FirstOrDefault();
            // check for duplication
            if (buffaloTypeDuplicateCheck != null)
            {
                return Conflict("Entity already exists");
            }

            var buffaloTypeModel = buildBuffaloTypeRegistrationModel(buffaloTypeRegistrationModel);

            try
            {

                _context.HBuffaloTypes.Add(buffaloTypeModel);
                await _context.SaveChangesAsync();

                return Ok("Registration Successful!");
            }
            catch (Exception ex)
            {
                
                return Problem(ex.GetBaseException().ToString());
            }
        }

        private HBuffaloType buildBuffaloTypeRegistrationModel(BuffaloTypeRegistrationModel buffaloTypeRegistrationModel)
        {
            var buffaloType = new HBuffaloType()
            {
                BreedTypeCode = buffaloTypeRegistrationModel.BreedTypeCode,
                BreedTypeDesc = buffaloTypeRegistrationModel.BreedTypeDesc,
                Status = 1,
                CreatedBy = buffaloTypeRegistrationModel.CreatedBy,
                DateCreated = DateTime.Now
            };
            return buffaloType;
        }

        // POST: buffaloTypes/delete/5
        [HttpPost]
        public async Task<IActionResult> delete(DeletionModel deletionModel)
        {
            var buffaloTypeModel = _context.HBuffaloTypes.Where(buffaloType => !buffaloType.DeleteFlag && 
                                                                                 buffaloType.Id.Equals(deletionModel.id)).FirstOrDefault();

            if (buffaloTypeModel == null)
            {
                return Conflict("No records matched!");
            }

            try
            {
                buffaloTypeModel.DeleteFlag = true;
                buffaloTypeModel.DateDeleted = DateTime.Now;
                buffaloTypeModel.DeletedBy = deletionModel.deletedBy;
                buffaloTypeModel.DateRestored = null;
                buffaloTypeModel.RestoredBy = "";
                _context.Entry(buffaloTypeModel).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return Ok("Deletion Successful!");
            }
            catch (Exception ex)
            {
                return Problem(ex.GetBaseException().ToString());
            }
        }

        // POST: buffaloTypes/restore/
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<IActionResult> restore(RestorationModel restorationModel)
        {
            var buffaloTypeModel = _context.HBuffaloTypes.Where(buffaloType => buffaloType.DeleteFlag &&
                                                                               buffaloType.Id.Equals(restorationModel.id)).FirstOrDefault();
            if (buffaloTypeModel == null)
            {
                return Conflict("No deleted records matched!");
            }

            try
            {
                buffaloTypeModel.DeleteFlag = !buffaloTypeModel.DeleteFlag;
                buffaloTypeModel.DateDeleted = null;
                buffaloTypeModel.DeletedBy = "";
                buffaloTypeModel.DateRestored = DateTime.Now;
                buffaloTypeModel.RestoredBy = restorationModel.restoredBy;

                _context.Entry(buffaloTypeModel).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return Ok("Restoration Successful!");
            }
            catch (Exception ex)
            {
                
                return Problem(ex.GetBaseException().ToString());
            }
        }

        private List<BuffaloTypePagedModel> buildBuffaloTypesPagedModel(CommonSearchFilterModel searchFilter, List<HBuffaloType> buffaloTypeList)
        {

            int pagesize = searchFilter.pageSize == 0 ? 10 : searchFilter.pageSize;
            int page = searchFilter.page == 0 ? 1 : searchFilter.page;
            var items = (dynamic)null;

            int totalItems = buffaloTypeList.Count;
            int totalPages = (int)Math.Ceiling((double)totalItems / pagesize);
            items = buffaloTypeList.Skip((page - 1) * pagesize).Take(pagesize).ToList();

            List<BuffaloTypeResponseModel> buffaloTypeResponseModels = convertBuffaloTypeListToResponseModelList(buffaloTypeList);

            var result = new List<BuffaloTypePagedModel>();
            var item = new BuffaloTypePagedModel();

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
            item.items = buffaloTypeResponseModels;
            result.Add(item);

            return result;
        }

        private List<HBuffaloType> convertDataRowListToBuffaloTypelist(List<DataRow> dataRowList)
        {
            var buffaloTypeList = new List<HBuffaloType>();

            foreach (DataRow dataRow in dataRowList)
            {
                var buffaloTypeModel = DataRowToObject.ToObject<HBuffaloType>(dataRow);
                buffaloTypeList.Add(buffaloTypeModel);
            }

            return buffaloTypeList;
        }

        private List<BuffaloTypeResponseModel> convertBuffaloTypeListToResponseModelList(List<HBuffaloType> buffaloTypeList)
        {
            var buffaloTypeResponseModels = new List<BuffaloTypeResponseModel>();

            foreach (HBuffaloType buffaloType in buffaloTypeList)
            {
                var buffaloTypeResponseModel = new BuffaloTypeResponseModel()
                {
                    breedTypeCode = buffaloType.BreedTypeCode,
                    breedTypeDesc = buffaloType.BreedTypeDesc
                };
                buffaloTypeResponseModels.Add(buffaloTypeResponseModel);
            }
            return buffaloTypeResponseModels;
        }

        private void populateBuffaloType(HBuffaloType buffaloType, BuffaloTypeUpdateModel buffaloTypeUpdateModel)
        {
            buffaloType.BreedTypeCode = buffaloTypeUpdateModel.BreedTypeCode;
            buffaloType.BreedTypeDesc = buffaloTypeUpdateModel.BreedTypeDesc;
            buffaloType.DateUpdated = DateTime.Now;
            buffaloType.UpdatedBy = buffaloTypeUpdateModel.UpdatedBy;
        }

    }
}
