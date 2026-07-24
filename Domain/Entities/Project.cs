using Domain.Interfaces;

namespace Domain.Entities
{
    public class Project : IBaseEntity, ITenantEntity, ISoftDeletable, IAuditEntity
    {
        public int Id { get; set; }
        public long KeyId { get; set; }
        public bool IsDeleted { get; set; }

        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        public List<TaskItem>? Tasks { get; set; }
        public List<AppUser>? ProjectMembers { get; set; }

        public int OrganizationId { get; set; }
        public Organization? Organization { get; set; }

        public DateTime CreatedAt { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int? UpdatedBy { get; set; }
        public AppUser? CreatedByUser { get; set; }
        public AppUser? UpdatedByUser { get; set; }
    }
}
