using API_PCC.Manager;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Data;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
namespace API_PCC
{
    internal class BasicAuthenticationHandler : AuthenticationHandler<BasicAuthenticationOptions>
    {
        DbManager db = new DbManager();
        private IOptionsMonitor<BasicAuthenticationOptions> _options;

        public BasicAuthenticationHandler(IOptionsMonitor<BasicAuthenticationOptions> options, ILoggerFactory log, UrlEncoder encoder, ISystemClock clock) : base(options, log, encoder, clock)
        {
            _options = options;
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            System.Console.WriteLine("Handler Calling");
            if (!AuthenticationHeaderValue.TryParse(Request.Headers["Authorization"], out AuthenticationHeaderValue headervalue))
            {

                return AuthenticateResult.NoResult();
            }
            var auth = Request.Headers["Authorization"].ToString();
            string sql = $@"SELECT        Id, ApiToken, Role, Name, Status
                            FROM            tbl_ApiTokenModel
                            WHERE        (ApiToken = '"+ auth + "' and Status =1)";
            DataTable dt = db.SelectDb(sql).Tables[0];
            foreach (DataRow dr in dt.Rows)
            {
                if(dt.Rows.Count != 0)
                {
                    var claims = new[] {new Claim (ClaimTypes.Name, dr["Name"].ToString()),
                             new Claim(ClaimTypes.Role ,  dr["Role"].ToString())
                };
                    var identity = new ClaimsIdentity(claims, Scheme.Name);
                    var principal = new ClaimsPrincipal(identity);
                    var ticket = new AuthenticationTicket(principal, Scheme.Name);
                    return AuthenticateResult.Success(ticket);
                }
            
            }
            return AuthenticateResult.NoResult();
          

        }
    }
}
