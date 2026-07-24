using Domain.Enums;
using Domain.Wrappers;

namespace Host.Middlewares
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorHandlingMiddleware> _logger;
        public ErrorHandlingMiddleware(RequestDelegate next,ILogger<ErrorHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleErrorAsync(context, ex);
            }
        }

        private async Task HandleErrorAsync(HttpContext context, Exception ex)
        {
            _logger.LogError($"Error occurred while processing the request: {ex}");
            if (ex.InnerException is not null)
            {
                _logger.LogError($"Inner Exception: {ex.InnerException}");
            }

            if (context.Response.HasStarted)
            {
                _logger.LogWarning("The response has already started, the error handling middleware will not write to the response.");
                return;
            }

            ApiResponse<object> response = ex switch
            {
                AppException appEx => ApiResponse<object>.Fail(appEx.StatusCode, appEx.Message),
                _ => ApiResponse<object>.Fail(StatusCode.InternalServerError, "An unexpected error occurred.")
            };
            
            await context.Response.WriteAsJsonAsync(response);
        }
    }
}
