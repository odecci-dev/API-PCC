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
                var farmerAffiliationList = _context.HFarmerAffiliations.Where(farmerAffiliation => !farmerAffiliation.DeleteFlag &&
                                                                                                    (farmerAffiliation.FCode.Equals(searchFilter.searchParam) || 
                                                                                                     farmerAffiliation.FDesc.Equals(searchFilter.searchParam))).ToList();
                var result = buildFarmerAffiliationPagedModel(searchFilter, farmerAffiliationList);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Problem(ex.GetBaseException().ToString());
            }
        }

        // GET: FarmerAffiliations/search/5
        [HttpGet("{fcode}")]
        public async Task<ActionResult<IEnumerable<FarmerAffiliationResponseModel>>> search(string fcode)
        {
            try
            {
                var farmerAffiliationList = _context.HFarmerAffiliations.Where(farmerAffiliation => !farmerAffiliation.DeleteFlag && farmerAffiliation.FCode.Equals(fcode)).ToList();
                if (farmerAffiliationList == null)
                {
                    return Conflict("No records found!");
                }

                List<FarmerAffiliationResponseModel> herdClassificationResponseModels = convertFarmerAffiliationToResponseModelList(farmerAffiliationList);

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
                var farmerAffiliationRecords = _context.HFarmerAffiliations.Where(farmerAffiliation => !farmerAffiliation.DeleteFlag).ToList();
                if (farmerAffiliationRecords == null)
                {
                    return Conflict("No records found!");
                }

                List<FarmerAffiliationResponseModel> herdClassificationResponseModels = convertFarmerAffiliationToResponseModelList(farmerAffiliationRecords);

                return Ok(herdClassificationResponseModels);
            }
            catch (Exception ex)
            {
                return Problem(ex.GetBaseException().ToString());
            }
        }

        // PUT: FarmerAffiliations/update/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> update(int id, FarmerAffiliationUpdateModel farmerAffiliationUpdateModel)
        {
            var farmerAffiliationRecord = _context.HFarmerAffiliations.Where(farmerAffiliation => !farmerAffiliation.DeleteFlag && farmerAffiliation.Id.Equals(id)).FirstOrDefault();
            if (farmerAffiliationRecord == null)
            {
                return Conflict("No records matched!");
            }

            var farmerAffiliationDuplicateCheck = _context.HFarmerAffiliations.Where(farmerAffiliation => !farmerAffiliation.DeleteFlag && 
                                                                                                          !farmerAffiliation.Id.Equals(id) &&
                                                                                                          farmerAffiliation.FCode.Equals(farmerAffiliationUpdateModel.FCode) &&
                                                                                                          farmerAffiliation.FDesc.Equals(farmerAffiliationUpdateModel.FDesc)).FirstOrDefault();
            // check for duplication
            if (farmerAffiliationDuplicateCheck == null)
            {
                return Conflict("Entity already exists");
            }

            try
            {
                populateFarmerAffiliation(farmerAffiliationRecord, farmerAffiliationUpdateModel);
                _context.Entry(farmerAffiliationRecord).State = EntityState.Modified;
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
            var farmerAffiliationDuplicateCheck = _context.HFarmerAffiliations.Where(farmerAffiliation => !farmerAffiliation.DeleteFlag &&
                                                                                                           farmerAffiliation.FCode.Equals(farmerAffiliationRegistrationModel.FCode) &&
                                                                                                           farmerAffiliation.FDesc.Equals(farmerAffiliationRegistrationModel.FDesc)).FirstOrDefault();
            // check for duplication
            if (farmerAffiliationDuplicateCheck != null)
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

        // POST: FarmerAffiliations/delete/5
        [HttpPost]
        public async Task<IActionResult> delete(DeletionModel deletionModel)
        {
            var farmerAffiliationModel = _context.HFarmerAffiliations.Where(farmerAffiliation => !farmerAffiliation.DeleteFlag &&
                                                                                                   farmerAffiliation.Id.Equals(deletionModel.id)).FirstOrDefault();

            if (farmerAffiliationModel == null)
            {
                return Conflict("No records matched!");
            }

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
            var farmerAffiliationModel = _context.HFarmerAffiliations.Where(farmerAffiliation => !farmerAffiliation.DeleteFlag && 
                                                                                                   farmerAffiliation.Id.Equals(restorationModel.id)).FirstOrDefault();
            if (farmerAffiliationModel == null)
            {
                return Conflict("No deleted records matched!");
            }

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

        private List<FarmerAffiliationPagedModel> buildFarmerAffiliationPagedModel(CommonSearchFilterModel searchFilter, List<HFarmerAffiliation> farmerAffiliations)
        {

            int pagesize = searchFilter.pageSize == 0 ? 10 : searchFilter.pageSize;
            int page = searchFilter.page == 0 ? 1 : searchFilter.page;
            var items = (dynamic)null;

            int totalItems = farmerAffiliations.Count;
            int totalPages = (int)Math.Ceiling((double)totalItems / pagesize);
            items = farmerAffiliations.Skip((page - 1) * pagesize).Take(pagesize).ToList();

            List<FarmerAffiliationResponseModel> famerAffiliationResponseModels = convertFarmerAffiliationToResponseModelList(farmerAffiliations);

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
        private void populateFarmerAffiliation(HFarmerAffiliation farmerAffiliation, FarmerAffiliationUpdateModel farmerAffiliationUpdateModel)
        {
            farmerAffiliation.FCode = farmerAffiliationUpdateModel.FCode;
            farmerAffiliation.FDesc = farmerAffiliationUpdateModel.FDesc;
            farmerAffiliation.DateUpdated = DateTime.Now;
            farmerAffiliation.UpdatedBy = farmerAffiliationUpdateModel.UpdatedBy;
        }
    }
}
