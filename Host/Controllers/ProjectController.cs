using Application.DTOs.Request;
using Application.DTOs.Response.Project;
using Application.Services;
using Domain.Constants;
using Domain.Entities;
using Domain.Wrappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Host.Controllers
{
    [ApiController]
    [Authorize(Roles = $"{UserRoles.Manager},{UserRoles.Employee}")]
    [Route("api/v1/projects")]
    public class ProjectController(IProjectService projectService) : ControllerBase
    {
        [HttpPost]
        public async Task<ApiResponse<object>> Create(CreateProjectReqDto createProjectReq)
        {
            return await projectService.CreateProjectAsync(createProjectReq);
        }

        [HttpGet]
        public async Task<ApiResponse<List<ProjectResponse>>> GetAll()
        {
            return await projectService.GetAllProjectsAsync();
        }

        [HttpGet("{keyId}")]
        public async Task<ApiResponse<ProjectResponse>> GetByKeyId(long keyId)
        {
            return await projectService.GetProjectByKeyIdAsync(keyId);
        }

        [HttpPut("{keyId}")]
        public async Task<ApiResponse<object>> Update(long keyId, UpdateProjectReqDto updateProjectReq)
        {
            return await projectService.UpdateProjectAsync(keyId, updateProjectReq);
        }

        [HttpDelete("{keyId}")]
        public async Task<ApiResponse<object>> Delete(long keyId)
        {
            return await projectService.DeleteProjectByKeyIdAsync(keyId);
        }

        [HttpGet("meta-data")]
        public async Task<ApiResponse<List<ProjectResponseMinimal>>> GetProjectsMetadata()
        {
            return await projectService.GetProjectsMetadata();
        }

        [HttpGet("{keyId}/resources")]
        public async Task<ApiResponse<List<ProjectMemberResponse>>> GetResources(long keyId)
        {
            return await projectService.GetProjectResources(keyId);
        }

        [HttpPatch("{keyId}/add-member/{userKeyId}")]
        public Task<ApiResponse<object>> AddProjectMember(long keyId,string userKeyId)
        {
            return projectService.AddProjectMemberAsync(keyId, userKeyId);
        }

        [HttpPatch("{keyId}/remove-member/{userKeyId}")]
        public Task<ApiResponse<object>> RemoveProjectMember(long keyId,string userKeyId)
        {
            return projectService.RemoveProjectMemberAsync(keyId, userKeyId);
        }
    }
}
