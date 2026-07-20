using Microsoft.Extensions.DependencyInjection;
using System.Collections.Concurrent;
using System.Reflection;

namespace ECommerce.APP.Mediator;

public sealed class Mediator(IServiceProvider provider) : IMediator
{
    // Cache of compiled delegates.
    // Reflection is used only once for each request type.
    private static readonly ConcurrentDictionary<Type, HandlerInvoker> Invokers = new();
    
    // Reference to the generic InvokeAsync<TRequest,TResponse>() method.
    // Later we close it using MakeGenericMethod().
    private static readonly MethodInfo InvokeAsyncMethod =
        typeof(Mediator).GetMethod(nameof(InvokeAsync), BindingFlags.NonPublic | BindingFlags.Static)!;

    // Example: request = GetAllProductsQuery
    // 1) Find (or build once) a delegate capable of invoking
    //      IRequestHandler<GetAllProductsQuery, ResultOfT<...>>
    // 2) Execute it.
    public Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken ct = default)
    {
        var requestType = request.GetType();
        var invoker = Invokers.GetOrAdd(requestType, _ => BuildInvoker(requestType, typeof(TResponse)));
        return (Task<TResponse>)invoker(provider, request, ct);
    }

    // Uses reflection once to close the generic InvokeAsync method.
    // Before: InvokeAsync<TRequest,TResponse>()
    // After: InvokeAsync<GetAllProductsQuery,
    //             ResultOfT<IReadOnlyList<GetAllProductsResponse>>>()
    // The created delegate is cached forever.
    private static HandlerInvoker BuildInvoker(Type requestType, Type responseType)
    {
        var genericMethod = InvokeAsyncMethod.MakeGenericMethod(requestType, responseType);
        return (sp, request, ct) => (Task)genericMethod.Invoke(null, [sp, request, ct])!;
    }

    // 1. Resolve the handler from DI.
    // 2. Build the pipeline.
    // 3. Execute the pipeline.
    // 4. Return the handler's response.
    private static async Task<TResponse> InvokeAsync<TRequest, TResponse>(
        IServiceProvider sp, TRequest request, CancellationToken ct)
        where TRequest : IRequest<TResponse>
    {
        // Resolve the single handler responsible for this request.
        var handler = sp.GetService<IRequestHandler<TRequest, TResponse>>()
            ?? throw new InvalidOperationException(
                $"No handler registered for '{typeof(TRequest).Name}'. " +
                $"Did you forget to register it, or is it missing an IRequestHandler<{typeof(TRequest).Name}, ...> implementation?");

        // Start with the handler itself.
        // Behaviors will wrap around this delegate.
        RequestHandlerDelegate<TResponse> pipeline = () => handler.Handle(request, ct);

        // Reverse so the first registered behavior becomes the outermost middleware.
        // Validation -> Logging -> Handler
        var behaviors = sp.GetServices<IPipelineBehavior<TRequest, TResponse>>().Reverse();

        // Wrap the current pipeline with another behavior.
        // After wrapping: Validation -> Logging -> Handler
        foreach (var behavior in behaviors)
        {
            var next = pipeline; // capture current pipeline
            pipeline = () => behavior.Handle(request, next, ct);
        }

        return await pipeline();
    }

    private delegate Task HandlerInvoker(IServiceProvider provider, object request, CancellationToken ct);
}
