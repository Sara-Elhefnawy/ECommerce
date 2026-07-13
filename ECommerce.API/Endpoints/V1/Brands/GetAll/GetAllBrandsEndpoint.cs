using ECommerce.API.Common;
using ECommerce.API.Extensions;
using ECommerce.API.Extensions.Abstraction;
using ECommerce.API.Result;
using ECommerce.APP.Abstractions.Mediator;
using ECommerce.APP.Features.Brands.Queries.GetAll;

namespace ECommerce.API.Endpoints.V1.Brands.GetAll;

public class GetAllBrandsEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
        => app.MapVersionedEndpoint("brands", ApiVersions.V1)
            .MapGet("/", Handle)
            .WithTags("Brands")
            .WithName("GetBrands")
            .WithGroupName("v1")
            .Produces<ApiResponse<IReadOnlyList<GetAllBrandsResponse>>>(StatusCodes.Status200OK)
            .WithSummary("Get brands")
            .WithDescription("Returns all brands in DB");

    public static async Task<IResult> Handle(
        IMediator mediator,
        HttpContext httpContext,
        CancellationToken ct = default)
    {
        var result = await mediator.Send(new GetAllBrandsQuery(), ct);

        return result.ToApiResult(httpContext);
    }
}
