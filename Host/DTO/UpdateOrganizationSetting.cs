using System.ComponentModel.DataAnnotations;

namespace Host.DTO
{
    public class UpdateOrganizationSettingReqDto
    {
        [Required]
        public string Name { get; set; } = string.Empty;
        public IFormFile? Logo { get; set; } = null;
    }
}
