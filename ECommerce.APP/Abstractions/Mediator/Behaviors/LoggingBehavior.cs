namespace ECommerce.APP.Abstractions.Mediator.Behaviors;
using Microsoft.Extensions.Logging;

public sealed class LoggingBehavior<TRequest, TResponse>(
    ILogger<LoggingBehavior<TRequest, TResponse>> logger) : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken ct)
    {
        logger.LogInformation("Retrieving the {RequestName} from database", typeof(TRequest).Name);
        var result = await next();
        logger.LogInformation("Query completed with result: {RequestName}", typeof(TRequest).Name);
        return result;
    }
}
