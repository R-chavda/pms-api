using System.Reflection.PortableExecutable;
using Abstractions;
using Application.DTOs.Request.User;
using Application.DTOs.Response.Project;
using Application.DTOs.Response.User;
using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;
using Domain.Wrappers;
using Microsoft.EntityFrameworkCore;

namespace Application.Services
{
    public interface IAnalyticsService
    {
        Task<ApiResponse<AnalyticsResponse>> GetManagerAnalytics();
    }

    public class AnalyticsService(
        IGenericRepository<AppUser> userRepo,
        IGenericRepository<TaskItem> taskRepo,
        IGenericRepository<Project> projectRepo,
        IUserContext userContext
        ) : IAnalyticsService
    {
        public async Task<ApiResponse<AnalyticsResponse>> GetManagerAnalytics()
        {
            var projects = await projectRepo.GetAllAsync(p => p.CreatedBy == userContext.UserId, include: p => p.Include(p => p.Tasks!).Include(p => p.CreatedByUser).Include(p => p.ProjectMembers!));
            var resources = await userRepo.GetAllAsync(u => u.ReportsToUserId == userContext.UserId);
            var tasks = await taskRepo.GetAllAsync(t => t.Project!.CreatedBy == userContext.UserId, include: t => t.Include(t => t.Project!));

            var taskStatusPerProject = tasks
                .GroupBy(t => t.Project!.Name)
                .Select(g => new
                {
                    Projectname = g.Key,
                    Pending = g.Count(t => t.Status == Domain.Enums.TaskStatus.Created),
                    InProgress = g.Count(t => t.Status == Domain.Enums.TaskStatus.InProgress),
                    Completed = g.Count(t => t.Status == Domain.Enums.TaskStatus.Completed),
                }).ToList();

            var response = new AnalyticsResponse
            {
                ProjectsCount = projects.Count,
                ResourcesCount = resources.Count,
                TaskStatusPerProject = taskStatusPerProject
            };
            return ApiResponse<AnalyticsResponse>.Success(StatusCode.Success, response, "Analytics fetched successfully");
        }
    }
}
