namespace Backend.Controllers
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
        private readonly WeatherForecastHandler weatherForecastHandler;


        public WeatherForecastController(ILogger<WeatherForecastController> logger, WeatherForecastHandler weatherForecastHandler)
        {
            this.log = logger;
            this.weatherForecastHandler = weatherForecastHandler;
        }

        /// <summary>
        /// Test
        /// </summary>
        /// <returns></returns>
        [HttpGet(Name = "GetWeatherForecast")]
        public async Task<IEnumerable<WeatherForecast>> Get()
        {
            log.LogInformation("Get");

            return await weatherForecastHandler.GetForecastAsync(User);
        }
    }
}
