using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace ECommerce.APP.Abstractions.Mediator;

public static class MediatorServiceCollectionExtensions
{
    /// <summary>
    /// Registers IMediator itself, then scans the given assembly for every
    /// class implementing IRequestHandler&lt;,&gt; and registers each one
    /// against its specific closed generic interface.
    ///
    /// Example: CreateProductCommand + CreateProductHandler in this assembly
    /// gets registered as:
    ///   services.AddScoped(
    ///       typeof(IRequestHandler<CreateProductCommand, ResultOfT<CreateProductResponse>>),
    ///       typeof(CreateProductHandler));
    /// — automatically, without you writing that line by hand for every
    /// single Query/Command you create. 
    /// This is the main thing you'd lose by NOT using MediatR's package — 
    /// so we're rebuilding just this part.
    /// </summary>
    public static IServiceCollection AddMediatorPattern(this IServiceCollection services, Assembly assembly)
    {
        services.AddScoped<IMediator, Mediator>();

        var handlerInterfaceType = typeof(IRequestHandler<,>);

        var handlerRegistrations =
            from type in assembly.GetTypes()
            where !type.IsAbstract && !type.IsInterface
            from @interface in type.GetInterfaces()
            where @interface.IsGenericType && @interface.GetGenericTypeDefinition() == handlerInterfaceType
            select (Interface: @interface, Implementation: type);

        foreach (var (@interface, implementation) in handlerRegistrations)
        {
            services.AddScoped(@interface, implementation);
        }

        return services;
    }
}
