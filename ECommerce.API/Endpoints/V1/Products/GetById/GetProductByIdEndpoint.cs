using ECommerce.API.Common;
using ECommerce.API.Extensions;
using ECommerce.API.Extensions.Abstraction;
using ECommerce.API.Result;
using ECommerce.APP.Abstractions.Mediator;
using ECommerce.APP.Features.Products.Queries.GetById;
using Serilog.Context;

namespace ECommerce.API.Endpoints.V1.Products.Details;

public class GetProductByIdEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
        => app.MapVersionedEndpoint("products/", ApiVersions.V1)
            .MapGet("/{id:guid}", Handle)
            .WithTags("Products")
            .WithName("GetProductByID")
            .WithGroupName("v1")
            .Produces<ApiResponse<IReadOnlyList<GetProductByIdResponse>>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Get products")
            .WithDescription("Returns all products in DB, or 404 if list is empty");

    public static async Task<IResult> Handle(
        Guid id,
        IMediator mediator,
        HttpContext httpContext,
        ILogger<GetProductByIdEndpoint> logger,
        CancellationToken ct = default
        )
    {
        using (LogContext.PushProperty("ProductId", id))
        {
            logger.LogInformation("Retrieving product with ID {Id} from database", id);

            var result = await mediator.Send(new GetProductByIdQuery(id), ct);

            logger.LogInformation("Query completed with result: {Result}", result);

            return result.ToApiResult(httpContext);
        }
    }
}

