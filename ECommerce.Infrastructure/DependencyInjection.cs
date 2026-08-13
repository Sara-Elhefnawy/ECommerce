using ECommerce.APP.Cachings;
using ECommerce.APP.Cachings.Carts;
using ECommerce.APP.Cachings.ResetPassword;
using ECommerce.APP.Email;
using ECommerce.APP.Identity;
using ECommerce.APP.Settings;
using ECommerce.APP.Token;
using ECommerce.APP.Token.RefreshTokens;
using ECommerce.Domain.Abstractions.Repositories;
using ECommerce.Infrastructure.Cachings;
using ECommerce.Infrastructure.Cachings.Carts;
using ECommerce.Infrastructure.Cachings.ResetPassword;
using ECommerce.Infrastructure.Email;
using ECommerce.Infrastructure.Identity;
using ECommerce.Infrastructure.Persistent;
using ECommerce.Infrastructure.Persistent.Interceptors;
using ECommerce.Infrastructure.Persistent.Repositories;
using ECommerce.Infrastructure.Persistent.Seedings;
using ECommerce.Infrastructure.Token;
using ECommerce.Infrastructure.Token.RefreshTokens;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using StackExchange.Redis;
using System.Net;
using System.Net.Mail;

namespace ECommerce.Infrastructure;

public static class DependencyInjection
{
    // could return void but IServiceCollection return type makes it useful to chain
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<AuditInterceptor>();
        services.AddScoped<SoftDeleteInterceptor>();

        services.AddDbContext<ECommerceDbContext>((serviceProvider, options) =>
        {
            var auditInterceptor = serviceProvider.GetRequiredService<AuditInterceptor>();
            var softDeleteInterceptor = serviceProvider.GetRequiredService<SoftDeleteInterceptor>();

            var connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

            options.UseSqlServer(connectionString, sql =>
                sql.MigrationsHistoryTable("__ApplicationMigrationsHistroy"));

            options.AddInterceptors(softDeleteInterceptor, auditInterceptor);
        });

        services.AddDbContext<ECommerceIdentityDbContext>(options =>
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

            options.UseSqlServer(connectionString, sql =>
                sql.MigrationsHistoryTable("__IdentityMigrationsHistroy"));
        });

        services.AddScoped<DatabaseSeeder>();

        services.AddScoped<IDataSeeder, IdentitySeeder>();
        services.AddScoped<IDataSeeder, ProductBrandSeeder>();
        services.AddScoped<IDataSeeder, ProductTypeSeeder>();
        services.AddScoped<IDataSeeder, ProductSeeder>();

        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddScoped(typeof(IReadRepository<>), typeof(Repository<>));

        services.Configure<JwtSettings>(configuration.GetSection(JwtSettings.SectionName));
        services.AddScoped<IIdentityService, IdentityService>();
        services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
        services.AddScoped<IRefreshTokenService, RefreshTokenService>();
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        services.AddScoped<IEmailVerification, EmailVerification>();

        services.Configure<EmailVerificationSettings>(
            configuration.GetSection(EmailVerificationSettings.SectionName));

        services.Configure<EmailSettings>(configuration.GetSection(EmailSettings.SectionName));

        var emailSettings = configuration.GetSection(EmailSettings.SectionName).Get<EmailSettings>()
            ?? throw new InvalidOperationException($"Configuration section '{EmailSettings.SectionName}' is missing.");

        services.AddFluentEmail(emailSettings.FromEmail, emailSettings.FromName)
            .AddSmtpSender(() => new SmtpClient(emailSettings.Host, emailSettings.Port)
            {
                // Without this, SmtpClient defaults to UseDefaultCredentials = false
                // with no credentials attached at all — which is exactly the "please
                // authenticate first" error Brevo (and any real SMTP provider) throws.
                // Mailpit doesn't require auth, which is why this gap went unnoticed
                // through all the dev testing so far.
                Credentials = new NetworkCredential(emailSettings.Username, emailSettings.Password),

                // Brevo's port 587 uses STARTTLS (plaintext connection that upgrades
                // to TLS) rather than implicit TLS on connect. EnableSsl = true tells
                // SmtpClient to issue the STARTTLS command after connecting.
                EnableSsl = true
            });

        services.AddScoped<IEmailSender, FluentEmailSender>();

        AddCartCaching(services, configuration);

        services.AddScoped<IResetPasswordRepository, ResetPasswordRepository>();

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
        }

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
