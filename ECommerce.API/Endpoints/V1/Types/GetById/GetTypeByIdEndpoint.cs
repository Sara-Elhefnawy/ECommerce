using ECommerce.API.Common;
using ECommerce.API.Endpoints.V1.Brands.GetAll;
using ECommerce.API.Extensions;
using ECommerce.API.Extensions.Abstraction;
using ECommerce.API.Result;
using ECommerce.APP.Abstractions.Mediator;
using ECommerce.APP.Features.Types.Queries.GetById;
using Serilog.Context;

namespace ECommerce.API.Endpoints.V1.Types.GetById;

public class GetTypeByIdEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
        => app.MapVersionedEndpoint("api/types", ApiVersions.V1)
            .MapGet("/{id:guid}", Handle)
            .WithTags("Types")
            .WithName("GetTypeById")
            .WithGroupName("v1")
            .Produces<ApiResponse<GetTypeByIdResponse>>(StatusCodes.Status200OK)
            .WithSummary("Get type by id")
            .WithDescription("Returns a type by id from DB");

    public static async Task<IResult> Handle(
        Guid id,
        IMediator mediator,
        HttpContext httpContext,
        ILogger<GetAllBrandsEndpoint> logger,
        CancellationToken ct = default)
    {
        using (LogContext.PushProperty("TypeId", id))
        {
            logger.LogInformation("Retrieving type with id {Id} from database", id);

            var result = await mediator.Send(new GetTypeByIdQuery(id), ct);

            logger.LogInformation("Query completed with result: {Result}", result);

            return result.ToApiResult(httpContext);
        }
    }
}
