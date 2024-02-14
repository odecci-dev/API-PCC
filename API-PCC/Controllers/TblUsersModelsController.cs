using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using API_PCC.Data;
using API_PCC.Models;
using API_PCC.Manager;
using System.Data;
using System.Text;

namespace API_PCC.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TblUsersModelsController : ControllerBase
    {
        private readonly PCC_DEVContext _context;
        DbManager db = new DbManager();
        public TblUsersModelsController(PCC_DEVContext context)
        {
            _context = context;
        }

        // GET: api/TblUsersModels
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TblUsersModel>>> GetTblUsersModels()
        {
          if (_context.TblUsersModels == null)
          {
              return NotFound();
          }
            return await _context.TblUsersModels.ToListAsync();
        }

        // GET: api/TblUsersModels/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TblUsersModel>> GetTblUsersModel(int id)
        {
          if (_context.TblUsersModels == null)
          {
              return NotFound();
          }
            var tblUsersModel = await _context.TblUsersModels.FindAsync(id);

            if (tblUsersModel == null)
            {
                return NotFound();
            }

            return tblUsersModel;
        }

        // PUT: api/TblUsersModels/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTblUsersModel(int id, TblUsersModel tblUsersModel)
        {
            if (id != tblUsersModel.Id)
            {
                return BadRequest();
            }

            _context.Entry(tblUsersModel).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TblUsersModelExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/TblUsersModels
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<TblUsersModel>> PostTblUsersModel(TblUsersModel tblUsersModel)
        {
          if (_context.TblUsersModels == null)
          {
              return Problem("Entity set 'PCC_DEVContext.TblUsersModels'  is null.");
          }
            _context.TblUsersModels.Add(tblUsersModel);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetTblUsersModel", new { id = tblUsersModel.Id }, tblUsersModel);
        }

        // DELETE: api/TblUsersModels/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTblUsersModel(int id)
        {
            if (_context.TblUsersModels == null)
            {
                return NotFound();
            }
            var tblUsersModel = await _context.TblUsersModels.FindAsync(id);
            if (tblUsersModel == null)
            {
                return NotFound();
            }

            _context.TblUsersModels.Remove(tblUsersModel);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool TblUsersModelExists(int id)
        {
            return (_context.TblUsersModels?.Any(e => e.Id == id)).GetValueOrDefault();
        }
        public class LogInModel
        {
            public string? Username { get; set; }
            public string? Password { get; set; }

        }
        public class AppModel
        {
            public string? Key { get; set; }
            public string? Status { get; set; }

        }
        [HttpPost]

        public IActionResult LogIn(LogInModel data)
        {
            var pass3 = Cryptography.Decrypt("KOECkOzDU7+PCgWECK4nMGj5Oy0LOyqcEdO1ek1Jiz8=");
            string currentDate = DateTime.Now.ToString("yyyy-MM-dd");
            //_global.Status = gv.ValidationUser(data.Username, data.Password, _context);
            bool compr_user = false;
            //var userinfo = dbContext.tbl_UsersModel.Where(a => EF.Functions.Collate(a.Username, "Latin1_General_CI_AI") == username).ToList();
            if(data.Username.Length != 0 || data.Password.Length != 0)
            {
                string sql = $@"SELECT [Id],[Username]
                        ,[Password]
                         FROM [dbo].[tbl_UsersModel]
                         where [Username]  ='" + data.Username + "' and Password='" + Cryptography.Encrypt(data.Password) + "' and Active = ";
                DataTable dt = db.SelectDb(sql).Tables[0];
                var item = new AppModel();
                if (dt.Rows.Count != 0)
                {
                    compr_user = String.Equals(dt.Rows[0]["Username"].ToString().Trim(), data.Username, StringComparison.Ordinal);
                    if(compr_user)
                    {
                        string pass = Cryptography.Decrypt(dt.Rows[0]["Password"].ToString().Trim());
                        if (pass.Equals(data.Password.Trim()))
                        {
                            StringBuilder str_build = new StringBuilder();
                            Random random = new Random();
                            int length = 8;
                            char letter;

                            for (int i = 0; i < length; i++)
                            {
                                double flt = random.NextDouble();
                                int shift = Convert.ToInt32(Math.Floor(25 * flt));
                                letter = Convert.ToChar(shift + 2);
                                str_build.Append(letter);
                            }
                            //gv.AudittrailLogIn("Successfully", "Log In Form", dt.Rows[0]["EmployeeID"].ToString(), 7);
                            var token = Cryptography.Encrypt(str_build.ToString());
                            string strtokenresult = token;
                            string[] charsToRemove = new string[] { "/", ",", ".", ";", "'", "=", "+" };
                            foreach (var c in charsToRemove)
                            {
                                strtokenresult = strtokenresult.Replace(c, string.Empty);
                            }
                            string query = $@"update tbl_UsersModel set JWToken='" + string.Concat(strtokenresult.TakeLast(15)) + "' where Id = '" + dt.Rows[0]["id"].ToString() + "'";
                            db.AUIDB_WithParam(query);
                            item.Status = "Successfully Log In";
                            item.Key = string.Concat(strtokenresult.TakeLast(15));
                            return Ok(item);

                        }
                        else
                        {
                            string get_loginAttempts = $@"select Attempts from tbl_UsersModel where Username = '"+data.Username+"'";
                            DataTable tbl_attempts_count = db.SelectDb(get_loginAttempts).Tables[0];

                            int attempts = int.Parse(tbl_attempts_count.Rows[0]["Attempts"].ToString());

                            if(attempts < 5)
                            {
                                item.Status = "You reached the log in attempts please contact your administrator.";
                                item.Key = "";
                                return Ok(item);
                            }
                            else
                            {
                                string loginattempts = $@"UPDATE [dbo].[tbl_UsersModel]
                                  SET Attempts = '"+attempts + 1+ "' WHERE  Username = '" + data.Username + "'";
                                db.AUIDB_WithParam(loginattempts);
                                item.Status = "Invalid Log In";
                                item.Key = "";
                            }
                           
                            return Ok(item);
                        }
                    }
                    return Ok(item);
                }
                else
                {
                    return Ok(item);
                }
            }
            else
            {
                return BadRequest("Invalid Log In");
            }

            

        }
    }
}
