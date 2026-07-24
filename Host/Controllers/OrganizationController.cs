using Application.DTOs.Request.Organization;
using Application.DTOs.Response.Organization;
using Application.Services;
using Domain.Constants;
using Domain.Wrappers;
using Host.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Host.Controllers
{
    [ApiController]
    [Route("api/v1/organizations")]
    public class OrganizationController(IOrganizationService organizationService) : ControllerBase
    {
        [HttpGet]
        [Authorize(Roles = $"{UserRoles.SuperUser}")]
        public async Task<ApiResponse<List<OrganizationResponse>>> GetAll()
        {
            return await organizationService.GetOrganizationsAsync();
        }

        [HttpPost]
        [Authorize(Roles = $"{UserRoles.SuperUser}")]
        public async Task<ApiResponse<object>> Create(CreateOrganizationReqDto createOrganizationReq)
        {
            return await organizationService.CreateOrganizationAsync(createOrganizationReq);
        }

        [HttpPut("{keyId}")]
        [Authorize(Roles = $"{UserRoles.SuperUser}")]
        public async Task<ApiResponse<object>> Update(long keyId, UpdateOrganizationReqDto updateOrganizationReq)
        {
            return await organizationService.UpdateOrganizationAsync(keyId, updateOrganizationReq);
        }

        [HttpPut("{keyId}/theme")]
        [Authorize(Roles = $"{UserRoles.Admin}")]
        public async Task<ApiResponse<object>> UpdateTheme(long keyId, UpdateOrganizationThemeReqDto updateOrganizationThemeReq)
        {
            return await organizationService.UpdateOrganizationThemeAsync(keyId, updateOrganizationThemeReq);
        }

        [HttpPut("{keyId}/setting")]
        [Authorize(Roles = $"{UserRoles.Admin}")]
        public async Task<ApiResponse<object>> UpdateSetting(long keyId, [FromForm] UpdateOrganizationSettingReqDto updateOrganizationSettingReq)
        {
            var file = updateOrganizationSettingReq.Logo;
            string logoUrl = string.Empty;
            if (file != null && file.Length != 0)
            {
                var uploadsPath = Path.Combine("wwwroot", "uploads", "logos");
                if (!Directory.Exists(uploadsPath)) Directory.CreateDirectory(uploadsPath);

                var extension = Path.GetExtension(file.FileName);
                var fileName = $"{updateOrganizationSettingReq.Name}{extension}";
                var filePath = Path.Combine(uploadsPath, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }
                logoUrl = $"{Request.Scheme}://{Request.Host}/uploads/logos/{fileName}";
            }
            return await organizationService.UpdateOrganizationSettingAsync(keyId, updateOrganizationSettingReq.Name, logoUrl);
        }

        [HttpDelete("{keyId}")]
        [Authorize(Roles = $"{UserRoles.SuperUser}")]
        public async Task<ApiResponse<object>> Delete(long keyId)
        {
            return await organizationService.DeleteOrganizationAsync(keyId);
        }
    }
}
