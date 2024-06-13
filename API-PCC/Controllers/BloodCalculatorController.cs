using AngouriMath.Extensions;
using API_PCC.ApplicationModels;
using API_PCC.Data;
using API_PCC.EntityModels;
using API_PCC.Models;
using API_PCC.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using System.Text.RegularExpressions;

namespace API_PCC.Controllers
{
    [Authorize("ApiKey")]
    [Route("[controller]/[action]")]
    [ApiController]
    public class BloodCalculatorController : ControllerBase
    {
        private readonly PCC_DEVContext _context;
        private readonly BloodCalculator _bloodCalculator;
        public BloodCalculatorController (PCC_DEVContext context)
        {
            _context = context;
            _bloodCalculator = new BloodCalculator(context);
        }

        [HttpPost]
        public async Task<IActionResult> compute(BloodCalculatorModel bloodCalculatorModel)
        {
            try
            {
                var bloodComp = _bloodCalculator.compute(bloodCalculatorModel);
                if (bloodComp == null)
                {
                    return Problem("Problem in Calculating Blood Composition");
                }
                return Ok(bloodComp);
            }
            catch (BadHttpRequestException ex)
            {
                return BadRequest(ex.GetBaseException().ToString());
            }
            catch (Exception ex)
            {
                return Problem(ex.GetBaseException().ToString());
            }
        }
    }
}
