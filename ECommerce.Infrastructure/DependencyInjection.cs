using ECommerce.APP.Cachings;
using ECommerce.Domain.Abstractions.Repositories;
using ECommerce.Infrastructure.Cachings;
using ECommerce.Infrastructure.Persistent;
using ECommerce.Infrastructure.Persistent.Interceptors;
using ECommerce.Infrastructure.Persistent.Repositories;
using ECommerce.Infrastructure.Persistent.Seedings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

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
            // This MUST come before AddHybridCache().
            // HybridCache looks for an already-registered IDistributedCache in the
            // container and wraps it as L2.
            //      If nothing is registered, it silently uses an in-memory
            //      stand-in for L2 too
            //          which is why your cart never survives a restart.
            services.AddStackExchangeRedisCache(options =>
            {
                var configOptions = ConfigurationOptions.Parse(redisConnection);
                configOptions.AbortOnConnectFail = false; // retry instead of crashing app startup if Redis is briefly unreachable
                configOptions.ConnectRetry = 3;
                configOptions.ConnectTimeout = 5000;       // ms — cloud Redis over the internet is slower than localhost, give it room

                options.ConfigurationOptions = configOptions;
                options.InstanceName = "ECommerceRoute:";   // prefixes every key, helps you spot cart keys in redis-cli
            });
        };

        services.AddHybridCache(options =>
        {
            options.DefaultEntryOptions = new HybridCacheEntryOptions
            {
                LocalCacheExpiration = TimeSpan.FromMinutes(5), // L1 (in-process) — short, since it's per-instance and dies on redeploy/restart anyway
                Expiration = TimeSpan.FromHours(1)              // L2 (Redis) — the real source of truth across instances/restarts
            };
        });

        services.AddScoped(typeof(ICache<>), typeof(Cache<>));
        services.AddScoped<ICartRepository, CartRepository>();
    }
}
