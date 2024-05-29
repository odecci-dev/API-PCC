using API_PCC.ApplicationModels;
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
using API_PCC.EntityModels;
using System.Data.SqlClient;
using static API_PCC.Controllers.UserController;
using AngouriMath.Extensions;
using Microsoft.IdentityModel.Tokens;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;

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
        public async Task<ActionResult<IEnumerable<BuffAnimalPagedModel>>> list(BuffAnimalSearchFilterModel searchFilter)
        {
            SortRequestToColumnNameConverter.convert(searchFilter.sortBy);

            try
            {
                DataTable queryResult = db.SelectDb_WithParamAndSorting(QueryBuilder.buildBuffAnimalSearch(searchFilter), searchFilter.sortBy, populateSqlParameters(searchFilter));

                var result = buildBuffAnimalPagedModel(searchFilter, queryResult);
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
        public async Task<ActionResult<BuffAnimalBaseModel>> search(String referenceNumber)
        {
            DataTable dt = db.SelectDb(QueryBuilder.buildBuffAnimalSearchByReferenceNumber(referenceNumber)).Tables[0];

            if (dt.Rows.Count == 0)
            {
                return Conflict("No records found!");
            }

            var animalModel= convertDataRowToBuffAnimalModel(dt.Rows[0]);

            return Ok(animalModel);
        }

        // GET: BuffAnimals/search/5
        // search by registrationNumber and RFID number
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BuffAnimalListResponseModel>>> view()
        {
            try { 
                DataTable dt = db.SelectDb(QueryBuilder.buildBuffAnimalSearchAll()).Tables[0];

                if (dt.Rows.Count == 0)
                {
                    return Conflict("No records found!");
                }

                var animalModelResponseList = convertDataRowListToBuffAnimalResponseModelList(dt.AsEnumerable().ToList());

                return Ok(animalModelResponseList);
            
            }
            catch (Exception ex)
            {
                return Problem(ex.GetBaseException().ToString());
            }
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

            var buffAnimal = convertDataRowToBuffAnimalEntityModel(buffAnimalDataTable.Rows[0]);
            
            DataTable sireRecordsCheck = db.SelectDb(QueryBuilder.buildSireSearchQueryById(buffAnimal.SireId)).Tables[0];

            if (sireRecordsCheck.Rows.Count == 0)
            {
                return Conflict("Sire does not exists");
            }

            string sire_update = $@"UPDATE [dbo].[tbl_SireModel] SET 
                                             [Sire_Registration_Number] = '" + updateModel.Sire.RegistrationNumber + "'" +
                                            ",[Sire_Id_Number] = '" + updateModel.Sire.IdNumber + "'" +
                                            ",[Sire_Name] = '" + updateModel.Sire.Name + "'" +
                                            ",[Breed_Code] = '" + updateModel.Sire.BreedCode + "'" +
                                            ",[Blood_Code] = '" + updateModel.Sire.BloodCode + "'" +
                                            " WHERE id = " + buffAnimal.SireId;
            string sireUpdateResult = db.DB_WithParam(sire_update);

            DataTable damRecordsCheck = db.SelectDb(QueryBuilder.buildSireSearchQueryById(buffAnimal.DamId)).Tables[0];

            if (damRecordsCheck.Rows.Count == 0)
            {
                return Conflict("Dam does not exists");
            }

            string dam_update = $@"UPDATE [dbo].[tbl_DamModel] SET 
                                             [Dam_Registration_Number] = '" + updateModel.Dam.RegistrationNumber + "'" +
                                            ",[Dam_Id_Number] = '" + updateModel.Dam.IdNumber + "'" +
                                            ",[Dam_Name] = '" + updateModel.Dam.Name + "'" +
                                            ",[Breed_Code] = '" + updateModel.Dam.BreedCode + "'" +
                                            ",[Blood_Code] = '" + updateModel.Dam.BloodCode + "'" +
                                            " WHERE id = " + buffAnimal.DamId;
            string damUpdateResult = db.DB_WithParam(dam_update);


            DataTable originOfAcquisition = db.SelectDb(QueryBuilder.buildOriginAcquisitionSearchQueryById(buffAnimal.OriginOfAcquisition)).Tables[0];

            if (originOfAcquisition.Rows.Count == 0)
            {
                return Conflict("Origin of Acquisition does not exists");
            }

            string origin_of_acquisition_update = $@"UPDATE [dbo].[tbl_OriginOfAcquisitionModel] SET
                                            [City] = '" + updateModel.OriginOfAcquisition.City + "'," +
                                            "[Province] = '" + updateModel.OriginOfAcquisition.Province + "'," +
                                            "[Barangay] = '" + updateModel.OriginOfAcquisition.Barangay + "'," +
                                            "[Region] = '" + updateModel.OriginOfAcquisition.Region + "' " +
                                            "WHERE id = " + buffAnimal.OriginOfAcquisition;

            string originOfAcquistionResult = db.DB_WithParam(origin_of_acquisition_update);

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
            if (_context.ABuffAnimals == null)
            {
                return Problem("Buff Animal entity Set is null!");
            }

            try
            {
                var duplicateRecordCheck = _context.ABuffAnimals
                                            .Where(buffAnimal => !buffAnimal.DeleteFlag &&
                                                   buffAnimal.HerdCode.Equals(buffAnimalRegistrationModel.HerdCode) &&
                                                   buffAnimal.AnimalIdNumber.Equals(buffAnimalRegistrationModel.AnimalIdNumber))
                                            .FirstOrDefault();

                if (duplicateRecordCheck != null)
                {
                    return Conflict("Buff Animal already exists");
                }

                var buffAnimal = buildBuffAnimal(buffAnimalRegistrationModel);

                var sireRecord = animalRecordCheck(buffAnimalRegistrationModel.Sire);

                if (sireRecord == null)
                {
                    var sire = buildBuffAnimal(buffAnimalRegistrationModel.Sire);
                    var sireModel = _context.ABuffAnimals.Add(sire);
                    sireRecord = sireModel.Entity;
                }

                var damRecord = animalRecordCheck(buffAnimalRegistrationModel.Dam);

                if (damRecord == null)
                {
                    var dam = buildBuffAnimal(buffAnimalRegistrationModel.Dam);
                    var damModel = _context.ABuffAnimals.Add(dam);
                    damRecord = damModel.Entity;
                }


                var originOfAcquisitionRecord = originOfAcquistionRecordCheck(buffAnimalRegistrationModel.OriginOfAcquisition);

                if (originOfAcquisitionRecord == null)
                {
                    var originOfAcquistion = buildOriginOfAcquistion(buffAnimalRegistrationModel.OriginOfAcquisition);

                    var originOfAcquistionModel = _context.OriginOfAcquisitionModels.Add(originOfAcquistion);
                    originOfAcquisitionRecord = originOfAcquistionModel.Entity;
                }

                await _context.SaveChangesAsync();

                buffAnimal.SireId = sireRecord.Id;
                buffAnimal.DamId = damRecord.Id;
                buffAnimal.OriginOfAcquisition = originOfAcquisitionRecord.Id;
                buffAnimal.CreatedBy = buffAnimalRegistrationModel.CreatedBy;
                buffAnimal.CreatedDate = DateTime.Now;
                buffAnimal.BloodCode = 
                buffAnimal.Status = "1";

                var bloodCompDetails = getBloodCode(sireRecord.bloodComp, damRecord.bloodComp);

                buffAnimal.bloodComp = (double) bloodCompDetails.GetValueOrDefault("bloodCompValue")!;
                buffAnimal.BloodCode = (string) bloodCompDetails.GetValueOrDefault("bloodCompCode")!;

                _context.ABuffAnimals.Add(buffAnimal);
                await _context.SaveChangesAsync();

                return CreatedAtAction("save", new { id = buffAnimal.Id }, buffAnimal);
            }
            catch (Exception ex)
            {

                return Problem(ex.GetBaseException().ToString());
            }
        }

        private Dictionary<string, object> getBloodCode(double sire, double dam)
        {
            var bloodCalculators = _context.bloodCalculators.AsEnumerable().ToList();
            string formula = "";
            foreach (TblBLoodCalculator bloodCalculator in bloodCalculators)
            {
                if (bloodCalculator.Criteria.IsNullOrEmpty())
                {
                    continue;
                }
                if (filterCriteria(sire, dam, bloodCalculator.Criteria))
                {
                    formula = bloodCalculator.Formula;
                    formula = formula.Replace("sire", sire.ToString());
                    formula = formula.Replace("dam", dam.ToString());
                    break;
                }
            }

            var bloodCompDetails = new Dictionary<string, object>();

            var bloodCompValue = (double)formula.EvalNumerical();

            var bloodCompRecord = _context.ABloodComps.Where(bloodComp => bloodComp.From <= bloodCompValue && bloodComp.To >= bloodCompValue).FirstOrDefault();

            bloodCompDetails.Add("bloodCompValue", bloodCompValue);
            bloodCompDetails.Add("bloodCompCode", bloodCompRecord.BloodCode);

            return bloodCompDetails;
        }

        private bool filterCriteria(double sire, double dam, string filter = null)
        {

            var sireParam = Expression.Parameter(typeof(double), "sire");
            var damParam = Expression.Parameter(typeof(double), "dam");

            // Add Filter string and parameters
            var e = (Expression)DynamicExpressionParser.ParseLambda(new[] { sireParam, damParam }, null, filter);

            // convert to Expression
            var typedExpression = (Expression<Func<double, double, bool>>)e;

            // Use as a condition
            bool filterCheck = typedExpression.Compile().Invoke(sire, dam);

            return filterCheck;
        }

        private TblOriginOfAcquisitionModel buildOriginOfAcquistion(OriginOfAcquisitionModel originOfAcquisitionModel)
        {
            var originOfAcquistionModel = new TblOriginOfAcquisitionModel()
            {
                City = originOfAcquisitionModel.City,
                Province = originOfAcquisitionModel.Province,
                Barangay = originOfAcquisitionModel.Barangay,
                Region = originOfAcquisitionModel.Region
            };

            return originOfAcquistionModel;
        }


        private ABuffAnimal animalRecordCheck(Animal animal)
        {
            var animalRecord = _context.ABuffAnimals
                                        .Where(buffAnimal => buffAnimal.RfidNumber.Equals(animal.RegistrationNumber) &&
                                                buffAnimal.AnimalIdNumber.Equals(animal.IdNumber) &&
                                                buffAnimal.AnimalName.Equals(animal.Name) &&
                                                buffAnimal.BreedCode.Equals(animal.BreedCode) &&
                                                buffAnimal.BloodCode.Equals(animal.BloodCode))
                                        .FirstOrDefault();
            return animalRecord;
        }

        private TblOriginOfAcquisitionModel originOfAcquistionRecordCheck(OriginOfAcquisitionModel originOfAcquisitionModel)
        {
            var originOfAcquisitionRecord = _context.OriginOfAcquisitionModels
                                        .Where(originOfAcquistion => originOfAcquistion.City.Equals(originOfAcquisitionModel.City) &&
                                                originOfAcquistion.Province.Equals(originOfAcquisitionModel.Province) &&
                                                originOfAcquistion.Barangay.Equals(originOfAcquisitionModel.Barangay) &&
                                                originOfAcquistion.Region.Equals(originOfAcquisitionModel.Region))
                                        .FirstOrDefault();
            return originOfAcquisitionRecord;
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

            var buffAnimal = convertDataRowListToBuffAnimalResponseModelList(items);

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

        private List<BuffAnimalListResponseModel> convertDataRowListToBuffAnimalResponseModelList(List<DataRow> dataRowList)
        {
            var buffAnimalResponseModelList = new List<BuffAnimalListResponseModel>();

            foreach (DataRow row in dataRowList)
            {
                buffAnimalResponseModelList.Add(convertDataRowToBuffAnimalResponseModel(row));
            }

            return buffAnimalResponseModelList;
        }

        private BuffAnimalListResponseModel convertDataRowToBuffAnimalResponseModel(DataRow datarow)
        {
            var buffAnimalEntityModel = DataRowToObject.ToObject<ABuffAnimal>(datarow);
            var OriginOfAcquisition = populateOriginOfAcquistionModel(buffAnimalEntityModel);
            var Sire = populateSireModel(buffAnimalEntityModel);
            var Dam = populateDamModel(buffAnimalEntityModel);
            var farmOwner = populateOwnerModel(buffAnimalEntityModel.HerdCode);

            string Fname = farmOwner == null ? "N/A" : farmOwner.FirstName;
            string Lname = farmOwner == null ? "N/A" : farmOwner.FirstName;
            var buffAnimalResponseModel = new BuffAnimalListResponseModel()
            {
                BreedRegNo = Dam.RegistrationNumber,
                HerdCode = buffAnimalEntityModel.HerdCode,
                AnimalIdNumber = buffAnimalEntityModel.AnimalIdNumber,
                Photo = buffAnimalEntityModel.Photo,
                Id = buffAnimalEntityModel.Id,
                Owner = Fname + " " + Lname,
                DateOfAcquisition = buffAnimalEntityModel.DateOfAcquisition?.ToString("yyyy-MM-dd")
            };

            return buffAnimalResponseModel;
        }

        private TblFarmOwner populateOwnerModel(string herdCode)
        {
            //DataTable dt = db.SelectDb(QueryBuilder.buildHerdOwnerJoinQuery(herdCode)).Tables[0];
            //if (dt.Rows.Count == 0)
            //{
            //    throw new Exception("Farmer Record not found!");
            //}
            //var farmOwnerModel = convertDataRowToFarmOwnerModel(dt.Rows[0]);
            //return farmOwnerModel;
            var farmOwnerModel = (dynamic)null;
            DataTable dt = db.SelectDb(QueryBuilder.buildHerdOwnerJoinQuery(herdCode)).Tables[0];
            if (dt.Rows.Count != 0)
            {
                 farmOwnerModel = convertDataRowToFarmOwnerModel(dt.Rows[0]);
            }
    
            return farmOwnerModel;

        }
        private OriginOfAcquisitionModel populateOriginOfAcquistionModel(ABuffAnimal buffAnimal)
        {
            DataTable dt = db.SelectDb(QueryBuilder.buildOriginAcquisitionSearchQueryById(buffAnimal.OriginOfAcquisition)).Tables[0];
            if (dt.Rows.Count == 0)
            {
                throw new Exception("Acquisition Record not found!");
            }
            var originOfAcquistionEntity = convertDataRowToOriginAcquistionModel(dt.Rows[0]);
            var originOfAcquisitionModel = new OriginOfAcquisitionModel()
            {
                City = originOfAcquistionEntity.City,
                Barangay = originOfAcquistionEntity.Barangay,
                Province = originOfAcquistionEntity.Province,
                Region = originOfAcquistionEntity.Region
            };
            return originOfAcquisitionModel;

        }

        private Animal populateSireModel(ABuffAnimal buffAnimal)
        {
            DataTable dt = db.SelectDb(QueryBuilder.buildSireSearchQueryById(buffAnimal.SireId)).Tables[0];
            if (dt.Rows.Count == 0)
            {
                throw new Exception("Sire Record not found!");
            }
            var sireEntity = convertDataRowToSireModel(dt.Rows[0]);
            var sireModel = new Animal()
            {
                RegistrationNumber = sireEntity.SireRegistrationNumber,
                IdNumber = sireEntity.SireIdNumber,
                Name = sireEntity.SireName,
                BreedCode = sireEntity.BreedCode,
                BloodCode = sireEntity.BloodCode
            };
            return sireModel;
        }

        private Animal populateDamModel(ABuffAnimal buffAnimal)
        {
            DataTable dt = db.SelectDb(QueryBuilder.buildDamSearchQueryById(buffAnimal.DamId)).Tables[0];
            if (dt.Rows.Count == 0)
            {
                throw new Exception("Dam Record not found!");
            }
            var damEntity = convertDataRowToDamModel(dt.Rows[0]);
            var damModel = new Animal()
            {
                RegistrationNumber = damEntity.DamRegistrationNumber,
                IdNumber = damEntity.DamIdNumber,
                Name = damEntity.DamName,
                BreedCode = damEntity.BreedCode,
                BloodCode = damEntity.BloodCode
            };
            return damModel;
        }

        private TblOriginOfAcquisitionModel convertDataRowToOriginAcquistionModel(DataRow dataRow) 
        {
            return DataRowToObject.ToObject<TblOriginOfAcquisitionModel>(dataRow);
        }

        private SireModel convertDataRowToSireModel(DataRow dataRow)
        {
            return DataRowToObject.ToObject<SireModel>(dataRow);
        }

        private DamModel convertDataRowToDamModel(DataRow dataRow)
        {
            return DataRowToObject.ToObject<DamModel>(dataRow);
        }

        private ABuffAnimal convertDataRowToBuffAnimalEntityModel(DataRow dataRow)
        {
            var buuffAnimalEntityModel = DataRowToObject.ToObject<ABuffAnimal>(dataRow);
            return buuffAnimalEntityModel;
        }

        private BuffAnimalBaseModel convertDataRowToBuffAnimalModel(DataRow datarow)
        {
            var buffAnimalEntityModel = DataRowToObject.ToObject<ABuffAnimal>(datarow);
            var buffAnimalResponseModel = new BuffAnimalBaseModel()
            {
                Id = buffAnimalEntityModel.Id,
                AnimalIdNumber = buffAnimalEntityModel.AnimalIdNumber,
                AnimalName = buffAnimalEntityModel.AnimalName,
                Photo = buffAnimalEntityModel.Photo,
                HerdCode = buffAnimalEntityModel.HerdCode,
                RfidNumber = buffAnimalEntityModel.RfidNumber,
                DateOfBirth = buffAnimalEntityModel?.DateOfBirth,
                Sex = buffAnimalEntityModel.Sex,
                BreedCode = buffAnimalEntityModel.BreedCode,
                BirthType = buffAnimalEntityModel.BirthType,
                CountryOfBirth = buffAnimalEntityModel.CountryOfBirth,
                OriginOfAcquisition = populateOriginOfAcquistionModel(buffAnimalEntityModel),
                DateOfAcquisition = buffAnimalEntityModel.DateOfAcquisition,
                Marking = buffAnimalEntityModel.Marking,
                TypeOfOwnership = buffAnimalEntityModel.TypeOfOwnership,
                BloodCode = buffAnimalEntityModel.BloodCode,
                Sire = populateSireModel(buffAnimalEntityModel),
                Dam = populateDamModel(buffAnimalEntityModel)
            };

            return buffAnimalResponseModel;
        }

        private TblFarmOwner convertDataRowToFarmOwnerModel(DataRow dataRow)
        {
            return DataRowToObject.ToObject<TblFarmOwner>(dataRow);
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
                DateOfAcquisition = registrationModel.DateOfAcquisition,
                Marking = registrationModel.Marking,
                TypeOfOwnership = registrationModel.TypeOfOwnership
                // To be calculated BloodCode = registrationModel.BloodCode
            };
            return buffAnimal;
        }

        private ABuffAnimal buildBuffAnimal(Animal animal)
        {
            var buffAnimal = new ABuffAnimal()
            {
                AnimalIdNumber = animal.IdNumber,
                AnimalName = animal.Name,
                RfidNumber = animal.RegistrationNumber,
                BreedCode = animal.BreedCode,
                BloodCode = animal.BloodCode,
                bloodComp = animal.bloodComp,
                CreatedDate = DateTime.Now
            };

            return buffAnimal;
        }

        private SqlParameter[] populateSqlParameters(BuffAnimalSearchFilterModel searchFilter)
        {

            var sqlParameters = new List<SqlParameter>();

            if (searchFilter.searchValue != null && searchFilter.searchValue != "")
            {
                sqlParameters.Add(new SqlParameter
                {
                    ParameterName = "SearchParam",
                    Value = searchFilter.searchValue ?? Convert.DBNull,
                    SqlDbType = System.Data.SqlDbType.VarChar,
                });
            }

            if (searchFilter.sex != null && searchFilter.sex != "")
            {
                sqlParameters.Add(new SqlParameter
                {
                    ParameterName = "Sex",
                    Value = searchFilter.sex ?? Convert.DBNull,
                    SqlDbType = System.Data.SqlDbType.VarChar,
                });
            }

            if (searchFilter.status != null && searchFilter.status != "")
            {
                sqlParameters.Add(new SqlParameter
                {
                    ParameterName = "Status",
                    Value = searchFilter.status ?? Convert.DBNull,
                    SqlDbType = System.Data.SqlDbType.VarChar,
                });
            }

            if (searchFilter.filterBy.BloodCode != null && searchFilter.filterBy.BloodCode != "")
            {
                sqlParameters.Add(new SqlParameter
                {
                    ParameterName = "BloodCode",
                    Value = searchFilter.filterBy.BloodCode ?? Convert.DBNull,
                    SqlDbType = System.Data.SqlDbType.VarChar,
                });
            }

            if (searchFilter.filterBy.BreedCode != null && searchFilter.filterBy.BreedCode != "")
            {
                sqlParameters.Add(new SqlParameter
                {
                    ParameterName = "BreedCode",
                    Value = searchFilter.filterBy.BreedCode ?? Convert.DBNull,
                    SqlDbType = System.Data.SqlDbType.VarChar,
                });
            }

            if (searchFilter.filterBy.TypeOfOwnership != null && searchFilter.filterBy.TypeOfOwnership != "")
            {
                sqlParameters.Add(new SqlParameter
                {
                    ParameterName = "TypeOfOwnership",
                    Value = searchFilter.filterBy.TypeOfOwnership ?? Convert.DBNull,
                    SqlDbType = System.Data.SqlDbType.VarChar,
                });
            }

            return sqlParameters.ToArray();
        }

    }
}
