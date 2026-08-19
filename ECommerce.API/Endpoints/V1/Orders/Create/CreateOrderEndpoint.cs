using ECommerce.API.Common;
using ECommerce.API.Extensions;
using ECommerce.API.Extensions.Abstraction;
using ECommerce.API.Serilog;
using ECommerce.APP.Features.Orders.Commands.Create;
using ECommerce.APP.Features.Orders.DTOs;
using ECommerce.APP.Mediator;
using ECommerce.Domain.Constants;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Endpoints.V1.Orders.Create;

public sealed class CreateOrderEndpoint : IEndpoint
{
    private const string Version = ApiVersions.V1;

    public void MapEndpoint(IEndpointRouteBuilder app)
        => app.MapVersionedEndpoint("orders", Version)
            .MapPost("/", Handle)
            .WithTags("Orders")
            .WithName("CreateOrder")
            .WithGroupName("v1")
            .Produces<ApiResponse<OrderResponse>>(StatusCodes.Status201Created)
            .ProducesValidationProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .WithSummary("Create order (checkout)")
            .WithDescription("Creates an order from the authenticated user's cart, then clears the cart.")
            .RequireAuthorization(policy => policy.RequireRole(Roles.User));

    public static async Task<IResult> Handle(
        [FromBody] CreateOrderCommand command,
        IMediator mediator,
        HttpContext httpContext,
        CancellationToken ct)
    {
        var result = await mediator.Send(command, ct);

        if (result.IsFailure)
            return result.ToApiResult(httpContext, "");

        using (LoggingExtensions.WithOrderContext(result.Value.Id))
        {
            // Pass location for 201 Created response
            var location = result.IsSuccess
                ? $"/api/v{Version}/orders/{result.Value.Id}"
                : null;

            return result.ToApiResult(httpContext, "Created order successfully", location);
        }
    }
}
