using Application.Validations;

namespace Application.DTOs.Request.User
{
    public class UpdateUserReqDto
    {
        [ValidateUsername]
        public string Username { get; set; } = string.Empty;

        [ValidateUserEmail]
        public string Email { get; set; } = string.Empty;

        [ValidateKeyIdTypes]
        public string RoleKeyId { get; set; } = string.Empty;

        [ValidateKeyIdTypes(isRequired: false)]
        public string? ReportsToUserKeyId { get; set; } = string.Empty;
    }
}
