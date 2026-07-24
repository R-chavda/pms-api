using Application.DTOs.Response.Task;
using Application.DTOs.Response.User;
using Domain.Entities;
using Mapster;

namespace Application.Mappings
{
    public static class TaskMappings
    {
        public static TaskResponse ToResponse(this TaskItem taskItem)
        {
            var task = taskItem.Adapt<TaskResponse>();
            task.AssignedToUser = taskItem.AssignedToUser.Adapt<UserResponseMinimal>();
            task.CreatedBy = taskItem.CreatedByUser!.Username;
            task.UpdatedBy = taskItem.UpdatedByUser?.Username ?? string.Empty;
            return task;
        }
    }
}
