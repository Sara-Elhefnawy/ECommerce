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

        // Create the global Serilog logger before the app starts,
        // so startup errors (like DB connection failures) are also captured.
        // ReadFrom.Configuration pulls MinimumLevel and Enrich settings from appsettings.json.
        // Sinks (Console + PostgreSQL) are configured in code because PostgreSQL
        // requires a complex columnOptions object that can't be expressed in JSON.
        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(builder.Configuration)
            .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
            .WriteTo.PostgreSQL(
                connectionString: builder.Configuration.GetConnectionString("LogsDb")
                    ?? throw new InvalidOperationException("LogsDb connection string not found"),
                tableName: "logs",
                columnOptions: ColumnOptionsHelper.GetColumnOptions(), // custom column schema
                needAutoCreateTable: true,   // creates the table if it doesn't exist
                batchSizeLimit: 50,          // flush after 50 log entries accumulate
                period: TimeSpan.FromSeconds(2) // or flush every 2 seconds, whichever comes first
            )
            .Enrich.FromLogContext()      // picks up properties pushed via LogContext.PushProperty(...)
            .Enrich.WithMachineName()    // adds MachineName to every log entry
            .Enrich.WithThreadId()       // adds ThreadId to every log entry
            .Enrich.WithCorrelationId()  // adds CorrelationId (from HTTP headers) to every log entry
            .CreateLogger();

        Log.Information("Application starting. PostgreSQL connection string: {ConnectionString}",
            builder.Configuration.GetConnectionString("LogsDb"));

        // Replace the default .NET logging pipeline with Serilog.
        // All ILogger<T> injections throughout the app will now route through Serilog.
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
                diagnosticContext.Set("UserAgent", httpContext.Request.Headers["User-Agent"].FirstOrDefault());

                if (httpContext.User.Identity?.IsAuthenticated == true)
                    diagnosticContext.Set("UserId", httpContext.User.FindFirst("sub")?.Value);
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
