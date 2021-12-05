using Backend.Data;
using Backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApplication4.Controllers
{
    /// <summary>
    /// WeatherForecastController test
    /// </summary>
    [ApiVersion("1.0")]
    [ApiController]
    [Authorize(Policy = PolicyRole.EDITOR)]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly ILogger<WeatherForecastController> log;
        private readonly WeatherForecastService _weatherForecastService;
        private readonly Auth0Service _auth0Service;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, WeatherForecastService _weatherForecastService, Auth0Service _auth0Service)
        {
            this.log = logger;
            this._weatherForecastService = _weatherForecastService;
            this._auth0Service = _auth0Service;
        }

        /// <summary>
        /// Test
        /// </summary>
        /// <returns></returns>
        [HttpGet(Name = "GetWeatherForecast")]
        public async Task<IEnumerable<WeatherForecast>> Get()
        {
            log.LogInformation("Get");
           
            try
            {
                var user = await _auth0Service.UpdateUserAppMetaDataAsync(User.Identity.Name, new { date = DateTime.Now });
            }
            catch (Exception ex) {
                log.LogInformation("Exception", ex);
            }
            
            return await _weatherForecastService.GetForecastAsync(DateTime.Now);
        }
    }
}