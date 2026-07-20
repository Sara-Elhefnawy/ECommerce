namespace ECommerce.APP.Mediator;

/// <summary>
/// A middleware that runs before and/or after the request handler.
/// Pipeline behaviors handle shared system tasks automatically so the core business logic stays clean.
/// These shared tasks include:
/// - Validation
/// - Logging
/// - Caching
/// - Authorization
/// - Transactions
/// </summary>
public interface IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
{
    Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken ct);
}

/// <summary>
/// Delegate representing the next step in the pipeline.
/// Calling next() executes either:
/// - the next behavior
/// - or, if no behaviors remain, the actual request handler.
/// </summary>
public delegate Task<TResponse> RequestHandlerDelegate<TResponse>();