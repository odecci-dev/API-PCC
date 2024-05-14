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
    public class FarmerAffiliationsController : ControllerBase
    {
        private readonly PCC_DEVContext _context;
        DbManager db = new DbManager();

        public FarmerAffiliationsController(PCC_DEVContext context)
        {
            _context = context;
        }

        // POST: FarmerAffiliations/list
        [HttpPost]
        public async Task<ActionResult<IEnumerable<FarmerAffiliationPagedModel>>> list(CommonSearchFilterModel searchFilter)
        {
            try
            {
                DataTable queryResult = db.SelectDb_WithParamAndSorting(QueryBuilder.buildFarmerAffiliationSearchQuery(searchFilter), null, populateSqlParameters(searchFilter));
                var result = buildFarmerAffiliationPagedModel(searchFilter, queryResult);
                return Ok(result);
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

        private SqlParameter[] populateSqlParameters(string fcode)
        {

            var sqlParameters = new List<SqlParameter>();

            if (fcode != null && fcode != "")
            {
                sqlParameters.Add(new SqlParameter
                {
                    ParameterName = "FCode",
                    Value = fcode ?? Convert.DBNull,
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

        private SqlParameter[] populateSqlParameters(int id, FarmerAffiliationUpdateModel farmerAffiliationUpdateModel)
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
                ParameterName = "FCode",
                Value = farmerAffiliationUpdateModel.FCode,
                SqlDbType = System.Data.SqlDbType.VarChar,
            });

            sqlParameters.Add(new SqlParameter
            {
                ParameterName = "FDesc",
                Value = farmerAffiliationUpdateModel.FDesc,
                SqlDbType = System.Data.SqlDbType.VarChar,
            });

            return sqlParameters.ToArray();
        }

        private SqlParameter[] populateSqlParameters(FarmerAffiliationRegistrationModel farmerAffiliationRegistrationModel)
        {
            var sqlParameters = new List<SqlParameter>();

            sqlParameters.Add(new SqlParameter
            {
                ParameterName = "FCode",
                Value = farmerAffiliationRegistrationModel.FCode,
                SqlDbType = System.Data.SqlDbType.VarChar,
            });

            sqlParameters.Add(new SqlParameter
            {
                ParameterName = "FDesc",
                Value = farmerAffiliationRegistrationModel.FDesc,
                SqlDbType = System.Data.SqlDbType.VarChar,
            });

            return sqlParameters.ToArray();
        }

        private List<FarmerAffiliationPagedModel> buildFarmerAffiliationPagedModel(CommonSearchFilterModel searchFilter, DataTable dt)
        {

            int pagesize = searchFilter.pageSize == 0 ? 10 : searchFilter.pageSize;
            int page = searchFilter.page == 0 ? 1 : searchFilter.page;
            var items = (dynamic)null;

            int totalItems = dt.Rows.Count;
            int totalPages = (int)Math.Ceiling((double)totalItems / pagesize);
            items = dt.AsEnumerable().Skip((page - 1) * pagesize).Take(pagesize).ToList();

            var farmerAffiliationModels = convertDataRowListToFarmerAffiliationlist(items);
            List<FarmerAffiliationResponseModel> famerAffiliationResponseModels = convertFarmerAffiliationToResponseModelList(farmerAffiliationModels);

            var result = new List<FarmerAffiliationPagedModel>();
            var item = new FarmerAffiliationPagedModel();

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
            item.items = famerAffiliationResponseModels;
            result.Add(item);

            return result;
        }

        private List<HFarmerAffiliation> convertDataRowListToFarmerAffiliationlist(List<DataRow> dataRowList)
        {
            var farmerAffiliationList = new List<HFarmerAffiliation>();

            foreach (DataRow dataRow in dataRowList)
            {
                var farmerAffiliationModel = DataRowToObject.ToObject<HFarmerAffiliation>(dataRow);
                farmerAffiliationList.Add(farmerAffiliationModel);
            }

            return farmerAffiliationList;
        }

        private HFarmerAffiliation convertDataRowToFarmerAffiliation(DataRow dataRow)
        {
            return DataRowToObject.ToObject<HFarmerAffiliation>(dataRow);        
        }

        private List<FarmerAffiliationResponseModel> convertFarmerAffiliationToResponseModelList(List<HFarmerAffiliation> farmerAffiliationList)
        {
            var farmerAffiliationResponseModels = new List<FarmerAffiliationResponseModel>(); 

            foreach (HFarmerAffiliation farmerAffiliation in farmerAffiliationList)
            {
                var farmerAffiliationResponseModel = new FarmerAffiliationResponseModel()
                {
                    farmerAffiliationCode = farmerAffiliation.FCode,
                    farmerAffiliationName = farmerAffiliation.FDesc
                };
                farmerAffiliationResponseModels.Add(farmerAffiliationResponseModel);
            }

            return farmerAffiliationResponseModels;
        }

        // GET: FarmerAffiliations/search/5
        [HttpGet("{fcode}")]
        public async Task<ActionResult<IEnumerable<FarmerAffiliationResponseModel>>> search(string fcode)
        {
            try
            {
                DataTable farmerAffiliationRecord = db.SelectDb_WithParamAndSorting(QueryBuilder.buildFarmerAffiliationSearchQueryByFCode(), null, populateSqlParameters(fcode));

                if (farmerAffiliationRecord.Rows.Count == 0)
                {
                    return Conflict("No records found!");
                }

                var herdClassificationModels = convertDataRowListToFarmerAffiliationlist(farmerAffiliationRecord.AsEnumerable().ToList());
                List<FarmerAffiliationResponseModel> herdClassificationResponseModels = convertFarmerAffiliationToResponseModelList(herdClassificationModels);

                return Ok(herdClassificationResponseModels);
            } catch (Exception ex)
            {
                return Problem(ex.GetBaseException().ToString());
            }
        }

        // GET: farmerAffiliations/view
        [HttpGet]
        public async Task<ActionResult<IEnumerable<FarmerAffiliationResponseModel>>> view()
        {
            try
            {
                DataTable farmerAffiliationRecord = db.SelectDb_WithParamAndSorting(QueryBuilder.buildFarmerAffiliationSearchQueryAll(), null, new SqlParameter[] { });

                if (farmerAffiliationRecord.Rows.Count == 0)
                {
                    return Conflict("No records found!");
                }

                var herdClassificationModels = convertDataRowListToFarmerAffiliationlist(farmerAffiliationRecord.AsEnumerable().ToList());
                List<FarmerAffiliationResponseModel> herdClassificationResponseModels = convertFarmerAffiliationToResponseModelList(herdClassificationModels);

                return Ok(herdClassificationResponseModels);
            }
            catch (Exception ex)
            {
                return Problem(ex.GetBaseException().ToString());
            }
        }

        private void populateFarmerAffiliation(HFarmerAffiliation farmerAffiliation, FarmerAffiliationUpdateModel farmerAffiliationUpdateModel)
        {
            farmerAffiliation.FCode = farmerAffiliationUpdateModel.FCode;
            farmerAffiliation.FDesc = farmerAffiliationUpdateModel.FDesc;
            farmerAffiliation.DateUpdated = DateTime.Now;
            farmerAffiliation.UpdatedBy = farmerAffiliationUpdateModel.UpdatedBy;
        }

        // PUT: FarmerAffiliations/update/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> update(int id, FarmerAffiliationUpdateModel farmerAffiliationUpdateModel)
        {
            DataTable farmerAffiliationRecord = db.SelectDb_WithParamAndSorting(QueryBuilder.buildFarmerAffiliationSearchQueryById(), null, populateSqlParameters(id));

            if (farmerAffiliationRecord.Rows.Count == 0)
            {
                return Conflict("No records matched!");
            }

            DataTable farmerAffiliationDuplicateCheck = db.SelectDb_WithParamAndSorting(QueryBuilder.buildFarmerAffiliationDuplicateCheckUpdateQuery(), null, populateSqlParameters(id, farmerAffiliationUpdateModel));

            // check for duplication
            if (farmerAffiliationDuplicateCheck.Rows.Count > 0)
            {
                return Conflict("Entity already exists");
            }

            var farmerAffiliationModel = convertDataRowToFarmerAffiliation(farmerAffiliationRecord.Rows[0]);

            try
            {
                populateFarmerAffiliation(farmerAffiliationModel, farmerAffiliationUpdateModel);
                _context.Entry(farmerAffiliationModel).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                return Ok("Update Successful!");
            }
            catch (Exception ex)
            {

                return Problem(ex.GetBaseException().ToString());
            }
        }

        // POST: FarmerAffiliations/save
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult> save(FarmerAffiliationRegistrationModel farmerAffiliationRegistrationModel)
        {

            DataTable farmerAffiliationDuplicateCheck = db.SelectDb_WithParamAndSorting(QueryBuilder.buildFarmerAffiliationDuplicateCheckSaveQuery(), null, populateSqlParameters(farmerAffiliationRegistrationModel));

            // check for duplication
            if (farmerAffiliationDuplicateCheck.Rows.Count > 0)
            {
                return Conflict("Entity already exists");
            }

            var farmerAffiliationModel = buildFarmerAffiliationRegistrationModel(farmerAffiliationRegistrationModel);

            try
            {
                _context.HFarmerAffiliations.Add(farmerAffiliationModel);
                await _context.SaveChangesAsync();

                return Ok("Registration Successful!");
            }
            catch (Exception ex)
            {

                return Problem(ex.GetBaseException().ToString());
            }
        }
        private HFarmerAffiliation buildFarmerAffiliationRegistrationModel(FarmerAffiliationRegistrationModel farmerAffiliationRegistrationModel)
        {
            var farmerAffiliation = new HFarmerAffiliation()
            {
                FCode = farmerAffiliationRegistrationModel.FCode,
                FDesc = farmerAffiliationRegistrationModel.FDesc,
                Status = 1,
                CreatedBy = farmerAffiliationRegistrationModel.CreatedBy,
                DateCreated = DateTime.Now
            };
            return farmerAffiliation;
        }

        // POST: FarmerAffiliations/delete/5
        [HttpPost]
        public async Task<IActionResult> delete(DeletionModel deletionModel)
        {
            DataTable farmerAffiliationRecord = db.SelectDb_WithParamAndSorting(QueryBuilder.buildFarmerAffiliationSearchQueryById(), null, populateSqlParameters(deletionModel.id));

            if (farmerAffiliationRecord.Rows.Count == 0)
            {
                return Conflict("No records matched!");
            }

            var farmerAffiliationModel = convertDataRowToFarmerAffiliation(farmerAffiliationRecord.Rows[0]);

            try
            {
                farmerAffiliationModel.DeleteFlag = true;
                farmerAffiliationModel.DateDeleted = DateTime.Now;
                farmerAffiliationModel.DeletedBy = deletionModel.deletedBy;
                farmerAffiliationModel.DateRestored = null;
                farmerAffiliationModel.RestoredBy = "";
                _context.Entry(farmerAffiliationModel).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return Ok("Deletion Successful!");
            }
            catch (Exception ex)
            {
                
                return Problem(ex.GetBaseException().ToString());
            }
        }

        // POST: farmerAffiliations/restore/
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<IActionResult> restore(RestorationModel restorationModel)
        {

            DataTable farmerAffiliationRecord = db.SelectDb_WithParamAndSorting(QueryBuilder.buildFarmerAffiliationDeletedSearchQueryById(), null, populateSqlParameters(restorationModel.id));

            if (farmerAffiliationRecord.Rows.Count == 0)
            {
                return Conflict("No deleted records matched!");
            }

            var farmerAffiliationModel = convertDataRowToFarmerAffiliation(farmerAffiliationRecord.Rows[0]);

            try
            {
                farmerAffiliationModel.DeleteFlag = !farmerAffiliationModel.DeleteFlag;
                farmerAffiliationModel.DateDeleted = null;
                farmerAffiliationModel.DeletedBy = "";
                farmerAffiliationModel.DateRestored = DateTime.Now;
                farmerAffiliationModel.RestoredBy = restorationModel.restoredBy;

                _context.Entry(farmerAffiliationModel).State = EntityState.Modified;
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
