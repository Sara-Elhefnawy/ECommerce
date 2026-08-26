namespace ECommerce.APP.Settings;

public sealed class StripeSettings
{
    public const string SectionName = "Stripe";

    public string SecretKey { get; set; } = default!;
    public string PublishableKey { get; set; } = default!;

    // 
    public string WebhookSecret { get; set; } = default!;

    // usd — must match Stripe account capabilities.
    public string Currency { get; set; } = "usd";
}
