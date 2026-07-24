using Application.DTOs.Response.User;
using Domain.Entities;

namespace Application.DTOs.Response.Organization
{
    public class OrganizationResponse
    {
        public string KeyId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Domain { get; set; } = string.Empty;
        public List<UserResponseMinimal>? OrganizationMembers { get; set; } = [];
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
