using AngouriMath.Extensions;
using API_PCC.ApplicationModels;
using API_PCC.Data;
using API_PCC.EntityModels;
using API_PCC.Models;
using API_PCC.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using System.Text.RegularExpressions;

namespace API_PCC.Controllers
{
    //[Authorize("ApiKey")]
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
            return Ok(_bloodCalculator.compute(bloodCalculatorModel));
        }
    }
}
