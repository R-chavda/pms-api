using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Request.Organization
{
    public class CreateOrganizationReqDto
    {
        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        public string Domain { get; set; } = string.Empty;

        [Required]
        public AdminCredentials AdminCredentials { get; set; } = null!;
    }   
}
