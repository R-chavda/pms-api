using Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Request.Task
{
    public class CreateTaskReqDto
    {
        [Required]
        public string Summary { get; set; } = string.Empty;

        [Required]
        public string Description { get; set; } = string.Empty;

        [Required]
        public string ProjectKeyId { get; set; } = string.Empty;

        public string? AssignedToUserKeyId { get; set; } = string.Empty;

        [Required]
        public TaskPriority Priority { get; set; }
    }
}
