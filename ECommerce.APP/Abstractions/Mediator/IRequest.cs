namespace ECommerce.APP.Abstractions.Mediator;

/// <summary>
/// Marker interface for anything sendable through the mediator
///     a Query OR a Command. 
/// TResponse is whatever the handler ultimately returns
///     (usually one of your ResultOfT<T> / Result types).
/// </summary>
public interface IRequest<TResponse>
{
}

/// <summary>
/// The handler that knows how to process one specific TRequest and produce a TResponse. 
/// Exactly one handler should exist per TRequest 
/// </summary>
public interface IRequestHandler<in TRequest, TResponse> where TRequest : IRequest<TResponse>
{
    Task<TResponse> Handle(TRequest request, CancellationToken ct = default);
}