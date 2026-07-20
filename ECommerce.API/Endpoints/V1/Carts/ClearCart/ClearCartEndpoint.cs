using ECommerce.API.Common;
using ECommerce.API.Extensions;
using ECommerce.API.Extensions.Abstraction;
using ECommerce.APP.Features.Carts.Commands.ClearCart;
using ECommerce.APP.Features.Carts.Queries.GetCart;
using ECommerce.APP.Mediator;

namespace ECommerce.API.Endpoints.V1.Carts.ClearCart;

public sealed class ClearCartEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
        => app.MapVersionedEndpoint("cart", ApiVersions.V1)
            .MapDelete("/", Handle)
            .WithTags("Cart")
            .WithName("ClearCart")
            .WithGroupName("v1")
            .Produces<ApiResponse<GetCartResponse>>(StatusCodes.Status204NoContent)
            .WithSummary("Clear Cart")
            .WithDescription("Clear the buyer's cart");

    public static async Task<IResult> Handle(
        IMediator mediator,
        HttpContext httpContext,
        CancellationToken ct = default)
    {
        var buyerIdResult = httpContext.GetBuyerId();

        if (buyerIdResult.IsFailure)
            return buyerIdResult.ToApiResult(httpContext, "");

        var result = await mediator.Send(new ClearCartCommand(buyerIdResult.Value), ct);

        return Results.NoContent();
    }
}
