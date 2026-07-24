using Application;
using Infrastructure;
using Host.Extensions;
using Host.Middlewares;
using Domain.Constants;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddNLogLogger();
builder.Services.AddApplicationLayer();
builder.Services.AddInfrastructureLayer(builder.Configuration);
builder.Services.AddScopedServices();
builder.Services.AddSwagger();
builder.Services.AddControllers();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigins", policy =>
    {
        policy.WithOrigins(builder.Configuration[AppSettings.AllowedOrigin]!)
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.ConfigureApiBehaviorForValidationError();
builder.Services.ConfigureAuthentication(builder.Configuration);

var app = builder.Build();
await app.SeedDatabase(); // Seed the database with initial data

// Configure the HTTP request pipeline.
app.UseCors("AllowSpecificOrigins");
app.UseStaticFiles();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseMiddleware<ErrorHandlingMiddleware>();
app.UseAuthentication();
app.UseMiddleware<UserContextMiddleware>();
app.UseAuthorization();
app.UseMiddleware<RequestLoggingMiddleware>();
app.MapControllers();
app.Run();
