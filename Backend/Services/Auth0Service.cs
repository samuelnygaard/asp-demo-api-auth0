using Auth0.ManagementApi;
using Auth0.ManagementApi.Models;

namespace Backend.Services
{
    public class Auth0Service
    {
        private readonly string domain;
        private readonly string clientId;
        private readonly string clientSecret;
        private readonly ManagementApiClient client;

        public Auth0Service(IConfiguration configuration)
        {
            domain = configuration["Auth0:Domain"];
            clientId = configuration["Auth0:ClientId"];
            clientSecret = configuration["Auth0:ClientSecret"];
            client = new ManagementApiClient(configuration["Auth0:ManagementToken"], domain);
        }

        public Task<User> GetUserAsync(string? userId)
        {
            if (string.IsNullOrEmpty(userId)) throw new ArgumentNullException("userId");
            return client.Users.GetAsync(userId);
        }

        public Task<User> UpdateUserAppMetaDataAsync(string? userId, dynamic appMetaData)
        {
            if (string.IsNullOrEmpty(userId)) throw new ArgumentNullException("userId");

            return client.Users.UpdateAsync(userId, new UserUpdateRequest()
            {
                AppMetadata = appMetaData
            });
        }

        private async Task<Role> GetRoleByName(string roleName)
        {
            var allRoles = await client.Roles.GetAllAsync(new GetRolesRequest());

            var rolesDict = allRoles.ToDictionary(kv => kv.Name);

            if (!rolesDict.ContainsKey(roleName)) throw new KeyNotFoundException(roleName);

            return rolesDict[roleName];
        }

        public async Task AddUserRoleAsync(string? userId, string roleName)
        {
            if (string.IsNullOrEmpty(userId)) throw new ArgumentNullException("userId");

            var role = await GetRoleByName(roleName);

            string[] roles = { role.Id };

            await client.Users.AssignRolesAsync(userId, new AssignRolesRequest()
            {
                Roles = roles
            });
        }

        public async Task RemveUserRoleAsync(string? userId, string roleName)
        {
            if (string.IsNullOrEmpty(userId)) throw new ArgumentNullException("userId");

            var role = await GetRoleByName(roleName);

            string[] roles = { role.Id };

            await client.Users.RemoveRolesAsync(userId, new AssignRolesRequest()
            {
                Roles = roles
            });
        }
    }
}
