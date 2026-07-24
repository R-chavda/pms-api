using Domain.Enums;
using Domain.Interfaces;

namespace Domain.Entities
{
    public class AppUserRole : IBaseEntity, ISoftDeletable
    {
        public int Id { get; set; }
        public long KeyId { get; set; }
        public bool IsDeleted { get; set; }
        public UserRole Name { get; set; }
    }
}
