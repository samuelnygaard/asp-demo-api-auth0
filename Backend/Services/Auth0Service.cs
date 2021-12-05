using System.Text.Json;
using Auth0.ManagementApi;
using Auth0.ManagementApi.Models;
using Backend.Data;

namespace Backend.Services
{
    public class Auth0Service
    {
        private readonly ManagementApiClient client;

        public Auth0Service(IConfiguration configuration)
        {
            client = new ManagementApiClient(configuration["Auth0:ManagementToken"], configuration["Auth0:Domain"]);
        }

        public Task<User> GetUserAsync(string? userId)
        {
            if (String.IsNullOrEmpty(userId)) throw new ArgumentNullException("userId");
            return client.Users.GetAsync(userId);
        }

        public Task<User> UpdateUserAppMetaDataAsync(string? userId, dynamic appMetaData)
        {
            if (String.IsNullOrEmpty(userId)) throw new ArgumentNullException("userId");

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
            if (String.IsNullOrEmpty(userId)) throw new ArgumentNullException("userId");

            var role = await GetRoleByName(roleName);

            string[] roles = { role.Id };

            await client.Users.AssignRolesAsync(userId, new AssignRolesRequest()
            {
                Roles = roles
            });
        }

        public async Task RemveUserRoleAsync(string? userId, string roleName)
        {
            if (String.IsNullOrEmpty(userId)) throw new ArgumentNullException("userId");

            var role = await GetRoleByName(roleName);

            string[] roles = { role.Id };

            await client.Users.RemoveRolesAsync(userId, new AssignRolesRequest()
            {
                Roles = roles
            });
        }
    }
}
