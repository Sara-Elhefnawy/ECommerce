using ECommerce.API.Common;
using ECommerce.API.Extensions;
using ECommerce.API.Extensions.Abstraction;
using ECommerce.APP.Features.Types.Queries.GetById;
using ECommerce.APP.Features.Types.Queries.GetByName;
using ECommerce.APP.Mediator;

namespace ECommerce.API.Endpoints.V1.Types.GetByName;

public class GetTypeByNameEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
        => app.MapVersionedEndpoint("types", ApiVersions.V1)
            .MapGet("/search-by-name", Handle)
            .WithTags("Types")
            .WithName("GetTypeByName")
            .WithGroupName("v1")
            .Produces<ApiResponse<GetTypeByIdResponse>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Retrieve type by name")
            .WithDescription("Returns type with specified name, or 404 if not found");

    public static async Task<IResult> Handle(
        string name,
        IMediator mediator,
        HttpContext httpContext,
        CancellationToken ct = default)
    {
        var result = await mediator.Send(new GetTypeByNameQuery(name), ct);

        return result.ToApiResult(httpContext, "Retrieved type name data successfully");
    }
}
