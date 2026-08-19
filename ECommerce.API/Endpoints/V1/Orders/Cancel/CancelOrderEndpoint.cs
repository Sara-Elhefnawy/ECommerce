using ECommerce.API.Common;
using ECommerce.API.Extensions;
using ECommerce.API.Extensions.Abstraction;
using ECommerce.APP.Features.Orders.Commands.Cancel;
using ECommerce.APP.Features.Orders.DTOs;
using ECommerce.APP.Mediator;
using ECommerce.Domain.Constants;

namespace ECommerce.API.Endpoints.V1.Orders.Cancel;

public sealed class CancelOrderEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
        => app.MapVersionedEndpoint("orders", ApiVersions.V1)
            .MapPut("/{id:guid}/cancel", Handle)
            .WithTags("Orders")
            .WithName("cancelOrder")
            .WithGroupName("v1")
            .Produces<ApiResponse<OrderResponse>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status409Conflict)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Cancel order by id")
            .WithDescription("Cancel the order by id.")
            .RequireAuthorization(policy => policy.RequireRole(Roles.User));

    public async Task<IResult> Handle(
        Guid id,
        IMediator mediator,
        HttpContext httpContext,
        CancellationToken ct)
    {
        var result = await mediator.Send(new CancelOrderCommand(id), ct);

        return result.ToApiResult(httpContext, "Order canceled successfully");
    }
}
