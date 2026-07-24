using Domain.Entities;

namespace Application.DTOs.Response.User
{
    public class UserResponse
    {
        public string KeyId { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public RoleResponse Role { get; set; } = null!;
        public UserResponse? ReportsToUser { get; set; }
        
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? CreatedByUser { get; set; }
        public string? UpdatedByUser { get; set; }
    }
}
