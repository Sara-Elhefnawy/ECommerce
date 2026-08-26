using ECommerce.API.Extensions;
using ECommerce.API.Extensions.Abstraction;
using ECommerce.APP.Payments;
using ECommerce.APP.Settings;
using Microsoft.Extensions.Options;
using Stripe;

namespace ECommerce.API.Endpoints.V1.Payments.Webhook;

public sealed class WebhookEndpoint : IEndpoint
{
    // Public — Stripe calls this (no JWT)
    public void MapEndpoint(IEndpointRouteBuilder app)
        => app.MapVersionedEndpoint("payments", ApiVersions.V1)
            .MapPost("/webhook", Handle)
            .WithTags("Payments")
            .WithName("Webhook")
            .WithGroupName("v1")
            .WithSummary("Stripe webhook")
            .WithDescription("Verifies Stripe-Signature; calls PaymentSucceeded / PaymentFailed on the payment service.")
            .AllowAnonymous();

    public async Task<IResult> Handle(
        HttpRequest request,
        IPaymentService paymentService,
        IOptions<StripeSettings> stripeOptions,
        CancellationToken ct)
    {
        var json = await new StreamReader(request.Body).ReadToEndAsync(ct);
        var signature = request.Headers["Stripe-Signature"].ToString();

        if (string.IsNullOrWhiteSpace(signature))
            return Results.BadRequest("Missing Stripe-Signature header.");

        try
        {
            var stripeEvent = EventUtility.ConstructEvent(
                json,
                signature,
                stripeOptions.Value.WebhookSecret);

            switch (stripeEvent.Type)
            {
                case EventTypes.PaymentIntentSucceeded:
                    {
                        var succeeded = stripeEvent.Data.Object as PaymentIntent;
                        if (succeeded is not null)
                            await paymentService.PaymentSucceeded(succeeded.Id, ct);
                        break;
                    }

                case EventTypes.PaymentIntentPaymentFailed:
                    {
                        var failed = stripeEvent.Data.Object as PaymentIntent;
                        if (failed is not null)
                            await paymentService.PaymentFailed(failed.Id, ct);
                        break;
                    }

                default:
                    break;
            }

            return Results.Ok();
        }
        catch (StripeException ex)
        {
            return Results.BadRequest(ex.Message);
        }
    }
}
