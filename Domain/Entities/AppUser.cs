using Domain.Interfaces;

namespace Domain.Entities
{
    public class AppUser : IBaseEntity, ITenantEntity, ISoftDeletable, IAuditEntity
    {
        public int Id { get; set; }
        public long KeyId { get; set; }
        public bool IsDeleted { get; set; }

        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;

        public int RoleId { get; set; }
        public AppUserRole? Role { get; set; }

        public int? ReportsToUserId { get; set; }
        public AppUser? ReportsToUser { get; set; }
        public List<Project>? Projects { get; set; }

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
