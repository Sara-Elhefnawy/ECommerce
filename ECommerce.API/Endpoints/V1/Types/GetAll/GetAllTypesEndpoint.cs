using ECommerce.API.Common;
using ECommerce.API.Endpoints.V1.Brands.GetAll;
using ECommerce.API.Extensions;
using ECommerce.API.Extensions.Abstraction;
using ECommerce.API.Result;
using ECommerce.APP.Abstractions.Mediator;
using ECommerce.APP.Features.Types.Queries.GetAll;

namespace ECommerce.API.Endpoints.V1.Types.GetAll;

public class GetAllTypesEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
        => app.MapVersionedEndpoint("api/types", ApiVersions.V1)
            .MapGet("/", Handle)
            .WithTags("Types")
            .WithName("GetTypes")
            .WithGroupName("v1")
            .Produces<ApiResponse<IReadOnlyList<GetAllTypesResponse>>>(StatusCodes.Status200OK)
            .WithSummary("Get types")
            .WithDescription("Returns all types in DB");


    public static async Task<IResult> Handle(
        IMediator mediator,
        HttpContext httpContext,
        CancellationToken ct = default)
    {
        var result = await mediator.Send(new GetAllTypesQuery(), ct);

        return result.ToApiResult(httpContext);
    }
}
