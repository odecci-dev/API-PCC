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
using Microsoft.IdentityModel.Tokens;
using System.Linq.Dynamic.Core;

namespace API_PCC.Controllers
{
    [Authorize("ApiKey")]
    [Route("[controller]/[action]")]
    [ApiController]
    public class UserTypeController : ControllerBase
    {
        private readonly PCC_DEVContext _context;

        public UserTypeController(PCC_DEVContext context)
        {
            _context = context;
        }
        [HttpPost]
        public async Task<ActionResult<IEnumerable<UserTypePagedModel>>> list(UserTypeSearchFilterModel searchFilter)
        {
            searchFilter.searchParam = StringSanitizer.sanitizeString(searchFilter.searchParam);
                
            try
            {
                List<TblUserTypeModel> userTypeList = await buildUserTypeSearchQuery(searchFilter).ToListAsync();

                var result = buildUserTypePagedModel(searchFilter, userTypeList);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Problem(ex.GetBaseException().ToString());
            }
        }

        private IQueryable<TblUserTypeModel> buildUserTypeSearchQuery(UserTypeSearchFilterModel searchFilter)
        {
            IQueryable<TblUserTypeModel> query = _context.tblUserTypeModels.Where(userType => !userType.DeleteFlag);

            // assuming that you return all records when nothing is specified in the filter

            if (!searchFilter.searchParam.IsNullOrEmpty())
                query = query.Where(userType =>
                               userType.code.Equals(searchFilter.searchParam) ||
                               userType.name.Equals(searchFilter.searchParam));


            if (!searchFilter.sortBy.Field.IsNullOrEmpty())
            {

                if (!searchFilter.sortBy.Sort.IsNullOrEmpty())
                {
                    query = query.OrderBy(searchFilter.sortBy.Field + " " + searchFilter.sortBy.Sort);
                }
                else
                {
                    query = query.OrderBy(searchFilter.sortBy.Field + " asc");

                }
            }
            else
            {
                query = query.OrderByDescending(herd => herd.Id);
            }

            return query;
        }

        private List<UserTypePagedModel> buildUserTypePagedModel(UserTypeSearchFilterModel searchFilter, List<TblUserTypeModel> userTypes)
        {
            int pagesize = searchFilter.pageSize == 0 ? 10 : searchFilter.pageSize;
            int page = searchFilter.page == 0 ? 1 : searchFilter.page;
            var items = (dynamic)null;
            int totalItems = 0;
            int totalPages = 0;

            totalItems = userTypes.Count;
            totalPages = (int)Math.Ceiling((double)totalItems / pagesize);
            items = userTypes.Skip((page - 1) * pagesize).Take(pagesize).ToList();
            
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

        // GET: userType/search/5
        [HttpGet("{name}")]
        public async Task<ActionResult<TblUserTypeModel>> view(string name)
        {
            var userTypeModel = _context.tblUserTypeModels.Where(userType => !userType.DeleteFlag && 
                                                                              userType.name.Equals(name)).FirstOrDefault();
            if (userTypeModel == null)
            {
                return Conflict("No records found!");
            }

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
            var userTypeModel = _context.tblUserTypeModels.Where(userType => !userType.DeleteFlag && userType.Id.Equals(id)).FirstOrDefault();
            if (userTypeModel == null)
            {
                return Conflict("No records matched!");
            }

            var userTypeDuplicateCheck = _context.tblUserTypeModels.Where(userType => !userType.DeleteFlag && 
                                                                                      !userType.Id.Equals(id) &&
                                                                                      (userType.code.Equals(userTypeUpdateModel.Code) &&
                                                                                       userType.name.Equals(userTypeUpdateModel.Name))).FirstOrDefault();

            // check for duplication
            if (userTypeDuplicateCheck != null)
            {
                return Conflict("Entity already exists");
            }

            try
            {
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
            var userTypeDuplicateCheck = _context.tblUserTypeModels.Where(userType => !userType.DeleteFlag &&
                                                                                      (userType.code.Equals(userTypeRegistrationModel.Code) &&
                                                                                       userType.name.Equals(userTypeRegistrationModel.Name))).FirstOrDefault();

            // check for duplication
            if (userTypeDuplicateCheck != null)
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
            var userTypeModel = _context.tblUserTypeModels.Where(userType => !userType.DeleteFlag && userType.Id.Equals(deletionModel.id)).FirstOrDefault();

            if (userTypeModel == null)
            {
                return Conflict("No records matched!");
            }

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
            var userTypeModel = _context.tblUserTypeModels.Where(userType => userType.DeleteFlag && userType.Id.Equals(restorationModel.id)).FirstOrDefault();

            if (userTypeModel == null)
            {
                return Conflict("No deleted records matched!");
            }

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
