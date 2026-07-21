using ECommerce.API.Common;
using ECommerce.API.Extensions;
using ECommerce.API.Extensions.Abstraction;
using ECommerce.APP.Features.Inventories.Commands.CreateInventory;
using ECommerce.APP.Mediator;

namespace ECommerce.API.Endpoints.V1.Inventories.Create;

public sealed class CreateInventoryEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
        => app.MapVersionedEndpoint("inventories", ApiVersions.V1)
            .MapPost("/", Handle)
            .WithTags("Inventories")
            .WithName("CreateInventory")
            .WithGroupName("v1")
            .Produces<ApiResponse<CreateInventoryResponse>>(StatusCodes.Status201Created)
            .ProducesValidationProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Create inventory")
            .WithDescription("Creates a new inventory record with initial stock for a product");

    public static async Task<IResult> Handle(
        CreateInventoryRequest request,
        IMediator mediator,
        HttpContext httpContext,
        CancellationToken ct = default)
    {
        var command = new CreateInventoryCommand(request.ProductId, request.Quantity);

        var result = await mediator.Send(command, ct);

        return result.ToApiResult(httpContext, "Inventory created successfully");
    }
}
