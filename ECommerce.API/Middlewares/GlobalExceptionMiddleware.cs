using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Middlewares;

// without globalexceptionmiddleware the FE will see unfriendly stacktrace with every error
// implemented with every 500 status || any throw
public class GlobalExceptionMiddleware(IProblemDetailsService problemDetails, ILogger<GlobalExceptionMiddleware> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext, 
        Exception exception, 
        CancellationToken ct)
    {
        logger.LogError(exception, "Unhandled exception");

        httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;

        var problemDetailsContext = new ProblemDetailsContext
        {
            HttpContext = httpContext,
            Exception = exception,
            ProblemDetails = new ProblemDetails
            {
                Status = StatusCodes.Status500InternalServerError,
                Title = "Internal Server Error",
                Detail = "An exception error occured. Please try again later."
            }
        };

        return await problemDetails.TryWriteAsync(problemDetailsContext);
    }
}
