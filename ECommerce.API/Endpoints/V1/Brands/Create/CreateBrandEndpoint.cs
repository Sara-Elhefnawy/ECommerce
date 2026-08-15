using ECommerce.API.Common;
using ECommerce.API.Extensions;
using ECommerce.API.Extensions.Abstraction;
using ECommerce.API.Serilog;
using ECommerce.APP.Features.Brands.Commands;
using ECommerce.APP.Mediator;
using ECommerce.Domain.Constants;
using Microsoft.AspNetCore.Mvc;

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
            .ProducesValidationProblem(StatusCodes.Status409Conflict)
            .WithSummary("Create brand")
            .WithDescription("Create brand in DB, or 400 if validation fails")
            .Accepts<CreateBrandRequest>("multipart/form-data")
            .DisableAntiforgery()
            .RequireAuthorization(policy => policy.RequireRole(Roles.Admin));

    public static async Task<IResult> Handle(
        [FromForm] CreateBrandRequest request,
        IMediator mediator,
        HttpContext httpContext,
        CancellationToken ct)
    {
        var command = request.ToCommand();

        var result = await mediator.Send(command, ct);

        if (result.IsFailure)
            return result.ToApiResult(httpContext, "");

        using (LoggingExtensions.WithBrandContext(result.Value.Id))
        {
            // Pass location for 201 Created response
            var location = $"/api/v{Version}/brands/{result.Value.Id}";

            return result.ToApiResult(httpContext, "Created brand successfully", location);
        }
    }
}
