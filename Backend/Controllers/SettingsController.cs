using Backend.Models;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers
{
    /// <summary>
    /// MovieController test
    /// </summary>
    [ApiVersion("1.0")]
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class SettingsController : Controller
    {
        private readonly IConfiguration Configuration;

        public SettingsController(IConfiguration configuration)
        {
            Configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        [HttpGet("auth")]
        public ActionResult<PublicAuthSettingsModel> GetPublicAuthSettings()
        {
            try
            {
                return new PublicAuthSettingsModel()
                {
                    Audience = Configuration["Auth0:Audience"],
                    Domain = Configuration["Auth0:Domain"],
                    ClientId = Configuration["Auth0:SPAClientId"]
                };
            }
            catch (Exception)
            {
                return new StatusCodeResult(500);
            }
        }
    }
}
