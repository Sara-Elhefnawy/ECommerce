namespace ECommerce.APP.Mediator.Behaviors;

using ECommerce.APP.Mediator;
using Microsoft.Extensions.Logging;

public sealed class LoggingBehavior<TRequest, TResponse>(
    ILogger<LoggingBehavior<TRequest, TResponse>> logger) : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    public async Task<TResponse> Handle(
    TRequest request,
    RequestHandlerDelegate<TResponse> next,
    CancellationToken ct)
    {
        var requestName = typeof(TRequest).Name;

        logger.LogInformation("Handling {RequestName}", requestName);

        try
        {
            var response = await next();

            logger.LogInformation("Handled {RequestName}", requestName);

            return response;
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "Unhandled exception while handling {RequestName}",
                requestName);

            throw;
        }
    }
}
