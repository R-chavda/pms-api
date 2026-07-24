using Application.DTOs.Request.User;
using Application.DTOs.Response.User;
using Application.Services;
using Domain.Constants;
using Domain.Enums;
using Domain.Wrappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Host.Controllers
{
    [ApiController]
    [Route("api/v1/users")]
    public class UserController(IUserService userService) : ControllerBase
    {
        [Authorize(Roles =$"{UserRoles.Admin}")]
        [HttpPost]
        public async Task<ApiResponse<object>> Create(CreateUserDto createUserDto)
        {
            return await userService.CreateUserAsync(createUserDto);
        }

        [Authorize(Roles = $"{UserRoles.Admin}")]
        [HttpGet]
        public async Task<ApiResponse<List<UserResponse>>> GetAll()
        {
            return await userService.GetUsers();
        }

        [Authorize]
        [HttpGet("{keyId}")]
        public async Task<ApiResponse<UserResponse>> Get(long keyId)
        {
            return await userService.GetUserByKeyId(keyId);
        }

        [Authorize(Roles = $"{UserRoles.Admin}")]
        [HttpDelete("{keyId}")]
        public async Task<ApiResponse<object>> Delete(long keyId)
        {
            return await userService.DeleteUserAsync(keyId);
        }

        [Authorize(Roles = $"{UserRoles.Admin}")]
        [HttpPut("{keyId}")]
        public async Task<ApiResponse<object>> Update(long keyId,UpdateUserReqDto updateUserReq)
        {
            return await userService.UpdateUserAsync(keyId,updateUserReq);
        }

        [Authorize(Roles = $"{UserRoles.Admin}")]
        [HttpGet("user-creation-metadata")]
        public async Task<ApiResponse<UserCreationMetadata>> GetMetadata()
        {
            return await userService.GetMetadata();
        }

        [Authorize(Roles = $"{UserRoles.Manager}")]
        [HttpGet("direct-reports")]
        public async Task<ApiResponse<List<UserResponseMinimal>>> GetDirectReports()
        {
            return await userService.GetDirectReports();
        }
    }
}
