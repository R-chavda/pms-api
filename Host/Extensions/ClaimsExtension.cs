using System.Security.Claims;

namespace Host.Extensions
{
    public static class ClaimsExtension
    {
        public static UserContext GetUserInfo(this ClaimsPrincipal User)
        {
            return new UserContext
            {
                IsAuthenticated = User.Identity!.IsAuthenticated,
                UserKeyId = long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!),
                OrganizationId = int.Parse(User.FindFirstValue(ClaimTypes.GroupSid)!),
                Email = User.FindFirstValue(ClaimTypes.Email)!,
                Role = User.FindFirstValue(ClaimTypes.Role)!,
            };
        }
    }
}
