using System.Security.Claims;

namespace Backend.Handlers
{
    public class BaseHandler<T> where T : class
    {
        private readonly ILogger<T> log;
        private readonly Auth0Service _auth0Service;
        private readonly IAuthorizationService _authorizationService;

        public BaseHandler(ILogger<T> logger, Auth0Service _auth0Service, IAuthorizationService _authorizationService)
        {
            this.log = logger;
            this._auth0Service = _auth0Service;
            this._authorizationService = _authorizationService;
        }

        public async Task<bool> CheckRoleAsync(ClaimsPrincipal user, string role)
        {
            log.LogInformation("Test CheckRoleAsync");
            return (await _authorizationService.AuthorizeAsync(user, role)).Succeeded;
        }
    }
}
