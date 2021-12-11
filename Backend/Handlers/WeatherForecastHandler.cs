
namespace Backend.Handlers
{
    public class WeatherForecastHandler : BaseHandler<WeatherForecastHandler>
    {
        private readonly WeatherForecastService weatherForecastService;
        private readonly ILogger<WeatherForecastHandler> log;

        public WeatherForecastHandler(
            WeatherForecastService weatherForecastService,
            ILogger<WeatherForecastHandler> logger,
            Auth0Service _auth0Service,
            IAuthorizationService _authorizationService) :
            base(logger, _auth0Service, _authorizationService)
        {
            this.log = logger;
            this.weatherForecastService = weatherForecastService;
        }

        public async Task<WeatherForecast[]> GetForecastAsync(ClaimsPrincipal user)
        {
            if (await CheckRoleAsync(user, PolicyRole.EDITOR))
            {
                log.LogError("Error");
                throw new Exception("Not valid");
            }
            log.LogInformation("Info");
            return await weatherForecastService.GetForecastAsync(DateTime.Now);
        }
    }
}
