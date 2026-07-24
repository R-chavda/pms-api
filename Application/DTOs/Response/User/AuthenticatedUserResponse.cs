using Application.DTOs.Response.Organization;

namespace Application.DTOs.Response.User
{
    public class AuthenticatedUserResponse
    {
        public string KeyId { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public RoleResponse Role { get; set; } = null!;
        public string AccessToken { get; set; } = string.Empty;
        public OrganizationResponseMinimal Organization { get; set; } = null!;
    }
}
