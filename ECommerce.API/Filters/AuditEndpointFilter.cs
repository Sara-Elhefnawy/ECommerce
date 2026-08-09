using ECommerce.API.Extensions;

namespace ECommerce.API.Filters;

public sealed class AuditEndpointFilter(ILogger<AuditEndpointFilter> logger) : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(
        EndpointFilterInvocationContext context,
        EndpointFilterDelegate next)
    {
        // "anonymous" covers both "no token" and "token present
        // good enough for a who-did-what audit trail
        var userId = context.HttpContext.User.GetUserIdOrAnonymous();

        var endpointName = context.HttpContext.GetEndpoint()?.DisplayName;

        try
        {
            var result = await next(context);

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
        catch (Exception ex)
        {
            logger.LogWarning(
                ex,
                "User {UserId} executed {EndpointName} and threw {ExceptionType}",
                userId,
                endpointName,
                ex.GetType().Name);

            throw;
        }
    }
}
