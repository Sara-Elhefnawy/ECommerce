using ECommerce.API.Common;
using ECommerce.API.Extensions;
using ECommerce.API.Extensions.Abstraction;
using ECommerce.APP.Features.Payments;
using ECommerce.APP.Features.Payments.Commands.Create;
using ECommerce.APP.Mediator;
using ECommerce.Domain.Constants;

namespace ECommerce.API.Endpoints.V1.Orders.Checkout;

public sealed class CheckoutEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
        => app.MapVersionedEndpoint("orders", ApiVersions.V1)
            .MapPost("/{id:guid}/pay", Handle)
            .WithTags("Orders")
            .WithName("CreateOrderPayment")
            .WithGroupName("v1")
            .Produces<ApiResponse<PaymentClientSecretResponse>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict)
            .WithSummary("Create Stripe PaymentIntent for order (or update amount if one already exists)")
            .WithDescription("Uses order.Total (server-side). Calls CreatePaymentIntent when none exists; otherwise UpdatePaymentIntent.")
            .RequireAuthorization(policy => policy.RequireRole(Roles.User));

    public async Task<IResult> Handle(
        Guid id,
        IMediator mediator,
        HttpContext httpContext,
        CancellationToken ct)
    {
        var result = await mediator.Send(new CreateOrderPaymentCommand(id), ct);

        return result.ToApiResult(httpContext, "Payment intent created successfully");
    }
}
