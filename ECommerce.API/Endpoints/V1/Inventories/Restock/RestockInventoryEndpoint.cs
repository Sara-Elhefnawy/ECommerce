using ECommerce.API.Common;
using ECommerce.API.Extensions;
using ECommerce.API.Extensions.Abstraction;
using ECommerce.APP.Features.Inventories.Commands.Restock;
using ECommerce.APP.Mediator;
using ECommerce.Domain.Constants;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Endpoints.V1.Inventories.Restock;

public sealed class RestockInventoryEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
        => app.MapVersionedEndpoint("inventories", ApiVersions.V1)
            .MapPut("/{productId:guid}/restock", Handle)
            .WithTags("Inventories")
            .WithName("RestockInventory")
            .WithGroupName("v1")
            .Produces<ApiResponse<RestockInventoryResponse>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesValidationProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Restock inventory")
            .WithDescription("Adds quantity to an existing inventory record for a product.")
            .RequireAuthorization(policy => policy.RequireRole(Roles.Manager));

    public static async Task<IResult> Handle(
        [FromBody] RestockInventoryRequest request,
        IMediator mediator,
        HttpContext httpContext,
        CancellationToken ct)
    {
        var result = await mediator.Send(new RestockInventoryCommand(request.ProductId, request.Quantity), ct);

        return result.ToApiResult(httpContext, "Inventory restocked successfully");
    }
}
