using API_PCC.Data;
using API_PCC.Models;
using API_PCC.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using static API_PCC.Controllers.UserController;

namespace API_PCC.Controllers
{
    [Authorize("ApiKey")]
    [Route("[controller]/[action]")]
    [ApiController]

    public class OTPController : ControllerBase
    {
        MailSender _mailSender;
        private readonly PCC_DEVContext _context;
        private readonly EmailSettings _emailsettings;
        public OTPController(PCC_DEVContext context, IOptions<EmailSettings> emailsettings)
        {
            _context = context;
            _emailsettings = emailsettings.Value;
        }

        [HttpGet]
        public async Task<IActionResult> OTPList()
        {//
            var list = _context.TblRegistrationOtpmodels.ToList();
            return Ok(list);
        }
      
        [HttpPost]
        public async Task<IActionResult> OTPFilterbyEmail(TblRegistrationOtpmodel data)
        {
            var list = _context.TblRegistrationOtpmodels.Where(a=>a.Email == data.Email).ToList();
            return Ok(list);
        }
        [HttpPost]
        public async Task<IActionResult> SendOTP(TblRegistrationOtpmodel data)
        {
            if (_context.TblRegistrationOtpmodels == null)
            {
                return Problem("Entity set 'PCC_DEVContext.OTP' is null!");
            }

            try
            {
                var registrationOtpModel = _context.TblRegistrationOtpmodels.Where(otpModel => otpModel.Otp == data.Otp && otpModel.Email == data.Email && otpModel.Status == 10).FirstOrDefault();
                if (registrationOtpModel != null)
                {
                    var model = new TblRegistrationOtpmodel()
                    {
                        Email = data.Email,
                        Otp = data.Otp,
                        Status = 10,

                    };
                    _context.TblRegistrationOtpmodels.Add(model);
                    _context.SaveChanges();

                    MailSender email = new MailSender(_emailsettings);
                    email.sendOtpMail(data);
                    return Ok("OTP sent successfully!!");
                }
                else
                {
                    return BadRequest("Record not found on database!");
                }
            }
            catch (Exception ex)
            {
                string exception = ex.GetBaseException().ToString();
                return Problem(exception);
            }
        }

        [HttpPost]
        public async Task<IActionResult> VerifyOTP(TblRegistrationOtpmodel data)
        {
            try
            {
                if (_context.TblRegistrationOtpmodels == null)
                {
                    return Problem("Entity set 'PCC_DEVContext.OTP' is null!");
                }
                var registOtpModels = _context.TblRegistrationOtpmodels.Where(otpModel => otpModel.Email == data.Email && otpModel.Status == 4  ).FirstOrDefault();

                if (registOtpModels != null)
                {
                    if (registOtpModels.Otp == data.Otp)
                    {
                        registOtpModels.Status = 3;
                        _context.Entry(registOtpModels).State = EntityState.Modified;

                        var userModel = _context.TblUsersModels.Where(user => user.Email == data.Email ).FirstOrDefault();
                        _context.Entry(userModel).State = EntityState.Modified;
                        userModel.Status = 3;

                        await _context.SaveChangesAsync();
                        return Ok("OTP verification successful!");
                    } else
                    {
                        var userModel = _context.TblUsersModels.Where(user => user.Email == data.Email).FirstOrDefault();
                        _context.Entry(userModel).State = EntityState.Modified;
                        userModel.Status = 4;
                        return BadRequest("OTP verification unsuccessful!");
                    }
                    
                }
                else
                {
                    return BadRequest("Record not found on database!");
                }
            }

            catch (Exception ex)
            {
                String exception = ex.GetBaseException().ToString();
                return Problem(exception);
            }
        }


        [HttpPost]
        public async Task<IActionResult> ResendOTP(TblRegistrationOtpmodel data)
        {
            try
            {
                if (_context.TblRegistrationOtpmodels == null)
                {
                    return Problem("Entity set 'PCC_DEVContext.OTP' is null!");
                }

                var registrationOtpModel = _context.TblRegistrationOtpmodels.Where(otpModel =>  otpModel.Email == data.Email && otpModel.Status == 4).FirstOrDefault();
                if (registrationOtpModel != null)
                {
                    TblRegistrationOtpmodel item = new TblRegistrationOtpmodel();
                    item.Email = registrationOtpModel.Email;
                    item.Otp = registrationOtpModel.Otp;

                    MailSender email = new MailSender(_emailsettings);
                    email.sendOtpMail(item);
                    return Ok("Resending OTP successful!");
                }
                else
                {
                    return BadRequest("Record not found on database!");
                }
            }
            catch (Exception ex)
            {
                String exception = ex.GetBaseException().ToString();
                return Problem(exception);
            }
        }

    }
}
