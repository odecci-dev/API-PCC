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
using NuGet.Packaging;
using NuGet.Protocol.Core.Types;
using System.Data;
using System.Data.SqlClient;
using static API_PCC.Controllers.UserController;

namespace API_PCC.Controllers
{
    [Authorize("ApiKey")]
    [Route("[controller]/[action]")]
    [ApiController]
    public class UserManagementController : ControllerBase
    {
        private readonly PCC_DEVContext _context;
        DbManager db = new DbManager();

        public UserManagementController(PCC_DEVContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<ActionResult<IEnumerable<UserPagedModel>>> List(CommonSearchFilterModel searchFilter)
        {
            try
            {
                var filter = new Dictionary<string, object>();
                filter.Add("searchParam", searchFilter.searchParam);
                List<TblUsersModel> userList = await buildUserManagementSearchQuery(filter).ToListAsync();
                var result = buildUserPagedModel(searchFilter, userList);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Problem(ex.GetBaseException().ToString());
            }
        }

        [HttpPost]
        public async Task<ActionResult<IEnumerable<UserPagedModel>>> UserForApprovalList(CommonSearchFilterModel searchFilter)
        {
            try
            {
                var filter = new Dictionary<string, object>();
                filter.Add("forApproval", true);
                List<TblUsersModel> userList = await buildUserManagementSearchQuery(filter).ToListAsync();
                var result = buildUserPagedModel(searchFilter, userList);
                return Ok(result);
            }

            catch (Exception ex)
            {

                return Problem(ex.GetBaseException().ToString());
            }
        }

        // GET: UserManagemetn/search/5
        [HttpGet("{username}")]
        public async Task<ActionResult<IEnumerable<UserResponseModel>>> search(string username)
        {
            try
            {
                var filter = new Dictionary<string, object>();
                filter.Add("username", username);
                List<TblUsersModel> userList = await buildUserManagementSearchQuery(filter).ToListAsync();

                if (userList.Count == 0)
                {
                    return Conflict("No records found!");
                }

                List<UserResponseModel> userResponseModels = convertUserListToResponseModelList(userList);

                return Ok(userResponseModels);
            }
            catch (Exception ex)
            {
                return Problem(ex.GetBaseException().ToString());
            }
        }

        // PUT: UserManagement/update/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> update(int id, UserUpdateModel userUpdateModel)
        {
            //DataTable userRecord = db.SelectDb_WithParamAndSorting(QueryBuilder.buildUserSearchQueryById(), null, populateSqlParameters(id));
            var filter = new Dictionary<string, object>();
            filter.Add("Id", id);
            var userModel = await buildUserManagementSearchQuery(filter).FirstOrDefaultAsync();

            if (userModel == null)
            {
                return Conflict("No records matched!");
            }

            DataTable userDuplicateCheck = db.SelectDb_WithParamAndSorting(QueryBuilder.buildUserDuplicateCheckUpdateQuery(), null, populateSqlParameters(id, userUpdateModel));

            // check for duplication
            if (userDuplicateCheck.Rows.Count > 0)
            {
                return Conflict("Entity already exists");
            }

            try
            {
                userModel.userAccessModels.Clear();
                populateUser(userModel, userUpdateModel);
                populateUserAccess(userModel, userUpdateModel);
                _context.Entry(userModel).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                return Ok("Update Successful!");
            }
            catch (Exception ex)
            {

                return Problem(ex.GetBaseException().ToString());
            }
        }

        // GET: usermanagement/useraccess/list/{username}
        [HttpGet]
        [Route("/UserManagement/useraccess/list/{username}")]
        public async Task<IActionResult> list(string username)
        {
            var userModel = await _context.TblUsersModels
                .Include(user => user.userAccessModels)
                .ThenInclude(userAccessModel => userAccessModel.userAccess)
                .Where(user => user.Username.Equals(username))
                .FirstOrDefaultAsync();

            if (userModel == null)
            {
                return Problem("Username does not exists!");
            }

            var userAccessListModel = populateUserAccessListModel(userModel);
            return Ok(userAccessListModel);
        }

        // GET: usermanagement/useraccess/update/{username}
        [HttpPut]
        [Route("/UserManagement/useraccess/update/{username}")]
        public async Task<IActionResult> update(string username, UserAccessListModel userAccessListModel)
        {
            var userModel = await _context.TblUsersModels
                .Include(user => user.userAccessModels)
                .ThenInclude(userAccessModel => userAccessModel.userAccess)
                .Where(user => user.Username.Equals(username))
                .FirstOrDefaultAsync();

            if (userModel == null)
            {
                return Problem("Username does not exists!");
            }

            userModel.userAccessModels.Clear();
            userModel.userAccessModels.AddRange(populateUserAccessList(userAccessListModel.userAccessList));
            _context.Entry(userModel).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return Ok("Update Successful!");
        }


        private void populateUser(TblUsersModel userModel, UserUpdateModel userUpdateModel)
        {
            userModel.Username = userUpdateModel.Username;
            userModel.Password = Cryptography.Encrypt(userUpdateModel.Password);
            userModel.Fullname = userUpdateModel.Fullname;
            userModel.Fname = userUpdateModel.Fname;
            userModel.Lname = userUpdateModel.Lname;
            userModel.Mname = userUpdateModel.Mname;
            userModel.Email = userUpdateModel.Email;
            userModel.Gender = userUpdateModel.Gender;
            userModel.EmployeeId = userUpdateModel.EmployeeId;
            userModel.Active = userUpdateModel.Active;
            userModel.Cno = userUpdateModel.Cno;
            userModel.Address = userUpdateModel.Address;
            userModel.CenterId = userUpdateModel.CenterId;
            userModel.AgreementStatus = userUpdateModel.AgreementStatus;
        }

        // POST: UserManagement/delete/5
        [HttpPost]
        public async Task<IActionResult> delete(DeletionModel deletionModel)
        {
            DataTable userRecord = db.SelectDb_WithParamAndSorting(QueryBuilder.buildUserSearchQueryById(), null, populateSqlParameters(deletionModel.id));

            if (userRecord.Rows.Count == 0)
            {
                return Conflict("No records found!");
            }

            var userModel = convertDataRowToUser(userRecord.Rows[0]);

            try
            {
                userModel.DeleteFlag = true;
                userModel.DateDeleted = DateTime.Now;
                userModel.DeletedBy = deletionModel.deletedBy;
                userModel.DateRestored = null;
                userModel.RestoredBy = "";
                _context.Entry(userModel).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return Ok("Deletion Successful!");
            }
            catch (Exception ex)
            {

                return Problem(ex.GetBaseException().ToString());
            }
        }

        // POST: UserManagement/restore/
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<IActionResult> restore(RestorationModel restorationModel)
        {


            DataTable userRecord = db.SelectDb_WithParamAndSorting(QueryBuilder.buildUserDeletedSearchQueryById(), null, populateSqlParameters(restorationModel.id));

            if (userRecord.Rows.Count == 0)
            {
                return Conflict("No deleted records found!");
            }

            var userModel = convertDataRowToUser(userRecord.Rows[0]);

            try
            {
                userModel.DeleteFlag = !userModel.DeleteFlag;
                userModel.DateDeleted = null;
                userModel.DeletedBy = "";
                userModel.DateRestored = DateTime.Now;
                userModel.RestoredBy = restorationModel.restoredBy;

                _context.Entry(userModel).State = EntityState.Modified;
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
                SqlDbType = System.Data.SqlDbType.VarChar,
            });
            return sqlParameters.ToArray();
        }

        private SqlParameter[] populateSqlParameters(string username)
        {
            var sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(new SqlParameter
            {
                ParameterName = "Username",
                Value = username ?? Convert.DBNull,
                SqlDbType = System.Data.SqlDbType.VarChar,
            });
            return sqlParameters.ToArray();
        }
        private SqlParameter[] populateSqlParameters(int id, UserUpdateModel userUpdateModel)
        {
            var sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(new SqlParameter
            {
                ParameterName = "Id",
                Value = id,
                SqlDbType = System.Data.SqlDbType.VarChar,
            });

            sqlParameters.Add(new SqlParameter
            {
                ParameterName = "Username",
                Value = userUpdateModel.Username,
                SqlDbType = System.Data.SqlDbType.VarChar,
            });

            sqlParameters.Add(new SqlParameter
            {
                ParameterName = "Fullname",
                Value = userUpdateModel.Fullname,
                SqlDbType = System.Data.SqlDbType.VarChar,
            });

            sqlParameters.Add(new SqlParameter
            {
                ParameterName = "Fname",
                Value = userUpdateModel.Fname,
                SqlDbType = System.Data.SqlDbType.VarChar,
            });

            sqlParameters.Add(new SqlParameter
            {
                ParameterName = "Lname",
                Value = userUpdateModel.Lname,
                SqlDbType = System.Data.SqlDbType.VarChar,
            });

            sqlParameters.Add(new SqlParameter
            {
                ParameterName = "Mname",
                Value = userUpdateModel.Mname,
                SqlDbType = System.Data.SqlDbType.VarChar,
            });

            sqlParameters.Add(new SqlParameter
            {
                ParameterName = "Email",
                Value = userUpdateModel.Email,
                SqlDbType = System.Data.SqlDbType.VarChar,
            });

            return sqlParameters.ToArray();
        }

        private List<UserPagedModel> buildUserPagedModel(CommonSearchFilterModel searchFilter, List<TblUsersModel> userList)
        {
            int pagesize = searchFilter.pageSize == 0 ? 10 : searchFilter.pageSize;
            int page = searchFilter.page == 0 ? 1 : searchFilter.page;
            var items = (dynamic)null;

            int totalItems = userList.Count;
            int totalPages = (int)Math.Ceiling((double)totalItems / pagesize);
            items = userList.AsEnumerable().Skip((page - 1) * pagesize).Take(pagesize).ToList();

            List<UserResponseModel> userResponseModels = convertUserListToResponseModelList(userList);

            var result = new List<UserPagedModel>();
            var item = new UserPagedModel();

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
            item.items = userResponseModels;
            result.Add(item);

            return result;
        }

        private List<TblUsersModel> convertDataRowToUserList(List<DataRow> dataRowList)
        {
            var userList = new List<TblUsersModel>();

            foreach (DataRow dataRow in dataRowList)
            {
                var user = DataRowToObject.ToObject<TblUsersModel>(dataRow);
                userList.Add(user);
            }

            return userList;
        }

        private TblUsersModel convertDataRowToUser(DataRow dataRow)
        {
            return DataRowToObject.ToObject<TblUsersModel>(dataRow);
        }

        private List<UserResponseModel> convertUserListToResponseModelList(List<TblUsersModel> userList)
        {
            var userResponseModels = new List<UserResponseModel>();

            foreach (TblUsersModel user in userList)
            {
                var userAccessModelListResponse = new Dictionary<string, List<int>>();
                foreach (UserAccessModel userAccessModel in user.userAccessModels)
                {
                    var userAccessTypes = new List<int>();
                    foreach (UserAccessType userAccessType in userAccessModel.userAccess)
                    {
                        userAccessTypes.Add(userAccessType.Code);
                    }
                    userAccessModelListResponse.TryAdd(userAccessModel.module, userAccessTypes);
                }

                var userResponseModel = new UserResponseModel()
                {
                    Id= user.Id,
                    FilePath= user.FilePath,
                    Username = user.Username,
                    Fullname = user.Fullname,
                    Fname = user.Fname,
                    Lname = user.Lname,
                    Mname = user.Mname,
                    Email = user.Email,
                    Gender = user.Gender,
                    EmployeeId = user.EmployeeId,
                    Active = user.Active,
                    Cno = user.Cno,
                    Address = user.Address,
                    CenterId = user.CenterId,
                    AgreementStatus = user.AgreementStatus,
                    userAccessList = userAccessModelListResponse
                };
                userResponseModels.Add(userResponseModel);
            }

            return userResponseModels;
        }
        private IQueryable<TblUsersModel> buildUserManagementSearchQuery(Dictionary<string, object> filter)
        {
            IQueryable<TblUsersModel> query = _context.TblUsersModels;

            query = query
                .Include(user => user.userAccessModels)
                .ThenInclude(userAccessModel => userAccessModel.userAccess)
                .Where(user => !user.DeleteFlag);
                

            // assuming that you return all records when nothing is specified in the filter

            if (filter.ContainsKey("searchParam"))
            {
                var searchParam = filter["searchParam"].ToString();
                query = query.Where(user =>
                               user.Fname.Contains(searchParam) ||
                               user.Lname.Contains(searchParam) ||
                               user.Mname.Contains(searchParam) ||
                               user.Email.Contains(searchParam));
            }

            if (filter.ContainsKey("forApproval") && Convert.ToBoolean(filter["forApproval"]))
            {
                query = query.Where(user => user.Status.Equals(3));
            }

            if (filter.ContainsKey("username"))
            {
                var username = filter["username"].ToString();
                query = query.Where(user => user.Username.Equals(username));
            }


            if (filter.ContainsKey("Id")) {
                var id = filter["Id"];
                query = query.Where(user => user.Id.Equals(id));
            }

            query = query.OrderByDescending(e => e.Id);

            return query;
        }

        private UserAccessListModel populateUserAccessListModel(TblUsersModel usersModel)
        {
            var userAccessModels = new UserAccessListModel();
            userAccessModels.username = usersModel.Username;

            var userAccessList = new Dictionary<string, List<int>>();
            foreach (UserAccessModel userAccessModel in usersModel.userAccessModels)
            {

                var userAccessTypeList = new List<int>();
                foreach (UserAccessType userAccessType in userAccessModel.userAccess)
                {
                    userAccessTypeList.Add(userAccessType.Code);
                }

                userAccessList.Add(userAccessModel.module, userAccessTypeList);
            }

            userAccessModels.userAccessList = userAccessList;
            return userAccessModels;
        }

        private void populateUserAccess(TblUsersModel userModel, UserUpdateModel updateModel)
        {
            userModel.userAccessModels.AddRange(populateUserAccessList(updateModel.userAccess));
        }

        private List<UserAccessModel> populateUserAccessList(Dictionary<string, List<int>> userAccessModelList)
        {
            var userAccessModels = new List<UserAccessModel>();

            foreach (var access in userAccessModelList)
            {
                var userAccessTypeList = new List<UserAccessType>();
                foreach (int userAccess in access.Value)
                {
                    var userAccessType = new UserAccessType()
                    {
                        Code = userAccess
                    };
                    _context.Attach(userAccessType);

                    userAccessTypeList.Add(userAccessType);
                }

                var userAccessModel = new UserAccessModel()
                {
                    module = access.Key
                };

                _context.Attach(userAccessModel);

                userAccessModel.userAccess.AddRange(userAccessTypeList);
                userAccessModels.Add(userAccessModel);
            }
            return userAccessModels;
        }
    }
}
