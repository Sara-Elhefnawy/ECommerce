using ECommerce.APP.Products;
using ECommerce.Domain.Abstractions.Interceptors;
using ECommerce.Domain.Abstractions.Repositories;
using ECommerce.Infrastructure.Persistent;
using ECommerce.Infrastructure.Persistent.Interceptors;
using ECommerce.Infrastructure.Persistent.Repositories;
using ECommerce.Infrastructure.Persistent.Seedings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ECommerce.Infrastructure;

public static class DependencyInjection
{
    // could return void but IServiceCollection return type makes it useful to chain
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IAuditInterceptor, AuditInterceptor>();
        services.AddScoped<ISoftDeleteInterceptor, SoftDeleteInterceptor>();

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
        // Register Generic Repository (optional - UnitOfWork handles this)
        //services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

        return services;
    }
}
