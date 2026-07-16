using ECommerce.API.Common;
using ECommerce.API.Extensions;
using ECommerce.API.Extensions.Abstraction;
using ECommerce.API.Result;
using ECommerce.APP.Abstractions.Mediator;
using ECommerce.APP.Features.Products.Commands;
using Microsoft.AspNetCore.Mvc;
using Serilog.Context;

namespace ECommerce.API.Endpoints.V1.Products.Create;

public class CreateProductEndpoint : IEndpoint
{
    private const string Version = ApiVersions.V1;

    public void MapEndpoint(IEndpointRouteBuilder app)
        => app.MapVersionedEndpoint("products", Version)
            .MapPost("/", Handle)
            .WithTags("Products")
            .WithName("CreateProduct")
            .WithGroupName("v1")
            .Produces<ApiResponse<CreateProductResponse>>(StatusCodes.Status201Created)
            .ProducesValidationProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Create  product")
            .WithDescription("Create product in DB, or 400 if validation fails or if BrandId/TypeId don't reference existing records")
            .Accepts<CreateProductRequest>("multipart/form-data")
            .DisableAntiforgery();

    public static async Task<IResult> Handle(
        [FromForm] CreateProductRequest request,  // Use [FromForm] for multipart/form-data
        IMediator mediator,
        HttpContext httpContext,
        CancellationToken ct)
    {
        using (LogContext.PushProperty("ProductName", request.Name))
        {
            var command = request.ToCommand();

            var result = await mediator.Send(command, ct);

            // Pass location for 201 Created response
            var location = result.IsSuccess
                ? $"/api/{Version}/products/{result.Value.Id}"
                : null;

            return result.ToApiResult(httpContext, "Created product successfully", location);
        }
    }
}
