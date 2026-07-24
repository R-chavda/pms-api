using Domain.Enums;
using Domain.Interfaces;

namespace Domain.Entities
{
    public class TaskItem : IBaseEntity, ISoftDeletable, IAuditEntity
    {
        public int Id { get; set; }
        public long KeyId { get; set; }
        public bool IsDeleted { get; set; }

        public string Summary { get; set; } = string.Empty;
        public string? Description { get; set; } = string.Empty;
        public TaskPriority Priority { get; set; }
        public Enums.TaskStatus Status { get; set; }
        
        public int AssignedToUserId { get; set; }
        public AppUser? AssignedToUser { get; set; }

        public int ProjectId { get; set; }
        public Project? Project { get; set; }

        public DateTime CreatedAt { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int? UpdatedBy { get; set; }
        public AppUser? CreatedByUser { get; set; }
        public AppUser? UpdatedByUser { get; set; }
    }
}
