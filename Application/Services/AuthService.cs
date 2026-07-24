using Abstractions;
using Application.DTOs.Request.User;
using Application.DTOs.Response.Organization;
using Application.DTOs.Response.User;
using Domain.Constants;
using Domain.Entities;
using Domain.Enums;
using Domain.Wrappers;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace Application.Services
{
    public interface IAuthService
    {
        Task<ApiResponse<AuthenticatedUserResponse>> LoginUser(LoginUserReqDto loginUserReqDto);
    }

    public class AuthService(
        IGenericRepository<AppUser> userRepo,
        IHashingService hashingService,
        ITokenService tokenService
        ) : IAuthService
    {
        public async Task<ApiResponse<AuthenticatedUserResponse>> LoginUser(LoginUserReqDto loginUserReqDto)
        {
            var userExist = await userRepo.GetSingleAsync(
                u => u.Email == loginUserReqDto.Email,
                u => u.Include(u => u.Role!).Include(u => u.Organization!),
                ignoreTenantFilter: true
            );
            if (userExist == null)
            {
                return ApiResponse<AuthenticatedUserResponse>.Fail(StatusCode.NotFound, $"User with email {loginUserReqDto.Email} does not exist.");
            }

            var passwordCheck = userExist.Id == SystemUser.UserId
                ? userExist.Password == loginUserReqDto.Password
                : hashingService.VerifyPassword(userExist.Password, loginUserReqDto.Password);
            if (!passwordCheck)
            {
                return ApiResponse<AuthenticatedUserResponse>.Fail(StatusCode.BadRequest, $"Invalid Credentials");
            }

            var token = tokenService.GenerateAccessToken(userExist);
            var response = new AuthenticatedUserResponse
            {
                KeyId = userExist.KeyId.ToString(),
                Username = userExist.Username,
                Email = userExist.Email,
                Role = userExist.Role.Adapt<RoleResponse>(),
                Organization = userExist.Organization.Adapt<OrganizationResponseMinimal>(),
                AccessToken = token,
            };
            return ApiResponse<AuthenticatedUserResponse>.Success(StatusCode.Success, response, "Login successful");
        }
    }
}
