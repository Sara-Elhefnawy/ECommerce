using ECommerce.API.Common;
using ECommerce.API.Extensions;
using ECommerce.API.Extensions.Abstraction;
using ECommerce.APP.Features.Types.Commands;
using ECommerce.APP.Mediator;
using Microsoft.AspNetCore.Mvc;
using Serilog.Context;

namespace ECommerce.API.Endpoints.V1.Types.Create;

public class CreateTypeEndpoint : IEndpoint
{
    private const string Version = ApiVersions.V1;

    public void MapEndpoint(IEndpointRouteBuilder app)
        => app.MapVersionedEndpoint("types", Version)
            .MapPost("/", Handle)
            .WithTags("Types")
            .WithName("CreateType")
            .WithGroupName("v1")
            .Produces<ApiResponse<CreateTypeResponse>>(StatusCodes.Status201Created)
            .ProducesValidationProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Create type")
            .WithDescription("Create type in DB, or 400 if validation fails")
            .Accepts<CreateTypeRequest>("multipart/form-data")
            .DisableAntiforgery();

    public static async Task<IResult> Handle(
        [FromForm] CreateTypeRequest request,
        IMediator mediator,
        HttpContext httpContext,
        CancellationToken ct)
    {
        using (LogContext.PushProperty("TypeName", request.Name))
        {
            var command = request.ToCommand();

            var result = await mediator.Send(command, ct);

            // Pass location for 201 Created response
            var location = result.IsSuccess
                ? $"/api/{Version}/types/{result.Value.Id}"
                : null;

            return result.ToApiResult(httpContext, "Created type successfully", location);
        }
    }
}
