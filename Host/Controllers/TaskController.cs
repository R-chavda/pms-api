using Application.DTOs.Request.Task;
using Application.DTOs.Response.Task;
using Application.Services;
using Domain.Constants;
using Domain.Wrappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Host.Controllers
{
    [ApiController]
    [Route("api/v1/tasks")]
    public class TaskController(ITaskService taskService) : ControllerBase
    {
        [HttpPost]
        [Authorize(Roles = $"{UserRoles.Manager},{UserRoles.Employee}")]
        public async Task<ApiResponse<object>> Create(CreateTaskReqDto createTaskReq)
        {
            return await taskService.CreateTaskAsync(createTaskReq);
        }

        [HttpDelete("{keyId}")]
        [Authorize(Roles = $"{UserRoles.Manager},{UserRoles.Employee}")]
        public async Task<ApiResponse<object>> Delete(long keyId)
        {
            return await taskService.DeleteTaskByKeyIdAsync(keyId);
        }

        [HttpPut("{keyId}")]
        [Authorize(Roles = $"{UserRoles.Manager},{UserRoles.Employee}")]
        public async Task<ApiResponse<object>> UpdateDetails(long keyId, UpdateTaskReqDto updateTaskReq)
        {
            return await taskService.UpdateTaskDetailsAsync(keyId, updateTaskReq);
        }

        [HttpPut("{keyId}/status")]
        [Authorize(Roles = $"{UserRoles.Manager},{UserRoles.Employee}")]
        public async Task<ApiResponse<object>> UpdateStatus(long keyId, UpdateTaskStatusReqDto updateTaskStatusReq)
        {
            return await taskService.UpdateTaskStatusAsync(keyId, updateTaskStatusReq);
        }

        [HttpGet("project/{projectKeyId}")]
        [Authorize(Roles = $"{UserRoles.Manager},{UserRoles.Employee}")]
        public async Task<ApiResponse<List<TaskResponse>>> GetAll(long projectKeyId)
        {
            return await taskService.GetAllTasksAsync(projectKeyId);
        }
    }
}
