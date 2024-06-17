using AngouriMath.Extensions;
using API_PCC.ApplicationModels;
using API_PCC.Data;
using API_PCC.EntityModels;
using API_PCC.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using System.Text.RegularExpressions;

namespace API_PCC.Utils
{
    public class BloodCalculator
    {
        private readonly PCC_DEVContext _context;

        public BloodCalculator(PCC_DEVContext context)
        {
            _context = context;
        }

        public ABloodComp compute(BloodCalculatorModel bloodCalculatorModel)
        {
            try
            {
                var bloodCalculators = _context.bloodCalculators.AsEnumerable().ToList();

                if (bloodCalculators == null || bloodCalculators.Count == 0)
                {
                    throw new BadHttpRequestException ("No Blood Composition Formula Found!!");
                }

                var formula = "";

                var bloodCompRecords = _context.ABloodComps;
                if (bloodCompRecords.IsNullOrEmpty())
                {
                    //throw new Exception("No records found for Blood Composition!!");
                    return null;
                }

                var sire = _context.ABuffAnimals.Where(animal => animal.breedRegistryNumber.Equals(bloodCalculatorModel.sireBreedRegistryNumber));

                if (sire.IsNullOrEmpty())
                {
                    //throw new Exception("No records found for Sire with Registry Number: "+ bloodCalculatorModel.sireBreedRegistryNumber);
                    return null;
                }

                if (sire.First().BloodCode == null) 
                {
                    //throw new Exception("No Blood Code found for Sire !!");
                    return null;
                }

                var sireRecord = sire.Join(bloodCompRecords, animal => animal.BloodCode, bloodComp => bloodComp.BloodCode,
                                       (animal, bloodComp) => new { animalIdNumber = animal.AnimalIdNumber, bloodCode = bloodComp.BloodCode, bloodDesc = bloodComp.BloodDesc });

                if (sireRecord.IsNullOrEmpty())
                {
                    //throw new Exception("Sire's blood code: " + sire.First().BloodCode + " not found on Blood Composition Table!");
                    return null;
                }

                var sireResult = sireRecord.First();

                var dam = _context.ABuffAnimals.Where(animal => animal.breedRegistryNumber.Equals(bloodCalculatorModel.damBreedRegistryNumber));                       

                if (dam.IsNullOrEmpty())
                {
                    //throw new Exception("No records found for Dam with Registry Number: "+ bloodCalculatorModel.damBreedRegistryNumber);
                    return null;
                }

                if (dam.First().BloodCode == null)
                {
                    //throw new Exception("No Blood Code found for Dam !!");
                    return null;
                }

                var damRecord = dam.Join(bloodCompRecords, animal => animal.BloodCode, bloodComp => bloodComp.BloodCode,
                                        (animal, bloodComp) => new { animalIdNumber = animal.AnimalIdNumber, bloodCode = bloodComp.BloodCode, bloodDesc = bloodComp.BloodDesc });

                if (damRecord.IsNullOrEmpty())
                {
                    //throw new Exception("Dam's blood code: " + dam.First().BloodCode + " not found on Blood Composition Table!");
                    return null;
                }

                var damResult = damRecord.First();

                if (sireResult.bloodDesc.IsNullOrEmpty())
                {
                    //throw new Exception("No Blood Desc found for Sire !!");
                    return null;
                }

                if (damResult.bloodDesc.IsNullOrEmpty())
                {
                    //throw new Exception("No Blood Code found for Dam !!");
                    return null;
                }

                if (sireResult.bloodCode.IsNullOrEmpty())
                {
                    //throw new Exception("Blood Code empty for Sire's blood composition record" );
                    return null;

                }

                if (damResult.bloodCode.IsNullOrEmpty())
                {
                    //throw new Exception("Blood Code empty for Dam's blood composition record" );
                    return null;
                }

                var sireValue = getValue(sireResult.bloodDesc);
                var damValue = getValue(damResult.bloodDesc);

                var sireBloodCode = sireResult.bloodCode;
                var damBloodCode = damResult.bloodCode;

                foreach (TblBLoodCalculator bloodCalculator in bloodCalculators)
                {
                    if (bloodCalculator.Criteria.IsNullOrEmpty())
                    {
                        continue;
                    }

                    bool criteriaCheck = filterCriteria(sireBloodCode, damBloodCode, bloodCalculator.Criteria);

                    if (criteriaCheck)
                    {
                        formula = bloodCalculator.Formula;

                        if (formula.IsNullOrEmpty())
                        {
                            continue;
                        }
                        formula = formula.Replace("dam", damValue.ToString());
                        formula = formula.Replace("sire", sireValue.ToString());
                        break;
                    }
                }

                var bloodCompValue = (double) formula.EvalNumerical();

                var bloodCompRecord = _context.ABloodComps.Where(bloodComp => bloodComp.From <= bloodCompValue && bloodComp.To >= bloodCompValue);

                if (bloodCompRecord == null)
                {
                    throw new Exception("Calculated Value did not match a Blood Composition Type!");
                }
                return bloodCompRecord.First();
            }
            catch (BadHttpRequestException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.GetBaseException().ToString());
            }
        }

        private double getValue(string bloodDesc)
        {
            var value = Double.Parse(Regex.Match(bloodDesc, @"\d+").Value);
            return value;
        }

        private bool filterCriteria(string sire, string dam, string filter = null)
        {

            var sireParam = Expression.Parameter(typeof(string), "sire");
            var damParam = Expression.Parameter(typeof(string), "dam");

            // Add Filter string and parameters
            var e = (Expression)DynamicExpressionParser.ParseLambda(new[] { sireParam, damParam }, null, filter);

            // convert to Expression
            var typedExpression = (Expression<Func<string, string, bool>>)e;

            // Use as a condition
            bool filterCheck = typedExpression.Compile().Invoke(sire, dam);

            return filterCheck;
        }
    }
}
