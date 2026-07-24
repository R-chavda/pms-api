using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Request.Organization
{
    public class UpdateOrganizationThemeReqDto
    {
        [Required]
        public bool DarkTheme { get; set; }
        [Required]
        public string MenuMode { get; set; } = string.Empty;
        [Required]
        public string Preset { get; set; } = string.Empty;
        [Required]
        public string Primary { get; set; } = string.Empty;
        [Required]
        public string Surface { get; set; } = string.Empty;
    }
}
