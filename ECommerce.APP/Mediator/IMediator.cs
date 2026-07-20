namespace ECommerce.APP.Mediator;

/// <summary>
/// What your endpoints actually depend on. 
/// They don't know concrete handler runs 
/// they just Send() a request and get a response.
/// </summary>
public interface IMediator
{
    Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken ct = default);
}
