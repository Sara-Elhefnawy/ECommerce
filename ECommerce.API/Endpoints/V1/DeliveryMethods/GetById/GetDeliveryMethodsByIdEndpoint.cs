using ECommerce.API.Extensions;
using ECommerce.API.Extensions.Abstraction;
using ECommerce.API.Serilog;
using ECommerce.APP.Features.DeliveryMethods.Queries.GetById;
using ECommerce.APP.Mediator;
using ECommerce.Domain.Constants;

namespace ECommerce.API.Endpoints.V1.DeliveryMethods.GetById;

public sealed class GetDeliveryMethodsByIdEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
        => app.MapVersionedEndpoint("delivery-methods", ApiVersions.V1)
            .MapGet("/{id:guid}", Handle)
            .WithTags("Delivery Methods")
            .WithName("GetDeliveryMethodById")
            .WithGroupName("v1")
            .Produces(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Get delivery method by id")
            .WithDescription("Gets the delivery methods by id if exists.")
            .RequireAuthorization(policy => policy.RequireRole(Roles.Manager));

    public static async Task<IResult> Handle(
        Guid id,
        IMediator mediator,
        HttpContext httpContext,
        CancellationToken ct = default)
    {
        var result = await mediator.Send(new GetDeliveryMethodByIdQuery(id), ct);

        using (LoggingExtensions.WithDeliveryMethodContext(id))
        {
            return result.ToApiResult(httpContext, "Delivery method's id data retrieved successfully");
        }
    }
}
