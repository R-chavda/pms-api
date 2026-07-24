namespace Application.DTOs.Response.Project
{
    public class AnalyticsResponse
    {
        public int ProjectsCount { get; set; }
        public int ResourcesCount { get; set; }
        public object TaskStatusPerProject { get; set; } = null!;
    }
}
