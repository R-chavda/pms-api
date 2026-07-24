using Abstractions;
using Application.DTOs.Request.User;
using Application.DTOs.Response.User;
using Application.Helpers;
using Application.Mappings;
using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;
using Domain.Wrappers;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace Application.Services
{
    public interface IUserService
    {
        Task<ApiResponse<object>> CreateUserAsync(CreateUserDto createUserDto);
        Task<ApiResponse<object>> DeleteUserAsync(long userKeyId);
        Task<ApiResponse<List<UserResponse>>> GetUsers();
        Task<ApiResponse<UserResponse>> GetUserByKeyId(long userKeyId);
        Task<ApiResponse<UserCreationMetadata>> GetMetadata();
        Task<ApiResponse<object>> UpdateUserAsync(long keyId, UpdateUserReqDto updateUserReq);
        Task<ApiResponse<List<UserResponseMinimal>>> GetDirectReports();
    }

    public class UserService(
        IUserContext userContext,
        IGenericRepository<AppUser> userRepo,
        IGenericRepository<AppUserRole> roleRepo,
        IHashingService hashingService,
        IIdResolverService idResolverService) : IUserService
    {
        public async Task<ApiResponse<List<UserResponse>>> GetUsers()
        {
            var users = await userRepo.GetAllAsync(
                u => u.OrganizationId == userContext.OrganizationId,
                u => u.Include(u => u.Role!).Include(u => u.ReportsToUser!)
            );
            var parsedUsers = users.Select(u => u.ToResponse()).ToList();
            return ApiResponse<List<UserResponse>>.Success(StatusCode.Success, parsedUsers, "Users retrieved successfully");
        }

        public async Task<ApiResponse<object>> CreateUserAsync(CreateUserDto createUserDto)
        {
            if (await userRepo.ExistsAsync(u => u.Email == createUserDto.Email))
            {
                return ApiResponse<object>.Fail(StatusCode.BadRequest, $"User with email {createUserDto.Email} already exists.");
            }

            var user = createUserDto.Adapt<AppUser>();
            var role = await roleRepo.GetByKeyIdAsync(IdParser.ParseToLong(createUserDto.RoleKeyId));
            if (role == null)
            {
                return ApiResponse<object>.Fail(StatusCode.BadRequest, $"Role does not exist");
            }
            user.RoleId = role.Id;

            var reportingManagerId = await idResolverService.ResolveOptionalIdAsync<AppUser>(createUserDto.ReportsToUserKeyId);
            if (!UserHelper.IsHigherAuthority(role.Name) && reportingManagerId == null)
            {
                return ApiResponse<object>.Fail(StatusCode.BadRequest, $"Reporting Manager Id is required");
            }

            user.OrganizationId = userContext.OrganizationId;
            user.ReportsToUserId = reportingManagerId;
            user.Password = hashingService.HashPassword(user.Password);

            userRepo.Add(user);
            await userRepo.SaveChangesAsync();
            return ApiResponse<object>.Success(StatusCode.Created, default, "User created");
        }

        public async Task<ApiResponse<object>> DeleteUserAsync(long userKeyId)
        {
            var userExist = await userRepo.GetByKeyIdAsync(userKeyId);
            if (userExist == null)
            {
                return ApiResponse<object>.Fail(StatusCode.NotFound, $"User with KeyId {userKeyId} does not exist.");
            }

            userRepo.Remove(userExist);
            await userRepo.SaveChangesAsync();
            return ApiResponse<object>.Success(StatusCode.NoContent, default, "User deleted successfully");
        }

        public async Task<ApiResponse<UserResponse>> GetUserByKeyId(long userKeyId)
        {
            var user = await userRepo.GetSingleAsync(
                u => u.KeyId == userKeyId,
                u => u.Include(u => u.Role!).Include(u => u.ReportsToUser!)
            );
            if (user == null)
            {
                return ApiResponse<UserResponse>.Fail(StatusCode.NotFound, $"User with KeyId {userKeyId} does not exist.");
            }

            return ApiResponse<UserResponse>.Success(StatusCode.Success, user.ToResponse(), "User retrieved successfully");
        }

        public async Task<ApiResponse<List<RoleResponse>>> GetAllRoles()
        {
            var roles = await roleRepo.GetAllAsync();
            var parsedRoles = roles.Select(r => r.Adapt<RoleResponse>()).ToList();
            return ApiResponse<List<RoleResponse>>.Success(StatusCode.Success, parsedRoles, "Roles retrieved");
        }

        public async Task<ApiResponse<UserCreationMetadata>> GetMetadata()
        {
            var roles = await roleRepo.GetAllAsync();
            var parsedRoles = roles.Select(r => r.Adapt<RoleResponse>()).ToList();
            var users = await userRepo.GetAllAsync(
                u => u.Role!.Name == UserRole.Manager,
                u => u.Include(u => u.Role!).Include(u => u.ReportsToUser!)
            );
            var parsedUsers = users.Select(u => u.Adapt<UserResponseMinimal>()).ToList();

            var response = new UserCreationMetadata
            {
                Users = parsedUsers,
                Roles = parsedRoles,
            };

            return ApiResponse<UserCreationMetadata>.Success(StatusCode.Success, response);
        }

        public async Task<ApiResponse<object>> UpdateUserAsync(long keyId, UpdateUserReqDto updateUserReq)
        {
            var user = await userRepo.GetByKeyIdAsync(keyId, tracking: true);
            if (user == null)
            {
                return ApiResponse<object>.Fail(StatusCode.NotFound, $"User with KeyId {keyId} does not exist.");
            }

            user.Username = updateUserReq.Username;
            user.Email = updateUserReq.Email;

            var role = await roleRepo.GetByKeyIdAsync(IdParser.ParseToLong(updateUserReq.RoleKeyId));
            if (role == null)
            {
                return ApiResponse<object>.Fail(StatusCode.BadRequest, $"Role does not exist");
            }
            user.RoleId = role.Id;

            var reportingManagerId = await idResolverService.ResolveOptionalIdAsync<AppUser>(updateUserReq.ReportsToUserKeyId);
            if (!UserHelper.IsHigherAuthority(role.Name) && reportingManagerId == null)
            {
                return ApiResponse<object>.Fail(StatusCode.BadRequest, $"Reporting Manager Id is required");
            }
            user.ReportsToUserId = reportingManagerId;
            userRepo.Update(user);
            await userRepo.SaveChangesAsync();

            return ApiResponse<object>.Success(StatusCode.NoContent, default, "User updated successfully");
        }

        public async Task<ApiResponse<List<UserResponseMinimal>>> GetDirectReports()
        {
            var users = await userRepo.GetAllAsync(
                u => u.ReportsToUserId == userContext.UserId
            );
            var parsedUsers = users.Select(u => u.Adapt<UserResponseMinimal>()).ToList();
            return ApiResponse<List<UserResponseMinimal>>.Success(StatusCode.Success, parsedUsers, "Direct reports retrieved successfully");
        }
    }
}
