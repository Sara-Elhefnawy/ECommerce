namespace ECommerce.APP.Abstractions.Mediator;

/// <summary>
/// The handler that knows how to process one specific TRequest and produce a TResponse. 
/// Exactly one handler should exist per TRequest 
/// </summary>
public interface IRequestHandler<in TRequest, TResponse> where TRequest : IRequest<TResponse>
{
    Task<TResponse> Handle(TRequest request, CancellationToken ct = default);
}
