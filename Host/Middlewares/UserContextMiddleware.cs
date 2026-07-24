using Abstractions;
using Domain.Entities;
using Domain.Interfaces;
using Host.Extensions;

namespace Host.Middlewares
{
    public class UserContextMiddleware(RequestDelegate next)
    {
        public async Task InvokeAsync(HttpContext context, IUserContext userContext, IIdResolverService idResolverService)
        {
            // Check if the user is authenticated
            if (context.User.Identity?.IsAuthenticated ?? false)
            {
                var userInfo = context.User.GetUserInfo();
                userContext.IsAuthenticated = userInfo.IsAuthenticated;
                userContext.OrganizationId = userInfo.OrganizationId;
                userContext.UserKeyId = userInfo.UserKeyId;
                userContext.UserId = await idResolverService.ResolveIdAsync<AppUser>(userInfo.UserKeyId.ToString());
                userContext.Email = userInfo.Email;
                userContext.Role = userInfo.Role;
            }

            await next(context);
        }
    }
}
