namespace ECommerce.API.Filters;

public sealed class AuditEndpointFilter(ILogger<AuditEndpointFilter> logger) : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(
        EndpointFilterInvocationContext context,
        EndpointFilterDelegate next)
    {
        // Read who's calling BEFORE next()
        // HttpContext.User is populated by auth middleware earlier in the pipeline,
        //      so it's already available here.
        var userId = context.HttpContext.User.FindFirst("sub")?.Value ?? "anonymous";

        // DisplayName reflects whatever you set via .WithName(...) on the endpoint,
        var endpointName = context.HttpContext.GetEndpoint()?.DisplayName;

        // Run the actual endpoint handler (and any filters registered after this one).
        var result = await next(context);

        // statusResult.StatusCode reads the status directly off the IResult object our handler
        // already returned
        //      that value was set the moment the handler built the result (Results.Json(..., statusCode: 404)),
        //      so it's available to us immediately, before this filter even finishes running.
        var statusCode = result is IStatusCodeHttpResult statusResult
            ? statusResult.StatusCode
            : StatusCodes.Status200OK;

        logger.LogInformation(
            "User {UserId} executed {EndpointName} with status code {StatusCode}",
            userId,
            endpointName,
            statusCode);

        return result;
    }
}
