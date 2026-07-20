namespace ECommerce.APP.Mediator;

/// <summary>
/// Marker interface for anything sendable through the mediator
///     a Query OR a Command. 
/// TResponse is whatever the handler ultimately returns
///     (usually one of your ResultOfT<T> / Result types).
/// </summary>
public interface IRequest<TResponse>
{
}
