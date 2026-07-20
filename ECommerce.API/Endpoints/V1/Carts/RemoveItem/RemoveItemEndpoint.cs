using ECommerce.API.Extensions;
using ECommerce.API.Extensions.Abstraction;
using ECommerce.APP.Features.Carts.Commands.RemoveItem;
using ECommerce.APP.Mediator;

namespace ECommerce.API.Endpoints.V1.Carts.RemoveItem;

public sealed class RemoveItemEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
        => app.MapVersionedEndpoint("cart", ApiVersions.V1)
            .MapDelete("/items/{productId:guid}", Handle)
            .WithTags("Cart")
            .WithName("RemoveCartItem")
            .WithGroupName("v1")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesValidationProblem(StatusCodes.Status404NotFound)
            .WithSummary("Remove item from cart")
            .WithDescription("Remove item from cart in the buyer's cart");

    public static async Task<IResult> Handle(
        Guid productId,
        IMediator mediator,
        HttpContext httpContext,
        CancellationToken ct = default)
    {
        var buyerIdResult = httpContext.GetBuyerId();

        if (buyerIdResult.IsFailure)
            return buyerIdResult.ToApiResult(httpContext, "");

        var result = await mediator.Send(new RemoveCartItemCommand(
                buyerIdResult.Value,
                productId), ct);

        return Results.NoContent();
    }
}
