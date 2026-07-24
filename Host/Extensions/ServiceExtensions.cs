using Domain.Constants;
using Domain.Enums;
using Domain.Interfaces;
using Domain.Wrappers;
using Infrastructure.Data;
using Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

namespace Host.Extensions
{
    public static class ServiceExtensions
    {
        public async static Task SeedDatabase(this IApplicationBuilder app)
        {
            using (var scope = app.ApplicationServices.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                await db.Database.MigrateAsync();

                var seederService = scope.ServiceProvider.GetRequiredService<ISeederService>();
                await seederService.SeedData();
            }
        }

        public static void AddScopedServices(this IServiceCollection services)
        {
            services.AddScoped<IUserContext, UserContext>();
        }

        public static void ConfigureAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = configuration[AppSettings.JwtIssuer],
                        ValidAudience = configuration[AppSettings.JwtAudience],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration[AppSettings.JwtSecretKey]!))
                    };

                    options.Events = new JwtBearerEvents
                    {
                        OnAuthenticationFailed = c =>
                        {
                            if (c.Exception.GetType() == typeof(SecurityTokenExpiredException))
                            {
                                c.NoResult();
                                c.Response.ContentType = MimeTypes.Json;
                                c.Response.StatusCode = (int)StatusCode.Unauthorized;
                                var response = ApiResponse<object>.Fail(StatusCode.Unauthorized, "The token is expired");
                                return c.Response.WriteAsJsonAsync(response);
                            }
                            else
                            {
                                c.NoResult();
                                c.Response.ContentType = MimeTypes.Json;
                                c.Response.StatusCode = (int)StatusCode.Unauthorized;
                                var response = ApiResponse<object>.Fail(StatusCode.InternalServerError, "Invalid token");
                                return c.Response.WriteAsJsonAsync(response);
                            }
                        },
                        OnChallenge = context =>
                        {
                            context.HandleResponse();
                            context.Response.ContentType = MimeTypes.Json;
                            context.Response.StatusCode = (int)StatusCode.Unauthorized;
                            var response = ApiResponse<object>.Fail(StatusCode.Unauthorized, "Unauthorized access");
                            return context?.Response?.WriteAsJsonAsync(response)!;
                        },

                        OnForbidden = context =>
                        {
                            context.Response.StatusCode = (int)StatusCode.Forbidden;
                            var response = ApiResponse<object>.Fail(StatusCode.Forbidden, "Access is forbidden");
                            return context.Response.WriteAsJsonAsync(response);
                        }
                    };
                });
            services.AddAuthorization();
        }

        public static void AddSwagger(this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new() { Title = "PMS RestAPI", Version = "v1", Description = "This API will be responsible for serving the requests for PMS Portal" });

                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement()
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            },
                            Scheme = "oauth2",
                            Name = "Bearer",
                            In = ParameterLocation.Header,
                        },
                    new List<string>()
                    }
                });
            });
        }
    }
}
