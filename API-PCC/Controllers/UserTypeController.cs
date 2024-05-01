using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using API_PCC.ApplicationModels;
using API_PCC.EntityModels;
using System.Data;
using API_PCC.Utils;
using System.Data.SqlClient;
using API_PCC.Data;
using API_PCC.Manager;
using API_PCC.ApplicationModels.Common;
using API_PCC.Models;
using Microsoft.EntityFrameworkCore;

namespace API_PCC.Controllers
{
    [Authorize("ApiKey")]
    [Route("[controller]/[action]")]
    [ApiController]
    public class UserTypeController : ControllerBase
    {
        private readonly PCC_DEVContext _context;
        DbManager db = new DbManager();

        public UserTypeController(PCC_DEVContext context)
        {
            _context = context;
        }
        [HttpPost]
        public async Task<ActionResult<IEnumerable<UserTypePagedModel>>> list(CommonSearchFilterModel searchFilter)
        {
            searchFilter.searchParam = StringSanitizer.sanitizeString(searchFilter.searchParam);

            try
            {
                DataTable queryResult = db.SelectDb_WithParamAndSorting(QueryBuilder.buildUserTypeSearchQuery(searchFilter), null, populateSearchSqlParameters(searchFilter));
                var result = buildUserTypePagedModel(searchFilter, queryResult);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Problem(ex.GetBaseException().ToString());
            }
        }
        private List<UserTypePagedModel> buildUserTypePagedModel(CommonSearchFilterModel searchFilter, DataTable dt)
        {
            int pagesize = searchFilter.pageSize == 0 ? 10 : searchFilter.pageSize;
            int page = searchFilter.page == 0 ? 1 : searchFilter.page;
            var items = (dynamic)null;
            int totalItems = 0;
            int totalPages = 0;

            totalItems = dt.Rows.Count;
            totalPages = (int)Math.Ceiling((double)totalItems / pagesize);
            items = dt.AsEnumerable().Skip((page - 1) * pagesize).Take(pagesize).ToList();

            var userTypes = convertDataRowListToUserTypeList(items);
            
            var result = new List<UserTypePagedModel>();
            var item = new UserTypePagedModel();

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
            item.items = userTypes;
            result.Add(item);

            return result;
        }

        private SqlParameter[] populateSearchSqlParameters(CommonSearchFilterModel searchFilterModel)
        {

            var sqlParameters = new List<SqlParameter>();

            sqlParameters.Add(new SqlParameter
            {
                ParameterName = "SearchParam",
                Value = searchFilterModel.searchParam ?? Convert.DBNull,
                SqlDbType = System.Data.SqlDbType.VarChar,
            });

            return sqlParameters.ToArray();
        }
        private SqlParameter[] populateSqlParameters(string name)
        {

            var sqlParameters = new List<SqlParameter>();

            sqlParameters.Add(new SqlParameter
            {
                ParameterName = "Name",
                Value = name ?? Convert.DBNull,
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

        private List<TblUserTypeModel> convertDataRowListToUserTypeList(List<DataRow> dataRowList)
        {
            var userTypeList = new List<TblUserTypeModel>();

            foreach (DataRow dataRow in dataRowList)
            {
                var userType = DataRowToObject.ToObject<TblUserTypeModel>(dataRow);
                userTypeList.Add(userType);
            }

            return userTypeList;
        }

        private TblUserTypeModel convertDataRowToUserType(DataRow dataRow)
        {
            return DataRowToObject.ToObject<TblUserTypeModel>(dataRow);
        }

        // GET: userType/search/5
        [HttpGet("{name}")]
        public async Task<ActionResult<TblUserTypeModel>> view(string name)
        {
            DataTable userTypeRecord = db.SelectDb_WithParamAndSorting(QueryBuilder.buildUserTypeQueryByName(), null, populateSqlParameters(name));

            if (userTypeRecord.Rows.Count == 0)
            {
                return Conflict("No records found!");
            }

            var userTypeModel = convertDataRowToUserType(userTypeRecord.Rows[0]);
            var userTypeResponseModel = convertUserTypeToResponseModel(userTypeModel);

            return Ok(userTypeResponseModel);
        }

        private UserTypeResponseModel convertUserTypeToResponseModel(TblUserTypeModel tblUserTypeModel)
        {
            var userTypeResponseModel = new UserTypeResponseModel()
            {
                code = tblUserTypeModel.code,
                name = tblUserTypeModel.name
            };
            return userTypeResponseModel;
        }
        // PUT: userType/update/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> update(int id, UserTypeUpdateModel userTypeUpdateModel)
        {
            DataTable userTypeRecord = db.SelectDb_WithParamAndSorting(QueryBuilder.buildUserTypeQueryById(), null, populateSqlParameters(id));

            if (userTypeRecord.Rows.Count == 0)
            {
                return Conflict("No records matched!");
            }

            DataTable userTypeDuplicateCheck = db.SelectDb_WithParamAndSorting(QueryBuilder.buildUserTypeDuplicateCheckUpdateQuery(), null, populateSqlParameters(id, userTypeUpdateModel));

            // check for duplication
            if (userTypeDuplicateCheck.Rows.Count > 0)
            {
                return Conflict("Entity already exists");
            }

            try
            {
                var userTypeModel = convertDataRowToUserType(userTypeRecord.Rows[0]);
                populateUserType(userTypeModel, userTypeUpdateModel);
                _context.Entry(userTypeModel).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                return Ok("Update Successful!");
            }
            catch (Exception ex)
            {

                return Problem(ex.GetBaseException().ToString());
            }
        }


        // POST: userType/save
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<TblUserTypeModel>> save(UserTypeRegistrationModel userTypeRegistrationModel)
        {
            DataTable userTypeRecord = db.SelectDb_WithParamAndSorting(QueryBuilder.buildUserTypeDuplicateCheckSaveQuery(), null, populateSqlParameters(userTypeRegistrationModel));

            // check for duplication
            if (userTypeRecord.Rows.Count > 0)
            {
                return Conflict("Entity already exists");
            }

            var userType = buildUserTypeRegistrationModel(userTypeRegistrationModel);

            try
            {
                _context.tblUserTypeModels.Add(userType);
                await _context.SaveChangesAsync();

                return Ok("Registration Successful");
            }
            catch (Exception ex)
            {

                return Problem(ex.GetBaseException().ToString());
            }
        }

        // POST: userType/delete/5
        [HttpPost]
        public async Task<IActionResult> delete(DeletionModel deletionModel)
        {
            DataTable userTypeRecord = db.SelectDb_WithParamAndSorting(QueryBuilder.buildUserTypeQueryById(), null, populateSqlParameters(deletionModel.id));

            if (userTypeRecord.Rows.Count == 0)
            {
                return Conflict("No records matched!");
            }

            var userTypeModel = convertDataRowToUserType(userTypeRecord.Rows[0]);

            try
            {
                userTypeModel.DeleteFlag = true;
                userTypeModel.DateDeleted = DateTime.Now;
                userTypeModel.DeletedBy = deletionModel.deletedBy;
                userTypeModel.DateRestored = null;
                userTypeModel.RestoredBy = "";
                _context.Entry(userTypeModel).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                return Ok("Deletion Successful!");
            }
            catch (Exception ex)
            {
                return Problem(ex.GetBaseException().ToString());
            }
        }

        // POST: userType/restore/
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<IActionResult> restore(RestorationModel restorationModel)
        {
            DataTable userTypeRecord = db.SelectDb_WithParamAndSorting(QueryBuilder.buildUserTypeDeletedSearchQueryById(), null, populateSqlParameters(restorationModel.id));

            if (userTypeRecord.Rows.Count == 0)
            {
                return Conflict("No deleted records matched!");
            }

            var userTypeModel = convertDataRowToUserType(userTypeRecord.Rows[0]);

            try
            {
                userTypeModel.DeleteFlag = !userTypeModel.DeleteFlag;
                userTypeModel.DateDeleted = null;
                userTypeModel.DeletedBy = "";
                userTypeModel.DateRestored = DateTime.Now;
                userTypeModel.RestoredBy = restorationModel.restoredBy;

                _context.Entry(userTypeModel).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return Ok("Restoration Successful!");
            }
            catch (Exception ex)
            {

                return Problem(ex.GetBaseException().ToString());
            }
        }

        private SqlParameter[] populateSqlParameters(int id, UserTypeUpdateModel userTypeUpdateModel)
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
                ParameterName = "Code",
                Value = userTypeUpdateModel.Code ?? Convert.DBNull,
                SqlDbType = System.Data.SqlDbType.VarChar,
            });

            sqlParameters.Add(new SqlParameter
            {
                ParameterName = "Name",
                Value = userTypeUpdateModel.Name ?? Convert.DBNull,
                SqlDbType = System.Data.SqlDbType.VarChar,
            });

            return sqlParameters.ToArray();
        }

        private SqlParameter[] populateSqlParameters(UserTypeRegistrationModel userTypeRegistrationModel)
        {
            var sqlParameters = new List<SqlParameter>();

            sqlParameters.Add(new SqlParameter
            {
                ParameterName = "Code",
                Value = userTypeRegistrationModel.Code ?? Convert.DBNull,
                SqlDbType = System.Data.SqlDbType.VarChar,
            });

            sqlParameters.Add(new SqlParameter
            {
                ParameterName = "Name",
                Value = userTypeRegistrationModel.Name ?? Convert.DBNull,
                SqlDbType = System.Data.SqlDbType.VarChar,
            });

            return sqlParameters.ToArray();
        }
        private void populateUserType(TblUserTypeModel userType, UserTypeUpdateModel userTypeUpdateModel)
        {
            userType.code = userTypeUpdateModel.Code;
            userType.name = userTypeUpdateModel.Name;
            userType.DateUpdated = DateTime.Now;
            userType.UpdatedBy = userTypeUpdateModel.UpdatedBy;
        }

        private TblUserTypeModel buildUserTypeRegistrationModel(UserTypeRegistrationModel userTypeRegistrationModel)
        {
            var userType = new TblUserTypeModel()
            {
                code = userTypeRegistrationModel.Code,
                name = userTypeRegistrationModel.Name,
                DeleteFlag = false,
                CreatedBy = userTypeRegistrationModel.CreatedBy,
                DateCreated = DateTime.Now
            };
            return userType;
        }
    }
}
