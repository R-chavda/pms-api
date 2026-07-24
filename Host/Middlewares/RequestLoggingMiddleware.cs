using System.Diagnostics;
using Domain.Interfaces;

namespace Host.Middlewares
{
    public class RequestLoggingMiddleware(RequestDelegate next)
    {
        public async Task InvokeAsync(HttpContext context, IUserContext userContext, ILogger<RequestLoggingMiddleware> logger)
        {
            logger.LogInformation($"Request received from the {(!userContext.IsAuthenticated ? "Unauthenticated user" : "User Id-" + userContext.UserId)}");
            logger.LogInformation($"{context.Request.Method} - {context.Request.Path}");
            var stopwatch = Stopwatch.StartNew();
            await next(context);
            stopwatch.Stop();
            logger.LogInformation($"Request duration: {stopwatch.ElapsedMilliseconds} ms");
            logger.LogInformation($"Response sent with Status Code - {context.Response.StatusCode}");
        }
    }
}
