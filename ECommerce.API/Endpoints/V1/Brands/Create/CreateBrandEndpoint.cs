using ECommerce.API.Common;
using ECommerce.API.Extensions;
using ECommerce.API.Extensions.Abstraction;
using ECommerce.APP.Features.Brands.Commands;
using ECommerce.APP.Mediator;
using Microsoft.AspNetCore.Mvc;
using Serilog.Context;

namespace ECommerce.API.Endpoints.V1.Brands.Create;

public class CreateBrandEndpoint : IEndpoint
{
    private const string Version = ApiVersions.V1;

    public void MapEndpoint(IEndpointRouteBuilder app)
        => app.MapVersionedEndpoint("brands", Version)
            .MapPost("/", Handle)
            .WithTags("Brands")
            .WithName("CreateBrand")
            .WithGroupName("v1")
            .Produces<ApiResponse<CreateBrandResponse>>(StatusCodes.Status201Created)
            .ProducesValidationProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Create brand")
            .WithDescription("Create brand in DB, or 400 if validation fails")
            .Accepts<CreateBrandRequest>("multipart/form-data")
            .DisableAntiforgery();

    public static async Task<IResult> Handle(
        [FromForm] CreateBrandRequest request,
        IMediator mediator,
        HttpContext httpContext,
        CancellationToken ct)
    {
        using (LogContext.PushProperty("BrandName", request.Name))
        {
            var command = request.ToCommand();

            var result = await mediator.Send(command, ct);

            // Pass location for 201 Created response
            var location = result.IsSuccess
                ? $"/api/{Version}/brands/{result.Value.Id}"
                : null;

            return result.ToApiResult(httpContext, "Created brand successfully", location);
        }
    }
}
