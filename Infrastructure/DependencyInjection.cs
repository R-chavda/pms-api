using Abstractions;
using Domain.Constants;
using IdGen;
using Infrastructure.Data;
using Infrastructure.Repositories;
using Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure
{
    public static class DependencyInjection
    {
        public static void AddInfrastructureLayer(this IServiceCollection services, IConfiguration configuration)
        {
            services.ConfigureIdGenerator(configuration);
            services.AddDatabase(configuration);
            services.AddSeeder();
            services.AddRepositories();
            services.AddScopedServices();
            //services.AddRedisCaching(configuration);
        }

        private static void AddDatabase(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseNpgsql(configuration.GetConnectionString(AppSettings.DbConnection));
            });
        }

        private static void AddSeeder(this IServiceCollection services)
        {
            services.AddScoped<ISeederService, SeederService>();
        }

        private static void ConfigureIdGenerator(this IServiceCollection services, IConfiguration configuration)
        {
            var generatorId = byte.Parse(configuration[AppSettings.IdGeneratorId]!);
            services.AddSingleton<IIdGenerator<long>>(provider =>
            {
                return new IdGenerator(generatorId);
            });
        }

        private static void AddScopedServices(this IServiceCollection services)
        {
            services.AddScoped<IIdResolverService, IdResolverService>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IHashingService, HashingService>();
            services.AddScoped<IMqttPublisherService,MqttPublisherService>();
        }

        private static void AddRepositories(this IServiceCollection services)
        {
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
        }

        //public static void AddRedisCaching(this IServiceCollection services, IConfiguration configuration)
        //{
        //    services.AddStackExchangeRedisCache(options =>
        //    {
        //        options.Configuration = configuration.GetConnectionString("Redis");
        //        options.InstanceName = "PMS_";
        //    });
        //}
    }
}
