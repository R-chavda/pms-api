namespace Application.DTOs.Response.Project
{
    public class ProjectResponse
    {
        public string KeyId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public List<ProjectMemberResponse> ProjectMembers { get; set; } = [];
    }
}
