using API_PCC.ApplicationModels;
using API_PCC.ApplicationModels.Common;
using API_PCC.Data;
using API_PCC.Manager;
using API_PCC.Models;
using API_PCC.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        DbManager db = new DbManager();

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
                DataTable queryResult = db.SelectDb_WithParamAndSorting(QueryBuilder.buildBuffaloTypeSearchQuery(searchFilter), null, populateSqlParameters(searchFilter));
                var result = buildBuffaloTypesPagedModel(searchFilter, queryResult);
                return Ok(result); ;
            }
            catch (Exception ex)
            {
                return Problem(ex.GetBaseException().ToString());
            }
        }

        // GET: BuffaloTypes/search/5
        [HttpGet("{breedTypeCode}")]
        public async Task<ActionResult<IEnumerable<BuffaloTypeResponseModel>>> search(string breedTypeCode)
        {

            DataTable buffaloTypeRecord = db.SelectDb_WithParamAndSorting(QueryBuilder.buildBuffaloTypeSearchQueryByBreedTypeCode(), null, populateSqlParameters(breedTypeCode));

            if (buffaloTypeRecord.Rows.Count == 0)
            {
                return Conflict("No records found!");
            }

            var buffaloTypeModels = convertDataRowListToBuffaloTypelist(buffaloTypeRecord.AsEnumerable().ToList());
            var buffaloTypeResponseModel = convertBuffaloTypeListToResponseModelList(buffaloTypeModels);
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
            DataTable buffaloTypeRecord = db.SelectDb_WithParamAndSorting(QueryBuilder.buildBuffaloTypeSearchQueryAll(), null, new SqlParameter[] { });

            if (buffaloTypeRecord.Rows.Count == 0)
            {
                return Conflict("No records found!");
            }

            var buffaloTypeModels = convertDataRowListToBuffaloTypelist(buffaloTypeRecord.AsEnumerable().ToList());
            var buffaloTypeResponseModel = convertBuffaloTypeListToResponseModelList(buffaloTypeModels);
            return buffaloTypeResponseModel;
        }

        // PUT: BuffaloTypes/update/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> update(int id, BuffaloTypeUpdateModel buffaloTypeUpdateModel)
        {
            DataTable farmerAffiliationRecord = db.SelectDb_WithParamAndSorting(QueryBuilder.buildBuffaloTypeSearchQueryById(), null, populateSqlParameters(id));

            if (farmerAffiliationRecord.Rows.Count == 0)
            {
                return Conflict("No records matched!");
            }

            DataTable buffaloTypeDuplicateCheck = db.SelectDb_WithParamAndSorting(QueryBuilder.buildBuffaloTypeDuplicateCheckUpdateQuery(), null, populateSqlParameters(id, buffaloTypeUpdateModel));

            // check for duplication
            if (buffaloTypeDuplicateCheck.Rows.Count > 0)
            {
                return Conflict("Entity already exists");
            }

            var buffaloTypeModel = convertDataRowToBuffaloType(farmerAffiliationRecord.Rows[0]);

            try
            {
                populateBuffaloType(buffaloTypeModel, buffaloTypeUpdateModel);
                _context.Entry(buffaloTypeModel).State = EntityState.Modified;
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
            DataTable buffaloTypeDuplicateCheck = db.SelectDb_WithParamAndSorting(QueryBuilder.buildBuffaloTypeDuplicateCheckSaveQuery(), null, populateSqlParameters(buffaloTypeRegistrationModel));

            // check for duplication
            if (buffaloTypeDuplicateCheck.Rows.Count > 0)
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
            DataTable buffaloTypeRecord = db.SelectDb_WithParamAndSorting(QueryBuilder.buildBuffaloTypeSearchQueryById(), null, populateSqlParameters(deletionModel.id));

            if (buffaloTypeRecord.Rows.Count == 0)
            {
                return Conflict("No records matched!");
            }

            var buffaloTypeModel = convertDataRowToBuffaloType(buffaloTypeRecord.Rows[0]);

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
            DataTable buffaloTypeRecord = db.SelectDb_WithParamAndSorting(QueryBuilder.buildBuffaloTypeDeletedSearchQueryById(), null, populateSqlParameters(restorationModel.id));

            if (buffaloTypeRecord.Rows.Count == 0)
            {
                return Conflict("No deleted records matched!");
            }

            var buffaloTypeModel = convertDataRowToBuffaloType(buffaloTypeRecord.Rows[0]);

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


        private SqlParameter[] populateSqlParameters(CommonSearchFilterModel searchFilter)
        {

            var sqlParameters = new List<SqlParameter>();

            if (searchFilter.searchParam != null && searchFilter.searchParam != "")
            {
                sqlParameters.Add(new SqlParameter
                {
                    ParameterName = "SearchParam",
                    Value = searchFilter.searchParam ?? Convert.DBNull,
                    SqlDbType = System.Data.SqlDbType.VarChar,
                });
            }

            return sqlParameters.ToArray();
        }

        private SqlParameter[] populateSqlParameters(string breedTypeCode)
        {

            var sqlParameters = new List<SqlParameter>();

            sqlParameters.Add(new SqlParameter
            {
                ParameterName = "BreedTypeCode",
                Value = breedTypeCode ?? Convert.DBNull,
                SqlDbType = System.Data.SqlDbType.VarChar,
            });

            return sqlParameters.ToArray();
        }

        private SqlParameter[] populateSqlParameters(int id)
        {

            var sqlParameters = new List<SqlParameter>();

            sqlParameters.Add(new SqlParameter
            {
                ParameterName = "Id",
                Value = id,
                SqlDbType = System.Data.SqlDbType.Int,
            });

            return sqlParameters.ToArray();
        }

        private SqlParameter[] populateSqlParameters(int id, BuffaloTypeUpdateModel buffaloTypeUpdateModel)
        {

            var sqlParameters = new List<SqlParameter>();

            sqlParameters.Add(new SqlParameter
            {
                ParameterName = "Id",
                Value = id,
                SqlDbType = System.Data.SqlDbType.Int,
            });

            sqlParameters.Add(new SqlParameter
            {
                ParameterName = "BreedTypeCode",
                Value = buffaloTypeUpdateModel.BreedTypeCode,
                SqlDbType = System.Data.SqlDbType.VarChar,
            });

            sqlParameters.Add(new SqlParameter
            {
                ParameterName = "BreedTypeDesc",
                Value = buffaloTypeUpdateModel.BreedTypeDesc,
                SqlDbType = System.Data.SqlDbType.VarChar,
            });

            return sqlParameters.ToArray();
        }

        private SqlParameter[] populateSqlParameters(BuffaloTypeRegistrationModel buffaloTypeRegistrationModel)
        {
            var sqlParameters = new List<SqlParameter>();

            sqlParameters.Add(new SqlParameter
            {
                ParameterName = "BreedTypeCode",
                Value = buffaloTypeRegistrationModel.BreedTypeCode,
                SqlDbType = System.Data.SqlDbType.VarChar,
            });

            sqlParameters.Add(new SqlParameter
            {
                ParameterName = "BreedTypeDesc",
                Value = buffaloTypeRegistrationModel.BreedTypeDesc,
                SqlDbType = System.Data.SqlDbType.VarChar,
            });

            return sqlParameters.ToArray();
        }

        private List<BuffaloTypePagedModel> buildBuffaloTypesPagedModel(CommonSearchFilterModel searchFilter, DataTable dt)
        {

            int pagesize = searchFilter.pageSize == 0 ? 10 : searchFilter.pageSize;
            int page = searchFilter.page == 0 ? 1 : searchFilter.page;
            var items = (dynamic)null;

            int totalItems = dt.Rows.Count;
            int totalPages = (int)Math.Ceiling((double)totalItems / pagesize);
            items = dt.AsEnumerable().Skip((page - 1) * pagesize).Take(pagesize).ToList();

            var buffaloTypeModels = convertDataRowListToBuffaloTypelist(items);
            List<BuffaloTypeResponseModel> buffaloTypeResponseModels = convertBuffaloTypeListToResponseModelList(buffaloTypeModels);

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
