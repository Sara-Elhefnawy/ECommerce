using ECommerce.APP.Cachings;
using ECommerce.Domain.Abstractions.Repositories;
using ECommerce.Infrastructure.Cachings;
using ECommerce.Infrastructure.Persistent;
using ECommerce.Infrastructure.Persistent.Interceptors;
using ECommerce.Infrastructure.Persistent.Repositories;
using ECommerce.Infrastructure.Persistent.Seedings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace ECommerce.Infrastructure;

public static class DependencyInjection
{
    // could return void but IServiceCollection return type makes it useful to chain
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<AuditInterceptor>();
        services.AddSingleton<SoftDeleteInterceptor>();

        services.AddScoped<AuditInterceptor>();
        services.AddScoped<SoftDeleteInterceptor>();

        services.AddDbContext<ECommerceDbContext>((serviceProvider, options) =>
        {
            var auditInterceptor = serviceProvider.GetRequiredService<AuditInterceptor>();
            var softDeleteInterceptor = serviceProvider.GetRequiredService<SoftDeleteInterceptor>();

            var connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

            options.UseSqlServer(connectionString);

            options.AddInterceptors(softDeleteInterceptor, auditInterceptor);
        });

        services.AddScoped<DatabaseSeeder>();

        services.AddScoped<IDataSeeder, ProductBrandSeeder>();
        services.AddScoped<IDataSeeder, ProductTypeSeeder>();
        services.AddScoped<IDataSeeder, ProductSeeder>();

        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddScoped(typeof(IReadRepository<>), typeof(Repository<>));

        AddCartCaching(services, configuration);

        return services;
    }

    private static void AddCartCaching(IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddOptions<CacheEntryPolicy>("Cart")
            .Bind(configuration.GetSection("Cache:Cart"))
            .ValidateOnStart();

        services.AddSingleton<IValidateOptions<CacheEntryPolicy>, CacheEntryPolicyValidator>();

        var redisConnection = configuration.GetConnectionString("Redis");

        if (!string.IsNullOrWhiteSpace(redisConnection))
        {
            services.AddStackExchangeRedisCache(options =>
                options.Configuration = redisConnection);
        }

        services.AddHybridCache();

        services.AddScoped(typeof(ICache<>), typeof(Cache<>));
        services.AddScoped<ICartRepository, CartRepository>();
    }
}
