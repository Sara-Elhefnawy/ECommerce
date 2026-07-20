using ECommerce.API.Common;
using ECommerce.API.Extensions;
using ECommerce.API.Extensions.Abstraction;
using ECommerce.APP.Features.Carts.Commands.AddItemToCart;
using ECommerce.APP.Features.Carts.Queries.GetCart;
using ECommerce.APP.Mediator;

namespace ECommerce.API.Endpoints.V1.Carts.AddItem;

public sealed class AddItemEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
        => app.MapVersionedEndpoint("cart", ApiVersions.V1)
            .MapPost("/items", Handle)
            .WithTags("Cart")
            .WithName("AddCartItem")
            .WithGroupName("v1")
            .Produces<ApiResponse<GetCartResponse>>(StatusCodes.Status200OK)
            .ProducesValidationProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Add Item to Cart")
            .WithDescription("Adds a product to the buyer's cart");

    public static async Task<IResult> Handle(
        AddItemRequest request,
        IMediator mediator,
        HttpContext httpContext,
        CancellationToken ct = default)
    {
        var buyerIdResult = httpContext.GetBuyerId();

        if (buyerIdResult.IsFailure)
            return buyerIdResult.ToApiResult(httpContext, "");

        var command = new AddCartItemCommand(
            buyerIdResult.Value,
            request.ProductId,
            request.Quantity);

        var result = await mediator.Send(command, ct);

        return result.ToApiResult(httpContext, "Item added to cart");
    }
}
