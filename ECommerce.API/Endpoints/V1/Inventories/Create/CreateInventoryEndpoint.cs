using ECommerce.API.Common;
using ECommerce.API.Extensions;
using ECommerce.API.Extensions.Abstraction;
using ECommerce.API.Serilog;
using ECommerce.APP.Features.Inventories.Commands.CreateInventory;
using ECommerce.APP.Mediator;

namespace ECommerce.API.Endpoints.V1.Inventories.Create;

public sealed class CreateInventoryEndpoint : IEndpoint
{
    private const string Version = ApiVersions.V1;

    public void MapEndpoint(IEndpointRouteBuilder app)
        => app.MapVersionedEndpoint("inventories", Version)
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
        using (LoggingExtensions.WithInventoryContext(request.ProductId))
        {
            var command = new CreateInventoryCommand(request.ProductId, request.Quantity);

            var result = await mediator.Send(command, ct);

            // Pass location for 201 Created response
            var location = result.IsSuccess
                ? $"/api/{Version}/inventories/{request.ProductId}"
                : null;

            return result.ToApiResult(httpContext, "Inventory created successfully", location);
        }
    }
}
