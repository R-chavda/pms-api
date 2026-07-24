using Application.DTOs.Response.User;

namespace Application.DTOs.Response.Task
{
    public class TaskResponse
    {
        public string KeyId { get; set; } = string.Empty;
        public string Summary { get; set; } = string.Empty;
        public string? Description { get; set; } = string.Empty;
        
        public string Status { get; set; } = string.Empty;
        public string Priority { get; set; } = string.Empty;
        public UserResponseMinimal AssignedToUser { get; set; } = null!;
        public string CreatedBy { get; set; } = string.Empty;
        public string? UpdatedBy { get; set; } = string.Empty;
        
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        
    }
}
