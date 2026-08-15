using ECommerce.API.Extensions;
using ECommerce.API.Extensions.Abstraction;
using ECommerce.API.Serilog;
using ECommerce.APP.Mediator;
using ECommerce.Domain.Constants;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Endpoints.V1.DeliveryMethods.Create;

public sealed class CreateDeliveryMethodEndpoint : IEndpoint
{
    private static readonly string Version = ApiVersions.V1;

    public void MapEndpoint(IEndpointRouteBuilder app)
        => app.MapVersionedEndpoint("delivery-methods", Version)
            .MapPost("/", Handle)
            .WithTags("Delivery Methods")
            .WithName("CreateDeliveryMethod")
            .WithGroupName("v1")
            .Produces(StatusCodes.Status201Created)
            .ProducesValidationProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Create delivery method")
            .WithDescription("Creates a new delivery method.")
            .DisableAntiforgery()
            .RequireAuthorization(policy => policy.RequireRole(Roles.Manager));

    public static async Task<IResult> Handle(
        [FromForm] CreateDeliveryMethodRequest request,
        IValidator<CreateDeliveryMethodRequest> validator,
        IMediator mediator,
        HttpContext httpContext,
        CancellationToken ct = default)
    {
        var validation = await validator.ValidateAsync(request, ct);

        if (!validation.IsValid)
            return Results.ValidationProblem(validation.ToDictionary());

        var command = request.ToCommand();

        var result = await mediator.Send(command, ct);

        if (result.IsFailure)
            return result.ToApiResult(httpContext, "");

        using (LoggingExtensions.WithDeliveryMethodContext(result.Value.Id))
        {
            var location = result.IsSuccess
                ? $"/api/v{Version}/delivery-methods/{result.Value.Id}"
                : null;

            return result.ToApiResult(httpContext, "Delivery method created successfully", location);
        }
    }
}
