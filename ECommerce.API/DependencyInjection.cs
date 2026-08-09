using ECommerce.API.Middlewares;
using ECommerce.APP.Features.Users.Commands.UpdateUser.Common;
using ECommerce.APP.Settings;
using ECommerce.Domain.Abstractions.ImageCloudinary;
using ECommerce.Infrastructure.Identity;
using ECommerce.Infrastructure.ImageCloudinary;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.RateLimiting;

namespace ECommerce.API;

public static class DependencyInjection
{
    // could return void but IServiceCollection return type makes it useful to chain
    public static IServiceCollection AddPresentation(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddProblemDetails(options =>
        {
            // Runs for EVERY ProblemDetails response in the entire app —
            //      this includes MapError's 4xx responses (validation/not-found/conflict/etc.)
            //      AND GlobalExceptionMiddleware's 500s,
            //          since that middleware also writes via IProblemDetailsService under the hood.
            // This is the ONE place traceId gets attached to error responses.
            // Individual error-building code (MapError, GlobalExceptionMiddleware)
            //      no longer needs to know about traceId at all —
            // It just happens automatically for every current AND future ProblemDetails response,
            //      without anyone having to remember to add it manually each time.
            options.CustomizeProblemDetails = context =>
            {
                context.ProblemDetails.Extensions["traceId"] = context.HttpContext.TraceIdentifier;
            };
        });

        services.AddExceptionHandler<GlobalExceptionMiddleware>();

        // discovers Fast Endpoints routes (MapGet, etc.) for OpenAPI
        services.AddEndpointsApiExplorer();

        // Add Cloudinary settings
        // services.Configure<CloudinarySettings>(...) was already correctly binding your user-secrets values to the class
        // this binding is only a safety net for the failure case
        services.AddOptions<CloudinarySettings>()
            .Bind(configuration.GetSection("CloudinarySettings"))
            .ValidateDataAnnotations()   // needs the [Required] attributes in CloudinarySettings
            .ValidateOnStart();

        // Register Cloudinary service
        services.AddScoped<ICloudinaryService, CloudinaryService>();

        // Register all validators in the API assembly
        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);

        // Configures Swagger documentation generation
        services.AddSwaggerGen(options =>
        {
            // Define a Swagger document for API version 1, 2
            options.SwaggerDoc("v1", new OpenApiInfo { Title = "ECommerce API V1", Version = "v1" });
            options.SwaggerDoc("v2", new OpenApiInfo { Title = "ECommerce API V2", Version = "v2" });

            // Tells Swagger which endpoints belong to which version
            // Show endpoints based on GroupName
            options.DocInclusionPredicate((docName, apiDesc) => apiDesc.GroupName == docName);

            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Description = "JWT Bearer. Example: Bearer {token}",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT"
            });

            options.AddSecurityRequirement(document =>
                new OpenApiSecurityRequirement
                {
                    [new OpenApiSecuritySchemeReference("Bearer", document)] = []
                });
        });

        // Make System.Text.Json serialize/deserialize enums as strings
        services.ConfigureHttpJsonOptions(options =>
        {
            options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
            options.SerializerOptions.Converters.Add(new OptionalJsonConverterFactory());
        });

        // AddIdentityCore, not AddIdentity:
        // AddIdentity<TUser,TRole> lives in Microsoft.AspNetCore.Identity
        //      and pulls in cookie-auth types that force a FrameworkReference to Microsoft.AspNetCore.App —
        //          i.e. the whole ASP.NET Core shared framework — onto whatever project calls it (Infrastructure).
        // AddIdentityCore lives in Microsoft.Extensions.Identity.Core
        //      (a plain NuGet package, no framework reference),
        // so Infrastructure stays a class library that doesn't know about the web host at all.
        services.AddIdentityCore<ApplicationUser>(options =>
        {
            // Password policy: min length 8, must include a digit, an uppercase letter, and
            // a non-alphanumeric character (e.g. !@#$). No RequireLowercase set explicitly,
            // so it stays at its default (true) — lowercase is still required.
            options.Password.RequiredLength = 8;
            options.Password.RequireDigit = true;
            options.Password.RequireNonAlphanumeric = true;
            options.Password.RequireUppercase = true;

            // Rejects registration if the email is already in use by another account.
            options.User.RequireUniqueEmail = true;

            // After 5 failed login attempts, the account is locked out for 15 minutes.
            // This is enforced by SignInManager.CheckPasswordSignInAsync
            options.Lockout.MaxFailedAccessAttempts = 5;
            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);

            // NOTE: RequireConfirmedEmail and RequireConfirmedPhoneNumber
            //      both being true means a user literally cannot sign in
            //      until BOTH an email confirmation link AND an SMS/phone confirmation code have been completed.
            // If you don't have a phone-confirmation flow built
            //      (SMS provider, OTP endpoint, etc.) yet,
            //      every single user will be permanently locked out at sign-in with no way to unblock themselves.
            options.SignIn.RequireConfirmedPhoneNumber = true;
            options.SignIn.RequireConfirmedEmail = true;
        })
            .AddRoles<ApplicationRole>()                     // AddIdentityCore doesn't wire up roles by default —
                                                             // this restores RoleManager<ApplicationRole> and role-based claims
            .AddSignInManager()                              // Registers SignInManager<ApplicationUser> —
                                                             // handles password checks, lockout tracking, and 2FA flow.
            .AddEntityFrameworkStores<ECommerceIdentityDbContext>()   // Wires UserStore/RoleStore to persist through EF Core
                                                                      // against your Identity DbContext.
            .AddDefaultTokenProviders();                     // Registers providers for password-reset tokens, email-confirmation
                                                             // tokens, and 2FA tokens —
                                                             // without this, GeneratePasswordResetTokenAsync return null instead of a usable token.

        var jwtSettings = configuration.GetSection(JwtSettings.SectionName).Get<JwtSettings>()
            ?? throw new InvalidOperationException(
                $"Configuration section '{JwtSettings.SectionName}' is missing.");

        // HS256 secret must be long enough (reject short / empty secrets at startup).
        if (string.IsNullOrWhiteSpace(jwtSettings.Secret) || jwtSettings.Secret.Length < 32)
            throw new InvalidOperationException("Jwt:Secret must be at least 32 characters.");


        // Nothing can actually authenticate a request until a real scheme is added here.
        services.AddAuthentication(options =>
        {
            // Which scheme runs when you call [Authorize] / RequireAuthorization (read the JWT).
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme; // "Bearer"

            // Which scheme runs when auth fails (401 Challenge — WWW-Authenticate: Bearer).
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
            // Rules applied to every incoming Bearer token.
            .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                NameClaimType = JwtRegisteredClaimNames.Sub,
                RoleClaimType = "role",

                // must match Jwt:Issuer (reject tokens from other APIs).
                ValidateIssuer = true,
                ValidIssuer = jwtSettings.Issuer,

                // must match Jwt:Audience (reject tokens meant for another client/API).
                ValidateAudience = true,
                ValidAudience = jwtSettings.Audience,

                // Signature must be valid using our shared secret (HS256).
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Secret)),

                // Reject expired tokens (check exp claim).
                ValidateLifetime = true,

                // It overrides the default 5-minute buffer, allowing tokens to be accepted
                // as valid for up to one minute past their strict expiration time to account
                // for minor time differences between servers
                ClockSkew = TimeSpan.FromSeconds(30)
            };
            // Without it, the JWT handler may translate standard JWT claims
            // like sub and email into the older XML-based claim types internally
            options.MapInboundClaims = false;
        });

        // Enables [Authorize] / policies (roles, etc.) after authentication has set HttpContext.User.
        services.AddAuthorization();

        // Needed so CurrentUserService can read claims from the current request.
        services.AddHttpContextAccessor();

        services.AddRateLimiter(options =>
        {
            options.AddPolicy("verify-code", httpContext =>
                RateLimitPartition.GetFixedWindowLimiter(
                partitionKey: httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                factory: _ => new FixedWindowRateLimiterOptions
                {
                    PermitLimit = 10,
                    Window = TimeSpan.FromMinutes(1),
                    QueueLimit = 0
                }));

            options.OnRejected = async (context, ct) =>
            {
                context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;

                // Route through the same IProblemDetailsService your other error
                // responses use (GlobalExceptionMiddleware's 500s, MapError's 4xxs) —
                // so this gets the same shape AND picks up the traceId automatically
                // via the CustomizeProblemDetails callback already configured in
                // AddProblemDetails(), instead of a bespoke plain-text body.
                var problemDetailsService = context.HttpContext.RequestServices
                    .GetRequiredService<IProblemDetailsService>();

                await problemDetailsService.WriteAsync(new ProblemDetailsContext
                {
                    HttpContext = context.HttpContext,
                    ProblemDetails = new ProblemDetails
                    {
                        Status = StatusCodes.Status429TooManyRequests,
                        Title = "Too Many Requests",
                        Detail = "Too many requests. Try again shortly.",
                        Type = "https://tools.ietf.org/html/rfc6585#section-4"
                    }
                });
            };
        });

        return services;
    }
}
