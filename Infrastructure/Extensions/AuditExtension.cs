using Domain.Interfaces;

namespace Infrastructure.Extensions
{
    public static class AuditExtension
    {
        public enum AuditAction
        {
            Create,
            Update,
            Delete
        }

        public static void ApplyFullAudit(this object entity,AuditAction auditAction,int userId)
        {
            if(entity is IAuditEntity auditEntity)
            {
                switch (auditAction)
                {
                    case AuditAction.Create:
                        auditEntity.CreatedAt = DateTime.UtcNow;
                        auditEntity.CreatedBy = userId;
                        break;

                    case AuditAction.Update:
                    case AuditAction.Delete:
                        auditEntity.UpdatedAt = DateTime.UtcNow;
                        auditEntity.UpdatedBy = userId;
                        break;
                }
            }
        }
    }
}
