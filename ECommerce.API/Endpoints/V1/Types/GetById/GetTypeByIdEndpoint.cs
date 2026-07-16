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
            .WithSummary("Retrieve type by id")
            .WithDescription("Returns a type by id from DB");

    public static async Task<IResult> Handle(
        Guid id,
        IMediator mediator,
        HttpContext httpContext,
        CancellationToken ct = default)
    {
        using (LogContext.PushProperty("TypeId", id))
        {
            var result = await mediator.Send(new GetTypeByIdQuery(id), ct);

            return result.ToApiResult(httpContext, "Retrieved type ID data successfully");
        }
    }
}
