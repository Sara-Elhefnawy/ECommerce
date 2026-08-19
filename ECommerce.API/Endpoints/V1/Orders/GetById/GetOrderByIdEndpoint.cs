using ECommerce.API.Common;
using ECommerce.API.Extensions;
using ECommerce.API.Extensions.Abstraction;
using ECommerce.API.Serilog;
using ECommerce.APP.Features.Orders.DTOs;
using ECommerce.APP.Features.Orders.Queries.GetById;
using ECommerce.APP.Mediator;
using ECommerce.Domain.Constants;

namespace ECommerce.API.Endpoints.V1.Orders.GetById;

public sealed class GetOrderByIdEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
        => app.MapVersionedEndpoint("orders", ApiVersions.V1)
            .MapGet("/{id:guid}", Handle)
            .WithTags("Orders")
            .WithName("getOrderById")
            .WithGroupName("v1")
            .Produces<ApiResponse<OrderResponse>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Get order by id")
            .WithDescription("get the order by id.")
            .RequireAuthorization(policy => policy.RequireRole(Roles.User));

    public async Task<IResult> Handle(
        Guid id,
        IMediator mediator,
        HttpContext httpContext,
        CancellationToken ct)
    {
        var result = await mediator.Send(new GetOrderByIdQuery(id), ct);

        if (result.IsFailure)
            return result.ToApiResult(httpContext, "");

        using (LoggingExtensions.WithOrderContext(id))
        {
            return result.ToApiResult(httpContext, "Retrieved order successfully");
        }
    }
}
