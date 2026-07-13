using ECommerce.API.Common;
using ECommerce.API.Extensions;
using ECommerce.API.Extensions.Abstraction;
using ECommerce.API.Result;
using ECommerce.APP.Abstractions.Mediator;
using ECommerce.APP.Features.Brands.Queries.GetAll;

namespace ECommerce.API.Endpoints.V2.Brands.GetAll;

public class GetAllBrandsEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
        => app.MapGet("/brands/v2", Handle)
            .WithName("GetBrandsV2")
            .WithGroupName("v2")
            .Produces<ApiResponse<IReadOnlyList<GetAllBrandsResponse>>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Get brands V2")
            .WithDescription("Returns all brands in DB with additional fields");

    public static async Task<IResult> Handle(
        IMediator mediator,
        HttpContext httpContext,
        ILogger<GetAllBrandsEndpoint> logger,
        CancellationToken ct = default)
    {
        logger.LogInformation("Retrieving all brands from database (V2)");

        var result = await mediator.Send(new GetAllBrandsQuery(), ct);

        logger.LogInformation("Query completed with result: {Result}", result);

        return result.ToApiResult(httpContext);
    }
}