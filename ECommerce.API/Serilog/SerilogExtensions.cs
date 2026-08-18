using Serilog;
using Serilog.Context;

namespace ECommerce.API.Serilog;

/// <summary>
/// Extension methods that configure Serilog logging for the application.
/// Pulled out of Program.cs so startup logic stays short and readable —
/// this is the standard place teams put logging setup (same idea as
/// AddPresentation/AddInfrastructure/AddApp for their respective layers).
/// </summary>
public static class SerilogExtensions
{
    /// <summary>
    /// Builds and assigns the global Serilog logger.
    /// Must run BEFORE builder.Build(), so that startup errors
    /// (like DB connection failures) are also captured by Serilog.
    /// </summary>
    public static WebApplicationBuilder AddSerilogLogging(this WebApplicationBuilder builder)
    {
        // Prints Serilog's internal errors to the console (e.g. sink connection failures).
        // Only useful during development — remove or disable in production.
        global::Serilog.Debugging.SelfLog.Enable(msg => Console.WriteLine($"[SERILOG] {msg}"));

        // Build the logger config as a variable instead of one long chained
        // expression, so the PostgreSQL sink can be added conditionally below
        // rather than always required (which is what threw on missing LogsDb).
        var loggerConfig = new LoggerConfiguration()
            .ReadFrom.Configuration(builder.Configuration)
            .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
            .Enrich.FromLogContext()
            .Enrich.WithMachineName()
            .Enrich.WithThreadId()
            .Enrich.WithCorrelationId();

        var logsDbConnectionString = builder.Configuration.GetConnectionString("LogsDb");

        if (!string.IsNullOrWhiteSpace(logsDbConnectionString))
        {
            // Only wire up Postgres logging when a connection string is actually
            // configured. Locally/in a fresh container without LogsDb set, you
            // still get Console logs instead of a hard crash on startup.
            loggerConfig.WriteTo.PostgreSQL(
                connectionString: logsDbConnectionString,
                tableName: "logs",
                columnOptions: ColumnOptionsHelper.GetColumnOptions(),
                needAutoCreateTable: true,
                batchSizeLimit: 50,
                period: TimeSpan.FromSeconds(2));
        }

        Log.Logger = loggerConfig.CreateLogger();

        Log.Information("Application starting. LogsDb connection configured: {IsConfigured}",
            !string.IsNullOrWhiteSpace(logsDbConnectionString));

        builder.Host.UseSerilog();

        return builder;
    }

    /// <summary>
    /// Adds structured HTTP request logging via Serilog (replaces ASP.NET's default request logs).
    /// Logs one entry per request with method, path, status code, and duration.
    /// Must run on the BUILT app (after builder.Build()), since it's middleware.
    /// </summary>
    public static WebApplication UseSerilogRequestLoggingConfigured(this WebApplication app)
    {
        // Adds structured HTTP request logging via Serilog (replaces ASP.NET's default request logs).
        // Logs one entry per request with method, path, status code, and duration.
        app.UseSerilogRequestLogging(options =>
        {
            // Controls what log level each response gets based on status code or exception.
            options.GetLevel = (httpContext, elapsed, ex) =>
            {
                if (ex is not null) return global::Serilog.Events.LogEventLevel.Error;
                if (httpContext.Response.StatusCode >= 500) return global::Serilog.Events.LogEventLevel.Error;
                if (httpContext.Response.StatusCode >= 400) return global::Serilog.Events.LogEventLevel.Warning;
                return global::Serilog.Events.LogEventLevel.Information;
            };

            // Attaches extra properties to the request log entry.
            options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
            {
                diagnosticContext.Set("RequestPath", httpContext.Request.Path);
                diagnosticContext.Set("RequestMethod", httpContext.Request.Method);
                diagnosticContext.Set("UserAgent", httpContext.Request.Headers["User-Agent"].FirstOrDefault() ?? "unknown");

                if (httpContext.User.Identity?.IsAuthenticated == true)
                {
                    var userId = httpContext.User.FindFirst("sub")?.Value;
                    if (!string.IsNullOrEmpty(userId))
                        diagnosticContext.Set("UserId", userId);
                }
            };
        });

        return app;
    }

    /// <summary>
    /// Pushes the current request's TraceIdentifier into Serilog's LogContext,
    /// so EVERY log line written during this request — info, warnings, errors,
    /// including ones from GlobalExceptionMiddleware — carries the SAME TraceId
    /// that gets returned to the client in ApiMeta/ProblemDetails.traceId.
    /// This is what lets you take a traceId the frontend reports and find the
    /// exact request's full log trail in PostgreSQL.
    /// Must run EARLY in the pipeline — before routing/endpoints/exception handling —
    /// so the property is already active when anything downstream logs.
    /// </summary>
    public static WebApplication UseTraceIdEnrichment(this WebApplication app)
    {
        app.Use(async (context, next) =>
        {
            using (LogContext.PushProperty("TraceId", context.TraceIdentifier))
            {
                await next();
            }
        });

        return app;
    }
}
