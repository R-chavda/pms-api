using System.ComponentModel.DataAnnotations;
namespace Application.DTOs.Request
{
    public class UpdateProjectReqDto
    {
        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        public string Description { get; set; } = string.Empty;
    }
}
