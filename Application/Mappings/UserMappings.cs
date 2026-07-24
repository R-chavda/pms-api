using Application.DTOs.Response.User;
using Domain.Entities;
using Mapster;

namespace Application.Mappings
{
    public static class UserMappings
    {
        public static UserResponse ToResponse(this AppUser user)
        {
            return new UserResponse
            {
                KeyId = user.KeyId.ToString(),
                Username = user.Username,
                Email = user.Email,
                Role = user.Role.Adapt<RoleResponse>(),
                ReportsToUser = user.ReportsToUser.Adapt<UserResponse>(),
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt,
                CreatedByUser = user.CreatedByUser?.Username,
                UpdatedByUser = user.UpdatedByUser?.Username,
            };
        }
    }
}
