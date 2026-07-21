using ECommerce.API.Common;
using ECommerce.API.Extensions;
using ECommerce.API.Extensions.Abstraction;
using ECommerce.APP.Features.Inventories.Queries.GetAll;
using ECommerce.APP.Mediator;

namespace ECommerce.API.Endpoints.V1.Inventories.GetAll;

public sealed class GetAllInventoriesEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
        => app.MapVersionedEndpoint("inventories", ApiVersions.V1)
            .MapGet("/", Handle)
            .WithTags("Inventories")
            .WithName("GetInventories")
            .WithGroupName("v1")
            .Produces<ApiResponse<IReadOnlyList<GetAllInventoriesResponse>>>(StatusCodes.Status200OK)
            .WithSummary("Get inventories")
            .WithDescription("Returns all inventories in DB");

    public static async Task<IResult> Handle(
        [AsParameters] GetAllInventoriesRequest request,
        IMediator mediator,
        HttpContext httpContext,
        CancellationToken ct = default)
    {
        var result = await mediator.Send(new GetAllInventoriesQuery(request.Count), ct);

        return result.ToApiResult(httpContext, "Retrieved all inventories successfully");
    }
}
