using ECommerce.API.Common;
using ECommerce.API.Extensions;
using ECommerce.API.Extensions.Abstraction;
using ECommerce.API.Result;
using ECommerce.APP.Abstractions.Mediator;
using ECommerce.APP.Features.Products.Queries.GetAll;
using FluentValidation;

namespace ECommerce.API.Endpoints.V1.Products.GetPagination;

public class GetProductsPaginationEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
        => app.MapVersionedEndpoint("products", ApiVersions.V1)
            .MapGet("/paged", Handle)
            .WithTags("Products")
            .WithName("Get products with paginations")
            .WithGroupName("v1")
            .Produces<ApiResponse<IReadOnlyList<GetAllProductsResponse>>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Get products with paginations")
            .WithDescription("Returns paginated products in DB, or 404 if not found");

    public async static Task<IResult> Handle(
        [AsParameters] GetProductsPaginationRequest request,
        IValidator<GetProductsPaginationRequest> validator,
        IMediator mediator,
        HttpContext httpContext,
        CancellationToken ct = default)
    {
        var validation = await validator.ValidateAsync(request, ct);

        if (!validation.IsValid)
            return Results.ValidationProblem(validation.ToDictionary());

        var query = request.ToQuery();

        var result = await mediator.Send(query, ct);

        return result.ToPaginatedApiResult(httpContext, request.PageNumber, request.PageSize, "Retrieved paginated products successfully");
    }
}
