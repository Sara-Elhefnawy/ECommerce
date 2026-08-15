using ECommerce.API.Extensions;
using ECommerce.API.Extensions.Abstraction;
using ECommerce.APP.Features.DeliveryMethods.Commands.Reorder;
using ECommerce.APP.Mediator;
using ECommerce.Domain.Constants;
using FluentValidation;

namespace ECommerce.API.Endpoints.V1.DeliveryMethods.Reorder;

public sealed class ReorderDeliveryMethodsEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
        => app.MapVersionedEndpoint("delivery-methods", ApiVersions.V1)
            .MapPut("/reorder", Handle)
            .WithTags("Delivery Methods")
            .WithName("ReorderDeliveryMethods")
            .WithGroupName("v1")
            .Produces(StatusCodes.Status200OK)
            .ProducesValidationProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Reorder delivery methods")
            .WithDescription("Updates the display order of delivery methods according to the supplied ID sequence.")
            .RequireAuthorization(policy => policy.RequireRole(Roles.Manager));

    public static async Task<IResult> Handle(
        ReorderDeliveryMethodsRequest request,
        IValidator<ReorderDeliveryMethodsRequest> validator,
        IMediator mediator,
        HttpContext httpContext,
        CancellationToken ct = default)
    {
        var validation = await validator.ValidateAsync(request, ct);

        if (!validation.IsValid)
            return Results.ValidationProblem(validation.ToDictionary());

        var deliveryMethodIds = request.DeliveryMethodIds
            .Select(Guid.Parse)
            .ToList();

        var command = new ReorderDeliveryMethodsCommand(deliveryMethodIds);

        var result = await mediator.Send(command, ct);

        return result.ToApiResult(httpContext,"Delivery methods reordered successfully");
    }
}
