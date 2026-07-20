using ECommerce.API.Common;
using ECommerce.API.Extensions;
using ECommerce.API.Extensions.Abstraction;
using ECommerce.APP.Features.Carts.Queries.GetCart;
using ECommerce.APP.Mediator;

namespace ECommerce.API.Endpoints.V1.Carts.GetCart;

public class GetCartEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
        => app.MapVersionedEndpoint("cart", ApiVersions.V1)
            .MapGet("/", Handle)
            .WithTags("Cart")
            .WithName("GetCart")
            .WithGroupName("v1")
            .Produces<ApiResponse<IReadOnlyList<GetCartResponse>>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Get current cart")
            .WithDescription("Returns the current buyer's current cart in cache");

    public static async Task<IResult> Handle(
        IMediator mediator,
        HttpContext httpContext,
        CancellationToken ct = default)
    {
        var buyerIdResult = httpContext.GetBuyerId();

        if (buyerIdResult.IsFailure)
            return buyerIdResult.ToApiResult(httpContext, "");

        var result = await mediator.Send(new GetCartQuery(buyerIdResult.Value), ct);

        return result.ToApiResult(httpContext, "Retrieved cart successfully");
    }
}
