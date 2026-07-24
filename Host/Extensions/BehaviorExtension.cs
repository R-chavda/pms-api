using Domain.Enums;
using Domain.Wrappers;
using Microsoft.AspNetCore.Mvc;

namespace Host.Extensions
{
    public static class BehaviorExtension
    {
        public static void ConfigureApiBehaviorForValidationError(this IServiceCollection services)
        {
            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = context =>
                {
                    var errors = context.ModelState
                        .Where(e => e.Value?.Errors.Count > 0)
                        .SelectMany(x => x.Value!.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();

                    var errorResponse = ApiResponse<object>.ValidationFailed("Validation failed", errors);
                    return new BadRequestObjectResult(errorResponse);
                };
            });
        }
    }
}
