using Abstractions;
using Application.DTOs.Request;
using Application.DTOs.Response.Project;
using Application.Mappings;
using Domain.Constants;
using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;
using Domain.Wrappers;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace Application.Services
{
    public interface IProjectService
    {
        Task<ApiResponse<object>> CreateProjectAsync(CreateProjectReqDto createProjectReq);
        Task<ApiResponse<object>> UpdateProjectAsync(long keyId, UpdateProjectReqDto updateProjectReq);
        Task<ApiResponse<object>> DeleteProjectByKeyIdAsync(long keyId);
        Task<ApiResponse<object>> AddProjectMemberAsync(long keyId, string userKeyId);
        Task<ApiResponse<object>> RemoveProjectMemberAsync(long keyId, string userKeyId);
        Task<ApiResponse<List<ProjectResponse>>> GetAllProjectsAsync();
        Task<ApiResponse<ProjectResponse>> GetProjectByKeyIdAsync(long keyId);
        Task<ApiResponse<List<ProjectMemberResponse>>> GetProjectResources(long keyId);
        Task<ApiResponse<List<ProjectResponseMinimal>>> GetProjectsMetadata();
    }

    public class ProjectService(
        IGenericRepository<Project> projectRepo,
        IGenericRepository<AppUser> userRepo,
        IUserContext userContext,
        IIdResolverService idResolver
    ) : IProjectService
    {
        public async Task<ApiResponse<object>> CreateProjectAsync(CreateProjectReqDto createProjectReq)
        {
            var project = createProjectReq.Adapt<Project>();
            project.OrganizationId=userContext.OrganizationId;
            projectRepo.Add(project);
            await projectRepo.SaveChangesAsync();

            return ApiResponse<object>.Success(StatusCode.Created, default, "Project created");
        }

        public async Task<ApiResponse<object>> DeleteProjectByKeyIdAsync(long keyId)
        {
            var project = await projectRepo.GetByKeyIdAsync(keyId);
            if (project == null)
            {
                return ApiResponse<object>.Fail(StatusCode.NotFound, "Project not found");
            }
            if (project.CreatedBy != userContext.UserId)
            {
                return ApiResponse<object>.Fail(StatusCode.BadRequest, "You don't have permission to delete the project");
            }
            projectRepo.Remove(project);
            await projectRepo.SaveChangesAsync();
            return ApiResponse<object>.Success(StatusCode.NoContent, "Project deleted");
        }

        public async Task<ApiResponse<List<ProjectResponse>>> GetAllProjectsAsync()
        {
            var projects = await projectRepo.GetAllAsync(
                p => p.CreatedBy == userContext.UserId,
                p => p.Include(p => p.CreatedByUser!).Include(p => p.ProjectMembers!)
            );
            var parsed = projects.Select(p => p.Adapt<ProjectResponse>()).ToList();
            return ApiResponse<List<ProjectResponse>>.Success(StatusCode.Success, parsed);
        }

        public async Task<ApiResponse<ProjectResponse>> GetProjectByKeyIdAsync(long keyId)
        {
            var project = await projectRepo.GetSingleAsync(
                p => p.KeyId == keyId,
                p => p.Include(p => p.CreatedByUser!).Include(p => p.ProjectMembers!)
            );
            return ApiResponse<ProjectResponse>.Success(StatusCode.Success, project.Adapt<ProjectResponse>());
        }

        public async Task<ApiResponse<object>> UpdateProjectAsync(long keyId, UpdateProjectReqDto updateProjectReq)
        {
            var project = await projectRepo.GetByKeyIdAsync(keyId, tracking: true);
            if (project == null)
            {
                return ApiResponse<object>.Fail(StatusCode.NotFound, "Project not found");
            }
            if (project.CreatedBy != userContext.UserId)
            {
                return ApiResponse<object>.Fail(StatusCode.BadRequest, "You don't have permission to update the project");
            }

            project.Name = updateProjectReq.Name;
            project.Description = updateProjectReq.Description;

            projectRepo.Update(project);
            await projectRepo.SaveChangesAsync();
            return ApiResponse<object>.Success(StatusCode.Success, default, "Project updated successfully");
        }

        public async Task<ApiResponse<object>> AddProjectMemberAsync(long keyId, string userKeyId)
        {
            var project = await projectRepo.GetSingleAsync(
                p => p.KeyId == keyId,
                p => p.Include(p => p.ProjectMembers!),
                tracking: true
            );

            if (project == null)
            {
                return ApiResponse<object>.Fail(StatusCode.NotFound, "Project not found");
            }

            if (project.CreatedBy != userContext.UserId)
            {
                return ApiResponse<object>.Fail(StatusCode.BadRequest, "You don't have permission to add members to the project");
            }

            var userId = await idResolver.ResolveIdAsync<AppUser>(userKeyId);

            var user = await userRepo.GetByIdAsync(userId);
            project.ProjectMembers?.Add(user!);
            projectRepo.Update(project);
            await projectRepo.SaveChangesAsync();
            return ApiResponse<object>.Success(StatusCode.Success, default, "Project member added successfully");
        }

        public async Task<ApiResponse<object>> RemoveProjectMemberAsync(long keyId, string userKeyId)
        {
            var project = await projectRepo.GetSingleAsync(
                p => p.KeyId == keyId,
                p => p.Include(p => p.ProjectMembers!),
                tracking: true
            );

            if (project == null)
            {
                return ApiResponse<object>.Fail(StatusCode.NotFound, "Project not found");
            }

            if (project.CreatedBy != userContext.UserId)
            {
                return ApiResponse<object>.Fail(StatusCode.BadRequest, "You don't have permission to remove members to the project");
            }

            var userId = await idResolver.ResolveIdAsync<AppUser>(userKeyId);

            var user = await userRepo.GetByIdAsync(userId);
            project.ProjectMembers?.Remove(user!);
            projectRepo.Update(project);
            await projectRepo.SaveChangesAsync();
            return ApiResponse<object>.Success(StatusCode.Success, default, "Project member removed successfully");
        }

        public async Task<ApiResponse<List<ProjectMemberResponse>>> GetProjectResources(long keyId)
        {
            var resources = await userRepo.GetAllAsync(
                u => u.Projects!.Any(p => p.KeyId == keyId) && u.ReportsToUserId == userContext.UserId,
                u => u.Include(u => u.Role!)
            );
            var parsedResources = resources.Select(u => u.ToProjectMember()).ToList();
            return ApiResponse<List<ProjectMemberResponse>>.Success(StatusCode.Success, parsedResources, "Project resources fetched successfully");
        }

        public async Task<ApiResponse<List<ProjectResponseMinimal>>> GetProjectsMetadata()
        {
            var projects = await projectRepo.GetAllAsync(
                p => p.CreatedBy == userContext.UserId || p.ProjectMembers!.Any(u => u.Id == userContext.UserId)
            );
            var parsedProjects = projects.Select(p => p.Adapt<ProjectResponseMinimal>()).ToList();
            return ApiResponse<List<ProjectResponseMinimal>>.Success(StatusCode.Success, parsedProjects, "Projects metadata fetched successfully");
        }
    }
}
