using AngouriMath.Extensions;
using API_PCC.ApplicationModels;
using API_PCC.Data;
using API_PCC.EntityModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;

namespace API_PCC.Controllers
{
    //[Authorize("ApiKey")]
    [Route("[controller]/[action]")]
    [ApiController]
    public class BloodCalculatorController : ControllerBase
    {
        private readonly PCC_DEVContext _context;

        public BloodCalculatorController (PCC_DEVContext context)
        {
            _context = context;
        }



        [HttpPost]
        public async Task<IActionResult> compute(BloodCalculatorModel bloodCalculatorModel)
        {
            var bloodCalculators = _context.bloodCalculators.AsEnumerable().ToList();
            string formula = "";
            foreach (TblBLoodCalculator bloodCalculator in bloodCalculators) 
            {
                if (bloodCalculator.Criteria.IsNullOrEmpty())
                {
                    continue;
                }
                if (filterCriteria(bloodCalculatorModel.sire, bloodCalculatorModel.dam, bloodCalculator.Criteria))
                {
                    formula = bloodCalculator.Formula;
                    formula = formula.Replace("sire", bloodCalculatorModel.sire.ToString());
                    formula = formula.Replace("dam", bloodCalculatorModel.dam.ToString());
                    break;
                }
            }

            var bloodCompValue = (double) formula.EvalNumerical();

            var bloodCompRecord = _context.ABloodComps.Where(bloodComp => bloodComp.From <= bloodCompValue && bloodComp.To >= bloodCompValue).ToList();

            return Ok(bloodCompRecord);
        }

        private bool filterCriteria(int sire, int dam, string filter = null)
        {
            
            var sireParam = Expression.Parameter(typeof(int), "sire");
            var damParam = Expression.Parameter(typeof(int), "dam");

            // Add Filter string and parameters
            var e = (Expression)DynamicExpressionParser.ParseLambda(new[] { sireParam, damParam }, null, filter);

            // convert to Expression
            var typedExpression = (Expression<Func<int, int, bool>>)e;

            // Use as a condition
            bool filterCheck = typedExpression.Compile().Invoke(sire, dam);

            return filterCheck;
        }


    }
}
