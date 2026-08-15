using ECommerce.API.Common;
using ECommerce.API.Endpoints.V1.DeliveryMethods.Update;
using ECommerce.API.Extensions;
using ECommerce.API.Extensions.Abstraction;
using ECommerce.API.Serilog;
using ECommerce.APP.Features.DeliveryMethods;
using ECommerce.APP.Mediator;
using ECommerce.Domain.Constants;
using FluentValidation;

namespace ECommerce.API.Endpoints.V1.DeliveryMethods.Command.Update;

public sealed class UpdateDeliveryMethodEndpoint : IEndpoint
{
    private const string Version = ApiVersions.V1;

    public void MapEndpoint(IEndpointRouteBuilder app)
        => app.MapVersionedEndpoint("delivery-methods", Version)
            .MapPut("/{id:guid}", Handle)
            .WithTags("Delivery Methods")
            .WithName("UpdateDeliveryMethod")
            .WithGroupName("v1")
            .Produces<ApiResponse<DeliveryMethodResponse>>(
                StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesValidationProblem(
                StatusCodes.Status400BadRequest)
            .WithSummary("Update delivery method")
            .WithDescription("Updates an existing delivery method.")
            .RequireAuthorization(
                policy => policy.RequireRole(Roles.Manager));

    public static async Task<IResult> Handle(
        Guid id,
        UpdateDeliveryMethodRequest request,
        IValidator<UpdateDeliveryMethodRequest> validator,
        IMediator mediator,
        HttpContext httpContext,
        CancellationToken ct = default)
    {
        var validation = await validator.ValidateAsync(request, ct);

        if (!validation.IsValid)
            return Results.ValidationProblem(validation.ToDictionary());

        var command = request.ToCommand(id);

        var result = await mediator.Send(command, ct);

        if (result.IsFailure)
            return result.ToApiResult(httpContext, "");

        using (LoggingExtensions.WithDeliveryMethodContext(id))
        {
            return result.ToApiResult(
                httpContext,
                "Delivery method updated successfully");
        }
    }
}
