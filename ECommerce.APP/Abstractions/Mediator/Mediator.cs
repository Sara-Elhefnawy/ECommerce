using System.Collections.Concurrent;

namespace ECommerce.APP.Abstractions.Mediator;

/// <summary>
/// Default IMediator implementation. 
/// How it works:
/// 1. We know the CONCRETE type of the incoming request at runtime
///    (e.g. GetAllProductsQuery), even though we only see it as
///    IRequest&lt;TResponse&gt; at compile time.
/// 2. We build the matching handler's interface type:
///    IRequestHandler&lt;GetAllProductsQuery, ResultOfT&lt;...&gt;&gt;
/// 3. We ask the DI container (IServiceProvider) for whatever's
///    registered against that exact interface — this only works because
///    AddMediatorPattern() (see MediatorServiceCollectionExtensions.cs)
///    registered every handler against its specific closed generic
///    interface at startup.
/// 4. We invoke Handle(...) on it via a cached compiled delegate, so we
///    only pay the reflection cost ONCE per request TYPE, not once per call.
/// </summary>
public sealed class Mediator(IServiceProvider provider) : IMediator
{
    // Caches the "how do I call Handle() on this handler type" logic per request type,
    //      so repeated Sends of the same request type don't re-run reflection every single time
    private static readonly ConcurrentDictionary<Type, HandlerInvoker> Invokers = new();

    public Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken ct = default)
    {
        var requestType = request.GetType();

        var invoker = Invokers.GetOrAdd(requestType, _ => BuildInvoker(requestType, typeof(TResponse)));

        return (Task<TResponse>)invoker(provider, request, ct);
    }

    private static HandlerInvoker BuildInvoker(Type requestType, Type responseType)
    {
        // Build the closed generic handler interface, e.g.
        //      IRequestHandler<GetAllProductsQuery, ResultOfT<IReadOnlyList<GetAllProductsResponse>>>
        //
        // IMPORTANT: typeof(IRequestHandler<,>) — with an EMPTY comma inside
        // the angle brackets — is the OPEN generic type definition. That's
        // the only kind of Type MakeGenericType can be called on; it fills
        // the empty slots with requestType/responseType.
        //
        // typeof(IRequestHandler<IRequest<object>, object>) would instead be
        // a CLOSED, fully-constructed type (every slot already filled with
        // IRequest<object> and object) — there'd be nothing left for
        // MakeGenericType to plug requestType/responseType into, which is
        // exactly why that version threw "not a GenericTypeDefinition".
        var handlerInterfaceType = typeof(IRequestHandler<,>).MakeGenericType(requestType, responseType);

        var handleMethod = handlerInterfaceType.GetMethod(nameof(IRequestHandler<IRequest<object>, object>.Handle))
            ?? throw new InvalidOperationException($"Handle method not found on {handlerInterfaceType}");

        return (sp, request, ct) =>
        {
            var handler = sp.GetService(handlerInterfaceType)
                ?? throw new InvalidOperationException(
                    $"No handler registered for '{requestType.Name}'. " +
                    $"Did you forget to register it, or is it missing an IRequestHandler<{requestType.Name}, ...> implementation?");

            return (Task)handleMethod.Invoke(handler, [request, ct])!;
        };
    }

    private delegate Task HandlerInvoker(IServiceProvider provider, object request, CancellationToken ct);
}
