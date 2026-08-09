using ECommerce.API.Common;
using ECommerce.API.Extensions;
using ECommerce.API.Extensions.Abstraction;
using ECommerce.API.Serilog;
using ECommerce.APP.Features.Brands.Queries.GetById;
using ECommerce.APP.Mediator;

namespace ECommerce.API.Endpoints.V1.Brands.GetById;

public class GetBrandByIdEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
        => app.MapVersionedEndpoint("brands", ApiVersions.V1)
            .MapGet("/{id:guid}", Handle)
            .WithTags("Brands")
            .WithName("GetBrandById")
            .WithGroupName("v1")
            .Produces<ApiResponse<GetBrandByIdResponse>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Retrieve brand by ID")
            .WithDescription("Returns brand with specified ID, or 404 if not found");

    public static async Task<IResult> Handle(
        Guid id,
        IMediator mediator,
        HttpContext httpContext,
        CancellationToken ct = default)
    {
        var result = await mediator.Send(new GetBrandByIdQuery(id), ct);

        if (result.IsFailure)
            return result.ToApiResult(httpContext, "");

        using (LoggingExtensions.WithBrandContext(id))
        {
            return result.ToApiResult(httpContext, "Retrieved brand ID data successfully");
        }
    }
}
