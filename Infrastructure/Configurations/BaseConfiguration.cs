using Domain.Entities;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations
{
    public static class BaseConfiguration
    {
        public static void ApplyBaseConfiguration<TEntity>(this EntityTypeBuilder<TEntity> builder)
            where TEntity : class
        {
            var entityType = typeof(TEntity);

            if (typeof(IBaseEntity).IsAssignableFrom(entityType))
            {
                builder.HasKey("Id");
                builder.Property("KeyId").ValueGeneratedNever();
            }

            if (typeof(ISoftDeletable).IsAssignableFrom(entityType))
            {
                builder.Property("IsDeleted").HasDefaultValue(false);
            }

            if (typeof(IAuditEntity).IsAssignableFrom(entityType))
            {
                builder.Property("CreatedAt").IsRequired();

                builder.HasOne(typeof(AppUser), "CreatedByUser")
                       .WithMany()
                       .HasForeignKey("CreatedBy")
                       .OnDelete(DeleteBehavior.Restrict);

                builder.HasOne(typeof(AppUser), "UpdatedByUser")
                       .WithMany()
                       .HasForeignKey("UpdatedBy")
                       .OnDelete(DeleteBehavior.Restrict);
            }
        }
    }
}
