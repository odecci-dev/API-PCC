using API_PCC.Data;
using API_PCC.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Runtime.ConstrainedExecution;
using System.Text;
using static API_PCC.Controllers.UserController;
namespace API_PCC.Manager
{
    public class DBMethods
    {
        string sql = "";
        string Stats = "";
        string Mess = "";
        string JWT = "";
        DbManager db = new DbManager();
        private readonly PCC_DEVContext _context;
        #region Models

        public partial class RegistrationModel
        {
            public string Username { get; set; }

            public string Password { get; set; }

            public string Fname { get; set; }

            public string? Lname { get; set; }

            public string? Mname { get; set; }

            public string Email { get; set; }

            public string Gender { get; set; }

            public string? EmployeeId { get; set; }

            public string Jwtoken { get; set; }

            public string? FilePath { get; set; }

            public int? Active { get; set; }

            public string? Cno { get; set; }

            public string? Address { get; set; }

            public int? Status { get; set; }
            public string? CreatedBy { get; set; }

            public int? CenterId { get; set; }

            public bool? AgreementStatus { get; set; }
        }

        #endregion

        public StatusReturns GetUserLogIn(string username, string password, string? ipaddress, string? location)
        {
            var result = new List<TblUsersModel>();
            bool compr_user = false;
            if (username.Length != 0 || password.Length != 0)
            {
                var param = new IDataParameter[]
                {
                    new SqlParameter("@Username",username),
                    new SqlParameter("@Password",Cryptography.Encrypt(password))
                };
                DataTable dt = db.SelectDb_SP("GetUserLogIn", param).Tables[0];
                if (dt.Rows.Count != 0)
                {
                    string user_statId = dt.Rows[0]["StatusId"].ToString();
                    string user_activeId = dt.Rows[0]["ActiveStatusId"].ToString();
                    if (user_activeId == "1")
                    {
                        //active
                        switch (user_statId)
                        {
                            case "3":
                                //VERIFIED
                                Stats = "Error";
                                Mess = "Your account is under screening. Please contact admin to check the status.";
                                JWT = "";
                                break;
                            case "4":
                                //UNVERIFIED
                                Stats = "Error";
                                Mess = "Your account is unverified. Please contact your administrator.";
                                JWT = "";
                                break;
                            case "5":
                                //APPROVED
                           

                                compr_user = String.Equals(dt.Rows[0]["Username"].ToString().Trim(), username, StringComparison.Ordinal);

                                if (compr_user)
                                {
                                    string pass = Cryptography.Decrypt(dt.Rows[0]["password"].ToString().Trim());
                                    if ((pass).Equals(password))
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

                                        string query = $@"update tbl_UsersModel set JWToken='" + string.Concat(strtokenresult.TakeLast(15)) + "' where id = '" + dt.Rows[0]["id"].ToString() + "'";
                                        db.DB_WithParam(query);

                                        Stats = "Ok";
                                        Mess = "Successfully Log In";
                                        JWT = string.Concat(strtokenresult.TakeLast(15));
                                    }
                                    else
                                    {
                                        string sql = $@"select * from tbl_Attempts where UserId ='" + dt.Rows[0]["Id"].ToString() + "'";
                                        DataTable user_dt = db.SelectDb(sql).Tables[0];
                                        if (user_dt.Rows.Count != 0)
                                        {
                                            //update
                                            int attemp_count = int.Parse(user_dt.Rows[0]["AttemptCount"].ToString());
                                            if (attemp_count > 5)
                                            {
                                                string update_attempts = $@"update tbl_Attempts set AttemptCount ='" + attemp_count + 1 + "'  where id ='" + dt.Rows[0]["Id"].ToString() + "'";
                                                db.DB_WithParam(update_attempts);
                                            }
                                            else
                                            {
                                                Stats = "Error";
                                                Mess = "User LogIn Attempts Exceeded. Please contact admin";
                                                JWT = "";
                                            }


                                        }
                                        else
                                        {
                                            string OTPInsert = $@"INSERT INTO [dbo].[tbl_Attempts]
                                                               ([UserId]
                                                               ,[AttemptCount]
                                                               ,[IPAddress]
                                                               ,[Location])
                                                                VALUES
                                                               ('" + dt.Rows[0]["Id"].ToString() + "'" +
                                                               ",'1'," +
                                                               "'" + ipaddress + "'," +
                                                               "'" + location + "')";
                                            db.DB_WithParam(OTPInsert);
                                            Stats = "Error";
                                            Mess = "Invalid Log In";
                                            JWT = "";
                                            //insert

                                        }
                                        //update login attempts

                                    }
                                }
                                break;
                            case "6":
                                //REGISTERED
                                Stats = "Error";
                                Mess = "Your account is under screening or Your account has been disapproved . Please contact admin to check the status.";
                                JWT = "";

                                break;
                            default:
                                break;
                        }
                    }
                    else
                    {
                        //inactive
                        Stats = "Error";
                        Mess = "User is InActive. Please contact your administrator. ";
                        JWT = "";

                    }

                }
                else
                {
                    Stats = "Error";
                    Mess = "Invalid LogIn";
                    JWT = "";
                }


            }
            else
            {


            }
            StatusReturns results = new StatusReturns
            {
                Status = Stats,
                Message = Mess,
                JwtToken = JWT
            };
            return results;
        }



    }
}

