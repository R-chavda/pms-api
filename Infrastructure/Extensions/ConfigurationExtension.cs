using Domain.Entities;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Infrastructure.Extensions
{
    public static class ConfigurationExtension
    {
        public static void ConfigureIsDeletedFilter(this ModelBuilder modelBuilder)
        {
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                if (typeof(ISoftDeletable).IsAssignableFrom(entityType.ClrType))
                {
                    var parameter = Expression.Parameter(entityType.ClrType, "e");
                    var property = Expression.Property(parameter, nameof(ISoftDeletable.IsDeleted));
                    var filter = Expression.Lambda(
                        Expression.Equal(property, Expression.Constant(false)),
                        parameter);

                    modelBuilder.Entity(entityType.ClrType).HasQueryFilter(filter);
                }
            }
        }

        public static void ConfigureTenantFilter(this ModelBuilder modelBuilder, IUserContext userContext)
        {
            modelBuilder.Entity<AppUser>().HasQueryFilter(u => u.OrganizationId == userContext.OrganizationId);
            modelBuilder.Entity<Project>().HasQueryFilter(p => p.OrganizationId == userContext.OrganizationId);
        }
    }
}
