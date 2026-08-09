using ECommerce.API.Common;
using ECommerce.API.Extensions;
using ECommerce.API.Extensions.Abstraction;
using ECommerce.API.Serilog;
using ECommerce.APP.Features.Types.Queries.GetById;
using ECommerce.APP.Mediator;

namespace ECommerce.API.Endpoints.V1.Types.GetById;

public class GetTypeByIdEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
        => app.MapVersionedEndpoint("types", ApiVersions.V1)
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
        var result = await mediator.Send(new GetTypeByIdQuery(id), ct);

        if (result.IsFailure)
            return result.ToApiResult(httpContext, "");

        using (LoggingExtensions.WithTypeContext(id))
        {
            return result.ToApiResult(httpContext, "Retrieved type ID data successfully");
        }
    }
}
