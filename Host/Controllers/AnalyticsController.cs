using Application.DTOs.Request.User;
using Application.DTOs.Response.Project;
using Application.DTOs.Response.User;
using Application.Services;
using Domain.Wrappers;
using Microsoft.AspNetCore.Mvc;

namespace Host.Controllers
{
    [ApiController]
    [Route("api/v1/analytics")]
    public class AnalyticsController(IAnalyticsService analyticsService) : ControllerBase
    {
        [HttpGet]
        public async Task<ApiResponse<AnalyticsResponse>> GetAnalytics()
        {
            return await analyticsService.GetManagerAnalytics();
        }
    }
}
