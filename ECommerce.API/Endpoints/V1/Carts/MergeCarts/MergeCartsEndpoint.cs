using ECommerce.API.Common;
using ECommerce.API.Extensions;
using ECommerce.API.Extensions.Abstraction;
using ECommerce.API.Serilog;
using ECommerce.APP.Features.Carts.Commands.MergeGuestCart;
using ECommerce.APP.Features.Carts.Queries.GetCart;
using ECommerce.APP.Mediator;

namespace ECommerce.API.Endpoints.V1.Carts.MergeCarts;

public sealed class MergeCartsEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
        => app.MapVersionedEndpoint("cart", ApiVersions.V1)
            .MapPost("/merge", Handle)
            .WithTags("Cart")
            .WithName("MergeCarts")
            .WithGroupName("v1")
            .Produces<ApiResponse<GetCartResponse>>(StatusCodes.Status200OK)
            .WithSummary("Merge Carts")
            .WithDescription("Merge guest's cart with the buyer's cart");

    public static async Task<IResult> Handle(
        MergeCartsRequest request,
        IMediator mediator,
        HttpContext httpContext,
        CancellationToken ct = default)
    {
        var buyerIdResult = httpContext.GetBuyerId();

        if (buyerIdResult.IsFailure)
            return buyerIdResult.ToApiResult(httpContext, "");

        using (LoggingExtensions.WithCartContext(
            buyerIdResult.Value,
            request.AnonymousBuyerId))
        {
            var command = new MergeCartCommand(
                buyerIdResult.Value,
                request.AnonymousBuyerId);

            var result = await mediator.Send(command, ct);

            return result.ToApiResult(httpContext, "Carts merged successfully");
        }
    }
}
