using Domain.Interfaces;

namespace Domain.Entities
{
    public class Organization : IBaseEntity, ISoftDeletable, IAuditEntity
    {
        public int Id { get; set; }
        public long KeyId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Domain { get; set; } = string.Empty;
        public string? Theme { get; set; } = string.Empty;
        public string? Logo { get; set; } = string.Empty;
        public bool IsDeleted { get; set; }
        public DateTime CreatedAt { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int? UpdatedBy { get; set; }
        public AppUser? CreatedByUser { get; set; }
        public AppUser? UpdatedByUser { get; set; }
        public List<AppUser> OrganizationMembers { get; set; } = [];
        public List<Project> OrganizationProjects { get; set; } = [];
    }
}
