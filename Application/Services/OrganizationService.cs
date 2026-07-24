using System.Text.Json;
using Abstractions;
using Application.DTOs.Request.Organization;
using Application.DTOs.Response.Organization;
using Application.Mappings;
using Domain.Entities;
using Domain.Enums;
using Domain.Wrappers;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace Application.Services
{
    public interface IOrganizationService
    {
        Task<ApiResponse<object>> CreateOrganizationAsync(CreateOrganizationReqDto request);
        Task<ApiResponse<object>> DeleteOrganizationAsync(long keyId);
        Task<ApiResponse<object>> UpdateOrganizationAsync(long keyId, UpdateOrganizationReqDto request);
        Task<ApiResponse<object>> UpdateOrganizationThemeAsync(long keyId, UpdateOrganizationThemeReqDto request);
        Task<ApiResponse<object>> UpdateOrganizationSettingAsync(long keyId, string name, string logoPath);
        Task<ApiResponse<List<OrganizationResponse>>> GetOrganizationsAsync();
    }
    public class OrganizationService(
        IGenericRepository<Organization> organizationRepo,
        IGenericRepository<AppUser> userRepo,
        IGenericRepository<AppUserRole> roleRepo,
        IHashingService hashingService
        ) : IOrganizationService
    {
        public async Task<ApiResponse<object>> CreateOrganizationAsync(CreateOrganizationReqDto request)
        {
            var organization = request.Adapt<Organization>();
            organizationRepo.Add(organization);
            await organizationRepo.SaveChangesAsync();

            var adminRole = await roleRepo.GetSingleAsync(r => r.Name == UserRole.Admin);
            var adminUser = request.AdminCredentials.Adapt<AppUser>();
            adminUser.Password = hashingService.HashPassword(request.AdminCredentials.Password);
            adminUser.OrganizationId = organization.Id;
            adminUser.RoleId = adminRole!.Id;

            userRepo.Add(adminUser);
            await userRepo.SaveChangesAsync();
            return ApiResponse<object>.Success(StatusCode.Created, default, "Organization created successfully.");
        }

        public async Task<ApiResponse<object>> DeleteOrganizationAsync(long keyId)
        {
            var organization = await organizationRepo.GetByKeyIdAsync(keyId);
            if (organization == null)
            {
                return ApiResponse<object>.Fail(StatusCode.NoContent, "Organization not found");
            }

            organizationRepo.Remove(organization);
            await organizationRepo.SaveChangesAsync();
            return ApiResponse<object>.Success(StatusCode.NoContent, default, "Organization deleted");
        }

        public async Task<ApiResponse<List<OrganizationResponse>>> GetOrganizationsAsync()
        {
            var organizations = await organizationRepo.GetAllAsync(null, include: o => o.Include(o => o.OrganizationMembers));
            var parsedResponse = organizations.Select(o => o.ToResponse()).ToList();
            return ApiResponse<List<OrganizationResponse>>.Success(StatusCode.Success, parsedResponse, "Organizations fetched");
        }

        public async Task<ApiResponse<object>> UpdateOrganizationAsync(long keyId, UpdateOrganizationReqDto request)
        {
            var organization = await organizationRepo.GetByKeyIdAsync(keyId);
            if (organization == null)
            {
                return ApiResponse<object>.Fail(StatusCode.NoContent, "Organization not found");
            }

            organization.Name = request.Name;
            organization.Domain = request.Domain;

            organizationRepo.Update(organization);
            await organizationRepo.SaveChangesAsync();
            return ApiResponse<object>.Success(StatusCode.NoContent, default, "Organization updated");
        }

        public async Task<ApiResponse<object>> UpdateOrganizationThemeAsync(long keyId, UpdateOrganizationThemeReqDto request)
        {
            var organization = await organizationRepo.GetByKeyIdAsync(keyId);
            if (organization == null)
            {
                return ApiResponse<object>.Fail(StatusCode.NotFound, "Organization not found");
            }
            organization.Theme = JsonSerializer.Serialize(request, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });
            organizationRepo.Update(organization);
            await organizationRepo.SaveChangesAsync();
            return ApiResponse<object>.Success(StatusCode.NoContent, "Theme updated successfully");
        }

        public async Task<ApiResponse<object>> UpdateOrganizationSettingAsync(long keyId, string name, string logoPath)
        {
            var organization = await organizationRepo.GetByKeyIdAsync(keyId);
            if (organization == null)
            {
                return ApiResponse<object>.Fail(StatusCode.NotFound, "Organization not found");
            }
            organization.Name = name;
            if (!string.IsNullOrEmpty(logoPath))
            {
                organization.Logo = logoPath;
            }
            organizationRepo.Update(organization);
            await organizationRepo.SaveChangesAsync();
            return ApiResponse<object>.Success(StatusCode.NoContent, logoPath,"Organization Setting updated successfully");
        }
    }
}
