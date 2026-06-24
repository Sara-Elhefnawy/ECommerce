using ECommerce.Infrastructure.Interceptors;
using ECommerce.Infrastructure.Persistent;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ECommerce.Infrastructure;

public static class DependencyInjection
{
    // could return void but IServiceCollection return type makes it useful to chain
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<AuditInterceptor>();

        services.AddDbContext<ECommerceDbContext>((serviceProvider, options) =>
        {
            var auditInterceptor = serviceProvider.GetRequiredService<AuditInterceptor>();

            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")
                        ?? throw new InvalidOperationException("Connection string 'DefaultConnection' was not found."),
                        sqlOptions => sqlOptions.MigrationsAssembly(typeof(ECommerceDbContext).Assembly.FullName));
            options.AddInterceptors(auditInterceptor);
        });

        return services;
    }
}
