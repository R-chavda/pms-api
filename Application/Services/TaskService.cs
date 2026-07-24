using Abstractions;
using Application.DTOs.Request.Task;
using Application.DTOs.Response.Project;
using Application.DTOs.Response.Task;
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
    public interface ITaskService
    {
        Task<ApiResponse<object>> CreateTaskAsync(CreateTaskReqDto createTaskReq);
        Task<ApiResponse<object>> DeleteTaskByKeyIdAsync(long keyId);
        Task<ApiResponse<object>> UpdateTaskDetailsAsync(long keyId, UpdateTaskReqDto updateTaskReq);
        Task<ApiResponse<object>> UpdateTaskStatusAsync(long keyId, UpdateTaskStatusReqDto updateTaskStatusReq);
        Task<ApiResponse<List<TaskResponse>>> GetAllTasksAsync(long projectKeyId);
    }
    public class TaskService(
        IGenericRepository<TaskItem> taskRepo,
        IIdResolverService idResolver,
        IUserContext userContext,
        IMqttPublisherService mqttPublisherService
    ) : ITaskService
    {
        public async Task<ApiResponse<object>> CreateTaskAsync(CreateTaskReqDto createTaskReq)
        {
            var task = createTaskReq.Adapt<TaskItem>();

            var projectId = await idResolver.ResolveIdAsync<Project>(createTaskReq.ProjectKeyId);
            task.ProjectId = projectId;

            var userRole = userContext.Role;
            if (userRole == UserRoles.Manager)
            {
                var assignedToUserId = await idResolver.ResolveIdAsync<AppUser>(createTaskReq.AssignedToUserKeyId!);
                task.AssignedToUserId = assignedToUserId;
            }
            else if (userRole == UserRoles.Employee)
            {
                task.AssignedToUserId = userContext.UserId;
            }

            taskRepo.Add(task);
            await taskRepo.SaveChangesAsync();
            return ApiResponse<object>.Success(StatusCode.Created, default, "Task created");
        }

        public async Task<ApiResponse<object>> DeleteTaskByKeyIdAsync(long keyId)
        {
            var task = await taskRepo.GetByKeyIdAsync(keyId);
            if (task == null)
            {
                return ApiResponse<object>.Fail(StatusCode.NotFound, "Task not found");
            }
            if (task.CreatedBy != userContext.UserId)
            {
                return ApiResponse<object>.Fail(StatusCode.BadRequest, "You don't have permission to delete the task");
            }

            taskRepo.Remove(task);
            await taskRepo.SaveChangesAsync();
            return ApiResponse<object>.Success(StatusCode.NoContent, "Task deleted");
        }

        public async Task<ApiResponse<List<TaskResponse>>> GetAllTasksAsync(long projectKeyId)
        {
            var projectId = await idResolver.ResolveIdAsync<Project>(projectKeyId.ToString());
            var tasks = await taskRepo.GetAllAsync(
                t => t.ProjectId == projectId,
                t => t.Include(t => t.AssignedToUser!).Include(t => t.CreatedByUser!).Include(t => t.UpdatedByUser!)
            );
            var parsedTasks = tasks.Select(t => t.ToResponse()).ToList();
            return ApiResponse<List<TaskResponse>>.Success(StatusCode.Success, parsedTasks);
        }

        public async Task<ApiResponse<object>> UpdateTaskDetailsAsync(long keyId, UpdateTaskReqDto updateTaskReq)
        {
            var task = await taskRepo.GetByKeyIdAsync(keyId);
            if (task == null)
            {
                return ApiResponse<object>.Fail(StatusCode.NotFound, "Task not found");
            }

            if (task.CreatedBy != userContext.UserId)
            {
                return ApiResponse<object>.Fail(StatusCode.BadRequest, "You don't have permission to update the task");
            }

            var updatedTask = updateTaskReq.Adapt<TaskItem>();
            task.Summary = updatedTask.Summary;
            task.Description = updatedTask.Description;
            task.Priority = updatedTask.Priority;

            taskRepo.Update(task);
            await taskRepo.SaveChangesAsync();
            return ApiResponse<object>.Success(StatusCode.NoContent, "Task details updated successfully");
        }

        public async Task<ApiResponse<object>> UpdateTaskStatusAsync(long keyId, UpdateTaskStatusReqDto updateTaskStatusReq)
        {
            var task = await taskRepo.GetByKeyIdAsync(keyId);
            if (task == null)
            {
                return ApiResponse<object>.Fail(StatusCode.NotFound, "Task not found");
            }

            if (task.AssignedToUserId != userContext.UserId)
            {
                return ApiResponse<object>.Fail(StatusCode.BadRequest, "You don't have permission to update the task status");
            }
            var oldStatus = task.Status.ToString();
            task.Status = updateTaskStatusReq.Status;
            taskRepo.Update(task);
            await mqttPublisherService.PublishTaskUpdateAsync(task.KeyId.ToString(),oldStatus,task.Status.ToString());
            await taskRepo.SaveChangesAsync();

            return ApiResponse<object>.Success(StatusCode.NoContent, "Task status updated successfully");
        }
    }
}
