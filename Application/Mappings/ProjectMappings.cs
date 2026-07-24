using Application.DTOs.Response.Project;
using Domain.Entities;

namespace Application.Mappings
{
    public static class ProjectMappings
    {
        public static ProjectMemberResponse ToProjectMember(this AppUser appUser)
        {
            return new ProjectMemberResponse
            {
                KeyId = appUser.KeyId.ToString(),
                Email = appUser.Email,
                Username = appUser.Username,
                Role = appUser.Role!.Name.ToString(),
            };
        }
    }
}
