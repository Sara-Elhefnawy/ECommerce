using NpgsqlTypes;
using Serilog.Sinks.PostgreSQL;

namespace ECommerce.API.Serilog;

// Defines the exact schema of the PostgreSQL "logs" table.
// Each entry maps a column name (in the DB) to a writer that knows
// how to extract that value from a Serilog log event.
// This is passed to WriteTo.PostgreSQL(...) in Program.cs.
// If you change this, DROP the logs table so it gets recreated with the new schema.
public static class ColumnOptionsHelper
{
    public static IDictionary<string, ColumnWriterBase> GetColumnOptions()
    {
        return new Dictionary<string, ColumnWriterBase>
        {
            // Core Serilog fields — always include these.
            { "message",          new RenderedMessageColumnWriter() },           // final rendered log message
            { "message_template", new MessageTemplateColumnWriter() },           // raw template e.g. "User {UserId} logged in"
            { "level",            new LevelColumnWriter() },                     // log level as integer (0=Verbose, 1=Debug, 2=Info, 3=Warn, 4=Error, 5=Fatal)
            { "timestamp",        new TimestampColumnWriter() },                 // when the log was created
            { "exception",        new ExceptionColumnWriter() },                 // full exception stack trace (null if no exception)
            { "properties",       new PropertiesColumnWriter(NpgsqlDbType.Jsonb) }, // all structured properties as JSON

            // Enricher fields — these are populated by the .Enrich.With...() calls in Program.cs.
            { "application",      new SinglePropertyColumnWriter("Application",   PropertyWriteMethod.Raw, NpgsqlDbType.Text) },    // from appsettings "Properties": { "Application": "..." }
            { "machine_name",     new SinglePropertyColumnWriter("MachineName",   PropertyWriteMethod.Raw, NpgsqlDbType.Text) },    // from .Enrich.WithMachineName()
            { "thread_id",        new SinglePropertyColumnWriter("ThreadId",      PropertyWriteMethod.Raw, NpgsqlDbType.Integer) }, // from .Enrich.WithThreadId()
            { "correlation_id",   new SinglePropertyColumnWriter("CorrelationId", PropertyWriteMethod.Raw, NpgsqlDbType.Text) },    // from .Enrich.WithCorrelationId()

            // NEW: dedicated column for the request TraceId, pushed via LogContext in
            // UseTraceIdEnrichment(). This is the SAME value returned to the client in
            // ApiMeta.TraceId / ProblemDetails.traceId — so when a user reports a
            // traceId, you query this column directly to pull every log line for
            // that exact request, instead of digging through the properties JSONB.
            { "trace_id",         new SinglePropertyColumnWriter("TraceId",       PropertyWriteMethod.Raw, NpgsqlDbType.Text) },

            // Request context fields — populated by LogContext.PushProperty(...) or DiagnosticContext.
            { "user_id",          new SinglePropertyColumnWriter("UserId",        PropertyWriteMethod.Raw, NpgsqlDbType.Text) },    // set when user is authenticated
            { "request_path",     new SinglePropertyColumnWriter("RequestPath",   PropertyWriteMethod.Raw, NpgsqlDbType.Text) },    // e.g. /api/products
            { "request_method",   new SinglePropertyColumnWriter("RequestMethod", PropertyWriteMethod.Raw, NpgsqlDbType.Text) },    // e.g. GET, POST
            { "status_code",      new SinglePropertyColumnWriter("StatusCode",    PropertyWriteMethod.Raw, NpgsqlDbType.Integer) }, // e.g. 200, 404, 500
            { "elapsed_ms",       new SinglePropertyColumnWriter("ElapsedMs",     PropertyWriteMethod.Raw, NpgsqlDbType.Double) },  // how long the request took
        };
    }
}
