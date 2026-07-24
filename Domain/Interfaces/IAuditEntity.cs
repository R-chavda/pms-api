using Domain.Entities;

namespace Domain.Interfaces
{
    public interface IAuditEntity
    {
        DateTime CreatedAt { get; set; }
        int? CreatedBy { get; set; }
        DateTime? UpdatedAt { get; set; }
        int? UpdatedBy { get; set; }

        public AppUser? CreatedByUser { get; set; }
        public AppUser? UpdatedByUser { get; set; }
    }
}
