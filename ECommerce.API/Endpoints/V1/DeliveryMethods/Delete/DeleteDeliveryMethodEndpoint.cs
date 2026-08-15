using ECommerce.API.Extensions;
using ECommerce.API.Extensions.Abstraction;
using ECommerce.API.Serilog;
using ECommerce.APP.Features.DeliveryMethods.Commands.Delete;
using ECommerce.APP.Mediator;
using ECommerce.Domain.Constants;

namespace ECommerce.API.Endpoints.V1.DeliveryMethods.Delete;

public sealed class DeleteDeliveryMethodEndpoint : IEndpoint
{
    private const string Version = ApiVersions.V1;

    public void MapEndpoint(IEndpointRouteBuilder app)
        => app.MapVersionedEndpoint("delivery-methods", Version)
            .MapDelete("/{id:guid}", Handle)
            .WithTags("Delivery Methods")
            .WithName("DeleteDeliveryMethod")
            .WithGroupName("v1")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Delete delivery method")
            .WithDescription("Deletes an existing delivery method.")
            .RequireAuthorization(
                policy => policy.RequireRole(Roles.Manager));

    public static async Task<IResult> Handle(
        Guid id,
        IMediator mediator,
        HttpContext httpContext,
        CancellationToken ct = default)
    {
        using (LoggingExtensions.WithDeliveryMethodContext(id))
        {
            var command = new DeleteDeliveryMethodCommand(id);

            var result = await mediator.Send(command, ct);

            if (result.IsFailure)
                return result.ToApiResult(httpContext, "");

            return Results.NoContent();
        }
    }
}
