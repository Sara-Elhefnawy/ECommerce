using ECommerce.API.Common;
using ECommerce.API.Extensions;
using ECommerce.API.Extensions.Abstraction;
using ECommerce.APP.Features.Brands.Queries.GetById;
using ECommerce.APP.Features.Brands.Queries.GetByName;
using ECommerce.APP.Mediator;

namespace ECommerce.API.Endpoints.V1.Brands.GetByName;

public class GetBrandByNameEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
        => app.MapVersionedEndpoint("brands", ApiVersions.V1)
            .MapGet("/search-by-name", Handle)
            .WithTags("Brands")
            .WithName("GetBrandByName")
            .WithGroupName("v1")
            .Produces<ApiResponse<GetBrandByIdResponse>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Retrieve brand by name")
            .WithDescription("Returns brand with specified name, or 404 if not found");

    public static async Task<IResult> Handle(
        string name,
        IMediator mediator,
        HttpContext httpContext,
        CancellationToken ct = default)
    {
        var result = await mediator.Send(new GetBrandByNameQuery(name), ct);

        return result.ToApiResult(httpContext, "Retrieved brand name data successfully");
    }
}
