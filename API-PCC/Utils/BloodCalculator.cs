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

                string formula = "";

                var bloodCompRecords = _context.ABloodComps;

                var sireRecord = _context.ABuffAnimals.Where(animal => animal.breedRegistryNumber.Equals(bloodCalculatorModel.sireBreedRegistryNumber))
                                        .Join(bloodCompRecords, animal => animal.BloodCode, bloodComp => bloodComp.BloodCode,
                                        (animal, bloodComp) => new { animalIdNumber = animal.AnimalIdNumber, bloodCode = bloodComp.BloodCode, bloodDesc = bloodComp.BloodDesc });
                var damRecord = _context.ABuffAnimals.Where(animal => animal.breedRegistryNumber.Equals(bloodCalculatorModel.damBreedRegistryNumber))
                                        .Join(bloodCompRecords, animal => animal.BloodCode, bloodComp => bloodComp.BloodCode,
                                        (animal, bloodComp) => new { animalIdNumber = animal.AnimalIdNumber, bloodCode = bloodComp.BloodCode, bloodDesc = bloodComp.BloodDesc });

                var sireValue = getValue(sireRecord.First().bloodDesc);
                var damValue = getValue(damRecord.First().bloodDesc);

                foreach (TblBLoodCalculator bloodCalculator in bloodCalculators)
                {
                    if (bloodCalculator.Criteria.IsNullOrEmpty())
                    {
                        continue;
                    }

                    if (filterCriteria(sireValue, damValue, bloodCalculator.Criteria))
                    {
                        formula = bloodCalculator.Formula;
                        formula = formula.Replace("sire", sireValue.ToString());
                        formula = formula.Replace("dam", damValue.ToString());
                        break;
                    }
                }

                var bloodCompValue = (double)formula.EvalNumerical();

                var bloodCompRecord = _context.ABloodComps.Where(bloodComp => bloodComp.From <= bloodCompValue && bloodComp.To >= bloodCompValue).FirstOrDefault();

                if (bloodCompRecord == null)
                {
                    throw new Exception("Calculated Value did not match a Blood Composition Type!");
                }
                return bloodCompRecord;
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
    }
}
