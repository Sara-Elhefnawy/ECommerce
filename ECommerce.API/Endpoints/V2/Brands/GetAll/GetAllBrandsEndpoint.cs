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
        CancellationToken ct = default)
    {
        var result = await mediator.Send(new GetAllBrandsQuery(), ct);

        return result.ToApiResult(httpContext);
    }
}