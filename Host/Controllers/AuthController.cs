using Application.DTOs.Request.User;
using Application.DTOs.Response.User;
using Application.Services;
using Domain.Wrappers;
using Microsoft.AspNetCore.Mvc;

namespace Host.Controllers
{
    [ApiController]
    [Route("api/v1/auth")]
    public class AuthController(IAuthService authService) : ControllerBase
    {
        [HttpPost("login")]
        public async Task<ApiResponse<AuthenticatedUserResponse>> Login(LoginUserReqDto loginUserReqDto)
        {
            return await authService.LoginUser(loginUserReqDto);
        }
    }
}
