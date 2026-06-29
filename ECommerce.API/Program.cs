using ECommerce.API;
using ECommerce.API.Endpoints;
using ECommerce.API.Serilog;
using ECommerce.APP;
using ECommerce.Domain.Abstractions.Repositories;
using ECommerce.Infrastructure;
using ECommerce.Infrastructure.Persistent;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Prints Serilog's internal errors to the console (e.g. sink connection failures).
// Only useful during development — remove or disable in production.
Serilog.Debugging.SelfLog.Enable(msg => Console.WriteLine($"[SERILOG] {msg}"));

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

builder.Services.AddPresentation()
                .AddInfrastructure(builder.Configuration)
                .AddApp();

builder.Services.AddOpenApi();

var app = builder.Build();

// Adds structured HTTP request logging via Serilog (replaces ASP.NET's default request logs).
// Logs one entry per request with method, path, status code, and duration.
app.UseSerilogRequestLogging(options =>
{
    // Controls what log level each response gets based on status code or exception.
    options.GetLevel = (httpContext, elapsed, ex) =>
    {
        if (ex is not null) return Serilog.Events.LogEventLevel.Error;
        if (httpContext.Response.StatusCode >= 500) return Serilog.Events.LogEventLevel.Error;
        if (httpContext.Response.StatusCode >= 400) return Serilog.Events.LogEventLevel.Warning;
        return Serilog.Events.LogEventLevel.Information;
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

// Activates the GlobalExceptionMiddleware registered in AddPresentation().
// Catches any unhandled exception and returns a structured ProblemDetails 500 response.
app.UseExceptionHandler();

// Temporary DI sanity check — confirms IUnitOfWork is registered correctly.
// Safe to remove once you're confident the setup works.
using (var scope = app.Services.CreateScope())
{
    var unitOfWork = scope.ServiceProvider.GetService<IUnitOfWork>();
    Console.WriteLine(unitOfWork is null
        ? "IUnitOfWork is NOT registered!"
        : "IUnitOfWork is registered!");
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options =>
        options.SwaggerEndpoint("/openapi/v1.json", "ECommerce API v1"));
}

if (app.Environment.IsDevelopment())
{
    await using var scope = app.Services.CreateAsyncScope();
    var seeder = scope.ServiceProvider.GetRequiredService<DatabaseSeeder>();
    var dbContext = scope.ServiceProvider.GetRequiredService<ECommerceDbContext>();

    // Applies any pending EF Core migrations on startup in Development.
    await dbContext.Database.MigrateAsync();
    // Seeds initial data (brands, types, products) if the tables are empty.
    await seeder.SeedAllAsync();
}

app.MapProductEndpoints();

app.Run();
