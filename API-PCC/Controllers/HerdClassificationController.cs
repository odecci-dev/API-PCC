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
    public class HerdClassificationController : ControllerBase
    {

        private readonly PCC_DEVContext _context;
        DbManager db = new DbManager();

        public HerdClassificationController(PCC_DEVContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<ActionResult<IEnumerable<HerdClassificationPagedModel>>> List(CommonSearchFilterModel searchFilter)
        {

            try
            {
                DataTable queryResult = db.SelectDb_WithParamAndSorting(QueryBuilder.buildHerdClassificationSearchQuery(searchFilter), null, populateSqlParameters(searchFilter));
                var result = buildHerdClassificationPagedModel(searchFilter, queryResult);
                return Ok(result);
            }

            catch (Exception ex)
            {
                
                return Problem(ex.GetBaseException().ToString());
            }
        }

        // GET: HerdClassification/search/5
        [HttpGet("{herdClassCode}")]
        public async Task<ActionResult<IEnumerable<HerdClassificationResponseModel>>> search(string herdClassCode)
        {
            try { 
                DataTable queryResult = db.SelectDb_WithParamAndSorting(QueryBuilder.buildHerdClassificationSearchQueryByHerdClassCode(), null, populateSqlParameters(herdClassCode));
                if (queryResult.Rows.Count == 0)
                {
                    return Conflict("No records found!");
                }
                var herdClassificationModels = convertDataRowToHerdClassificationList(queryResult.AsEnumerable().ToList());
                List<HerdClassificationResponseModel> herdClassificationResponseModels = convertHerdClassificationToResponseModelList(herdClassificationModels);

                return Ok(herdClassificationResponseModels);
            }
            catch (Exception ex)
            {    
                return Problem(ex.GetBaseException().ToString());
            }
        }

        // GET: HerdClassification/view
        [HttpGet]
        public async Task<ActionResult<IEnumerable<HHerdClassification>>> view()
        {
            try
            {
                DataTable queryResult = db.SelectDb_WithParamAndSorting(QueryBuilder.buildHerdClassificationSearchQueryAll(), null, new SqlParameter[] { });
                if (queryResult.Rows.Count == 0)
                {
                    return Conflict("No records found!");
                }
                var herdClassificationModels = convertDataRowToHerdClassificationList(queryResult.AsEnumerable().ToList());
                List<HerdClassificationResponseModel> herdClassificationResponseModels = convertHerdClassificationToResponseModelList(herdClassificationModels);

                return Ok(herdClassificationResponseModels);
            }
            catch (Exception ex)
            {
                return Problem(ex.GetBaseException().ToString());
            }
        }

        // PUT: HerdClassification/update/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> update(int id, HerdClassificationUpdateModel herdClassificationUpdateModel)
        {
            DataTable herdClassificationRecord = db.SelectDb_WithParamAndSorting(QueryBuilder.buildHerdClassificationSearchQueryById(), null, populateSqlParameters(id));

            if (herdClassificationRecord.Rows.Count == 0)
            {
                return Conflict("No records matched!");
            }

            DataTable herdClassificationDuplicateCheck = db.SelectDb_WithParamAndSorting(QueryBuilder.buildHerdClassificationDuplicateCheckUpdateQuery(), null, populateSqlParameters(id, herdClassificationUpdateModel));

            // check for duplication
            if (herdClassificationDuplicateCheck.Rows.Count > 0)
            {
                return Conflict("Entity already exists");
            }

            var herdClassificationModel = convertDataRowToHerdClassification(herdClassificationRecord.Rows[0]);

            try
            {
                populateHerdClassification(herdClassificationModel, herdClassificationUpdateModel);
                _context.Entry(herdClassificationModel).State = EntityState.Modified;
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
        public async Task<ActionResult<HHerdClassification>> save(HerdClassificationRegistrationModel herdClassificationRegistrationModel)
        {

          DataTable hasDuplicateOnSave = db.SelectDb_WithParamAndSorting(QueryBuilder.buildHerdClassificationDuplicateCheckSaveQuery(), null, populateSqlParameters(herdClassificationRegistrationModel));

            // check for duplication
          if (hasDuplicateOnSave.Rows.Count > 0)
          {
              return Conflict("Entity already exists");
          }
          try
          {
                var herdClassification = buildHerdClassificationRegistrationModel(herdClassificationRegistrationModel);
                _context.HHerdClassifications.Add(herdClassification);
                await _context.SaveChangesAsync();
                return Ok("Herd successfully registered!");
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
            DataTable herdClassificationRecord = db.SelectDb_WithParamAndSorting(QueryBuilder.buildHerdClassificationSearchQueryById(), null, populateSqlParameters(deletionModel.id));

            if (herdClassificationRecord.Rows.Count == 0)
            {
                return Conflict("No records found!");
            }

            var herdClassificationModel = convertDataRowToHerdClassification(herdClassificationRecord.Rows[0]);
            DataTable herdRecord = db.SelectDb_WithParamAndSorting(QueryBuilder.buildHerdSelectQueryByHerdClassDesc(), null, populateSqlParameters(herdClassificationModel.HerdClassCode));

            if (herdRecord.Rows.Count > 0)
            {
                return Conflict("Used by other table!");
            }

            try
            {
                herdClassificationModel.DeleteFlag = true;
                herdClassificationModel.DateDeleted = DateTime.Now;
                herdClassificationModel.DeletedBy = deletionModel.deletedBy;
                herdClassificationModel.DateRestored = null;
                herdClassificationModel.RestoredBy = "";
                _context.Entry(herdClassificationModel).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return Ok("Deletion Successful!");
            }
            catch(Exception ex)
            {
                
                return Problem(ex.GetBaseException().ToString());
            }
        }

        // POST: HerdClassification/restore/
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<IActionResult> restore(RestorationModel restorationModel)
        {


            DataTable herdClassificationRecord = db.SelectDb_WithParamAndSorting(QueryBuilder.buildHerdClassificationDeletedSearchQueryById(), null, populateSqlParameters(restorationModel.id));

            if (herdClassificationRecord.Rows.Count == 0)
            {
                return Conflict("No deleted records found!");
            }

            var herdClassificationModel = convertDataRowToHerdClassification(herdClassificationRecord.Rows[0]);

            try
            {
                herdClassificationModel.DeleteFlag = !herdClassificationModel.DeleteFlag;
                herdClassificationModel.DateDeleted = null;
                herdClassificationModel.DeletedBy = "";
                herdClassificationModel.DateRestored = DateTime.Now;
                herdClassificationModel.RestoredBy = restorationModel.restoredBy;

                _context.Entry(herdClassificationModel).State = EntityState.Modified;
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

        private SqlParameter[] populateSqlParameters(string herdClassDesc)
        {

            var sqlParameters = new List<SqlParameter>();

            sqlParameters.Add(new SqlParameter
            {
                ParameterName = "HerdClassDesc",
                Value = herdClassDesc ?? Convert.DBNull,
                SqlDbType = System.Data.SqlDbType.VarChar,
            });

            return sqlParameters.ToArray();
        }

        private SqlParameter[] populateSqlParameters(int id, HerdClassificationUpdateModel herdClassificationUpdateModel)
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
                ParameterName = "HerdClassCode",
                Value = herdClassificationUpdateModel.HerdClassCode,
                SqlDbType = System.Data.SqlDbType.VarChar,
            });

            sqlParameters.Add(new SqlParameter
            {
                ParameterName = "HerdClassDesc",
                Value = herdClassificationUpdateModel.HerdClassDesc ?? Convert.DBNull,
                SqlDbType = System.Data.SqlDbType.VarChar,
            });


            return sqlParameters.ToArray();
        }

        private SqlParameter[] populateSqlParameters(HerdClassificationRegistrationModel herdClassificationRegistrationModel)
        {
            var sqlParameters = new List<SqlParameter>();

            sqlParameters.Add(new SqlParameter
            {
                ParameterName = "HerdClassCode",
                Value = herdClassificationRegistrationModel.HerdClassCode,
                SqlDbType = System.Data.SqlDbType.VarChar,
            });

            sqlParameters.Add(new SqlParameter
            {
                ParameterName = "HerdClassDesc",
                Value = herdClassificationRegistrationModel.HerdClassDesc ?? Convert.DBNull,
                SqlDbType = System.Data.SqlDbType.VarChar,
            });


            return sqlParameters.ToArray();
        }

        private List<HerdClassificationPagedModel> buildHerdClassificationPagedModel(CommonSearchFilterModel searchFilter, DataTable dt)
        {
            int pagesize = searchFilter.pageSize == 0 ? 10 : searchFilter.pageSize;
            int page = searchFilter.page == 0 ? 1 : searchFilter.page;
            var items = (dynamic)null;

            int totalItems = dt.Rows.Count;
            int totalPages = (int)Math.Ceiling((double)totalItems / pagesize);
            items = dt.AsEnumerable().Skip((page - 1) * pagesize).Take(pagesize).ToList();


            var herdClassificationModels = convertDataRowToHerdClassificationList(items);
            List<HerdClassificationResponseModel> herdClassificationResponseModels = convertHerdClassificationToResponseModelList(herdClassificationModels);

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
            item.items = herdClassificationResponseModels;
            result.Add(item);

            return result;
        }

        private List<HHerdClassification> convertDataRowToHerdClassificationList(List<DataRow> dataRowList)
        {
            var herdClassificationList = new List<HHerdClassification>();

            foreach (DataRow dataRow in dataRowList)
            {
                var herdClassificationModel = DataRowToObject.ToObject<HHerdClassification>(dataRow);
                herdClassificationList.Add(herdClassificationModel);
            }

            return herdClassificationList;
        }

        private List<HerdClassificationResponseModel> convertHerdClassificationToResponseModelList(List<HHerdClassification> hHerdClassificationList)
        {
            var herdClassificationResponseModels = new List<HerdClassificationResponseModel>();

            foreach (HHerdClassification herdClassification in hHerdClassificationList)
            {
                var herdClassificationResponseModel = new HerdClassificationResponseModel()
                {
                    herdClassCode = herdClassification.HerdClassCode,
                    herdClassDesc = herdClassification.HerdClassDesc
                };
                herdClassificationResponseModels.Add(herdClassificationResponseModel);
            }

            return herdClassificationResponseModels;
        }

        private HHerdClassification convertDataRowToHerdClassification(DataRow dataRow)
        {
            return DataRowToObject.ToObject<HHerdClassification>(dataRow);
        }
        private HHerdClassification buildHerdClassificationRegistrationModel(HerdClassificationRegistrationModel herdClassificationRegistrationModel)
        {
            var herdClassification = new HHerdClassification()
            {
                HerdClassCode = herdClassificationRegistrationModel.HerdClassCode,
                HerdClassDesc = herdClassificationRegistrationModel.HerdClassDesc,
                LevelFrom = herdClassificationRegistrationModel.LevelFrom,
                LevelTo = herdClassificationRegistrationModel.LevelTo,
                CreatedBy = herdClassificationRegistrationModel.CreatedBy,
                DateCreated = DateTime.Now
            };
            return herdClassification;
        }

        private void populateHerdClassification(HHerdClassification herdClassification, HerdClassificationUpdateModel herdClassificationUpdateModel)
        {
            herdClassification.HerdClassCode = herdClassificationUpdateModel.HerdClassCode;
            herdClassification.HerdClassDesc = herdClassificationUpdateModel.HerdClassDesc;
            herdClassification.LevelFrom = herdClassificationUpdateModel.LevelFrom;
            herdClassification.LevelTo = herdClassificationUpdateModel.LevelTo;
            herdClassification.DateUpdated = DateTime.Now;
            herdClassification.UpdatedBy = herdClassificationUpdateModel.UpdatedBy;
        }

    }
}
