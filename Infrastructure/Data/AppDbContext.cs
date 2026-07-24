using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data
{
    public class AppDbContext(DbContextOptions options, IUserContext userContext) : DbContext(options)
    {
        public DbSet<AppUserRole> Roles { get; set; }
        public DbSet<Organization> Organizations { get; set; }
        public DbSet<AppUser> Users { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<TaskItem> TaskItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

            modelBuilder.ConfigureIsDeletedFilter();

            var orgId = userContext.OrganizationId;
            // modelBuilder.Entity<AppUser>().HasQueryFilter(u => u.OrganizationId == orgId);
            // modelBuilder.Entity<Project>().HasQueryFilter(p => p.OrganizationId == orgId);
            // modelBuilder.ConfigureTenantFilter(userContext);
        }
    }
}
