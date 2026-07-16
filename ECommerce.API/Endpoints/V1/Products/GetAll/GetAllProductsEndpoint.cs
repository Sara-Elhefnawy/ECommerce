using ECommerce.API.Common;
using ECommerce.API.Extensions;
using ECommerce.API.Extensions.Abstraction;
using ECommerce.API.Result;
using ECommerce.APP.Abstractions.Mediator;
using ECommerce.APP.Features.Products.Queries.GetAll;

namespace ECommerce.API.Endpoints.V1.Products.GetAll;

public class GetAllProductsEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
        => app.MapVersionedEndpoint("products", ApiVersions.V1)
            .MapGet("/", Handle)
            .WithTags("Products")
            .WithName("GetProducts")
            .WithGroupName("v1")
            .Produces<ApiResponse<IReadOnlyList<GetAllProductsResponse>>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Get products")
            .WithDescription("Returns all products in DB, or 404 if list is empty");

    public static async Task<IResult> Handle(
        [AsParameters] GetAllProductsRequest request,
        IMediator mediator, 
        HttpContext httpContext,
        CancellationToken ct = default)
    {
        var result = await mediator.Send(new GetAllProductsQuery(request.Count), ct);

        return result.ToApiResult(httpContext, "Retrieved all products successfully");
    }
}
