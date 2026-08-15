using ECommerce.API.Extensions;
using ECommerce.API.Extensions.Abstraction;
using ECommerce.APP.Features.DeliveryMethods.Queries.GetAll;
using ECommerce.APP.Mediator;
using ECommerce.Domain.Constants;
using FluentValidation;

namespace ECommerce.API.Endpoints.V1.DeliveryMethods.GetAll;

public sealed class GetAllDeliveryMethodsEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
        => app.MapVersionedEndpoint("delivery-methods", ApiVersions.V1)
            .MapGet("/", Handle)
            .WithTags("Delivery Methods")
            .WithName("GetAllDeliveryMethods")
            .WithGroupName("v1")
            .Produces(StatusCodes.Status200OK)
            .WithSummary("Get all delivery methods")
            .WithDescription("Gets the delivery methods with searching or availability.")
            .RequireAuthorization(policy => policy.RequireRole(Roles.Manager));

    public static async Task<IResult> Handle(
        [AsParameters] GetAllDeliveryMethodsRequest request,
        IMediator mediator,
        HttpContext httpContext,
        CancellationToken ct = default)
    {
        var result = await mediator.Send(new GetAllDeliveryMethodsQuery(request.AvailableOnly, request.SearchTerm), ct);

        return result.ToApiResult(httpContext, "Delivery methods retrieved successfully");
    }
}
