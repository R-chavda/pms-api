using Domain.Enums;

namespace Application.DTOs.Request.Task
{
    public class UpdateTaskReqDto
    {
        public string Summary { get; set; } = string.Empty;
        public string? Description { get; set; } = string.Empty;
        public TaskPriority Priority { get; set; }
    }
}
