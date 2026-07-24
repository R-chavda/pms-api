using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Request.Organization
{
    public class UpdateOrganizationReqDto
    {
        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        public string Domain { get; set; } = string.Empty;
    }
}
