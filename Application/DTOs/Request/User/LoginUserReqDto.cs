using Application.Validations;

namespace Application.DTOs.Request.User
{
    public class LoginUserReqDto
    {
        [ValidateUserEmail]
        public string Email { get; set; } = string.Empty;

        [ValidateUserPassword]
        public string Password { get; set; } = string.Empty;    
    }
}
