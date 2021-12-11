using System.Security.Claims;

namespace Backend
{
    public static class Util
    {
        public static string GetEmail(this ClaimsPrincipal user)
        {
            return user.FindFirstValue(ClaimTypes.Email);
        }

        public static string GetName(this ClaimsPrincipal user)
        {
            return user.FindFirstValue(ClaimTypes.Name);
        }

        public static string GetUserId(this ClaimsPrincipal user)
        {
            return user.FindFirstValue(ClaimTypes.NameIdentifier);
        }

        public static bool HasRole(this ClaimsPrincipal user, string role)
        {
            return user.Claims.Any(c => c.Type == "permissions" && c.Value == "perm:" + role);
        }
    }
}
