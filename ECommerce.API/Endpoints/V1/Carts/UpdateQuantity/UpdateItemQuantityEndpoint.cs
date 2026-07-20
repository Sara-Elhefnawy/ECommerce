using ECommerce.API.Common;
using ECommerce.API.Extensions;
using ECommerce.API.Extensions.Abstraction;
using ECommerce.APP.Features.Carts.Commands.UpdateQuantity;
using ECommerce.APP.Features.Carts.Queries.GetCart;
using ECommerce.APP.Mediator;

namespace ECommerce.API.Endpoints.V1.Carts.UpdateQuantity;

public sealed class UpdateItemQuantityEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
        => app.MapVersionedEndpoint("cart", ApiVersions.V1)
            .MapPut("/items/{productId:guid}", Handle)
            .WithTags("Cart")
            .WithName("UpdateCartItem")
            .WithGroupName("v1")
            .Produces<ApiResponse<GetCartResponse>>(StatusCodes.Status200OK)
            .ProducesValidationProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Update cart item quantity")
            .WithDescription("Update a product's quantity in the buyer's cart");

    public static async Task<IResult> Handle(
        UpdateItemQuantityRequest request,
        IMediator mediator,
        HttpContext httpContext,
        CancellationToken ct = default)
    {
        var buyerIdResult = httpContext.GetBuyerId();

        if (buyerIdResult.IsFailure)
            return buyerIdResult.ToApiResult(httpContext, "");

        var command = new UpdateCartItemQuantityCommand(
            buyerIdResult.Value,
            request.ProductId,
            request.Quantity);

        var result = await mediator.Send(command, ct);

        return result.ToApiResult(httpContext, "Cart updated successfully");
    }
}
