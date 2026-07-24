using Application.DTOs.Response.Organization;
using Application.DTOs.Response.User;
using Domain.Entities;
using Mapster;

namespace Application.Mappings
{
    public static class OrganizationMappings
    {
        public static OrganizationResponse ToResponse(this Organization organization)
        {
            var organizationRes = organization.Adapt<OrganizationResponse>();
            organizationRes.OrganizationMembers = organization.OrganizationMembers.Select(m => m.Adapt<UserResponseMinimal>()).ToList();
            return organizationRes;
        }
    }
}