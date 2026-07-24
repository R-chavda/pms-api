using Abstractions;
using Domain.Constants;
using Domain.Entities;
using Domain.Enums;
using IdGen;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Infrastructure.Services
{
    public interface ISeederService
    {
        Task SeedData();
    }

    public class SeederService(AppDbContext context, IHashingService hashingService, IIdGenerator<long> idGenerator, ILogger<SeederService> logger) : ISeederService
    {
        private static readonly string ROLES_SEED_FILE_PATH = Path.Combine("..", "Infrastructure", "Seeders", "roles.json");
        private static readonly string USERS_SEED_FILE_PATH = Path.Combine("..", "Infrastructure", "Seeders", "users.json");
        private readonly JsonSerializerOptions jsonSerializerOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            WriteIndented = true,
            Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
        };
        public async Task SeedData()
        {
            try
            {
                await SeedSystemUser();
                await SeedRoles();
                await SeedUsers();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while seeding data.");
            }
        }

        private async Task SeedRoles()
        {
            if (context.Roles.IgnoreQueryFilters().Any(r => r.Id > 0))
            {
                logger.LogInformation("Roles already exists. Skipping role seeding.");
                return;
            }
            var json = File.ReadAllText(ROLES_SEED_FILE_PATH);
            var roles = JsonSerializer.Deserialize<List<AppUserRole>>(json, jsonSerializerOptions);

            if (roles == null)
            {
                logger.LogWarning("No roles found in the seed file.");
                return;
            }
            foreach (var role in roles)
            {
                role.KeyId = idGenerator.CreateId();
            }
            context.Roles.AddRange(roles);
            await context.SaveChangesAsync();
            logger.LogInformation("Roles Seeded");
        }

        private async Task SeedUsers()
        {
            if (context.Users.IgnoreQueryFilters().Any(u => u.Id > 0))
            {
                logger.LogInformation("Users already exists. Skipping seeding users");
                return;
            }

            var json = File.ReadAllText(USERS_SEED_FILE_PATH);
            var users = JsonSerializer.Deserialize<List<AppUser>>(json, jsonSerializerOptions);

            if (users == null)
            {
                logger.LogWarning("No users found in the seed file.");
                return;
            }
            foreach (var user in users)
            {
                user.KeyId = idGenerator.CreateId();
                user.Password = hashingService.HashPassword(user.Password);
            }
            context.Users.AddRange(users);
            await context.SaveChangesAsync();
            logger.LogInformation("Users Seeded");
        }

        private async Task SeedSystemUser()
        {
            if (!context.Roles.IgnoreQueryFilters().Any())
            {
                var systemRole = new AppUserRole
                {
                    Id = SystemUser.RoleId,
                    KeyId = idGenerator.CreateId(),
                    Name = UserRole.System
                };
                context.Roles.Add(systemRole);
            }

            if (!context.Organizations.IgnoreQueryFilters().Any())
            {
                context.Organizations.Add(new Organization
                {
                    Id = SystemUser.UserOrganizationId,
                    KeyId = idGenerator.CreateId(),
                    Name = "System",
                    Domain = "system",
                    CreatedAt = DateTime.UtcNow,
                });
            }

            if (!context.Users.IgnoreQueryFilters().Any())
            {
                var systemUser = new AppUser
                {
                    Id = SystemUser.UserId,
                    OrganizationId = SystemUser.UserOrganizationId,
                    KeyId = idGenerator.CreateId(),
                    Username = SystemUser.Username,
                    Email = SystemUser.Email,
                    Password = SystemUser.Password,
                    RoleId = SystemUser.RoleId,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = SystemUser.UserId,
                };
                context.Users.Add(systemUser);
                logger.LogInformation("System User Seeded");
            }
            await context.SaveChangesAsync();
        }
    }
}
