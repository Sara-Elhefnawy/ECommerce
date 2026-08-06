namespace ECommerce.APP.Settings;

public sealed class ResetPasswordSettings
{
    public const string SectionName = "ResetPassword";
    public string FrontendResetPasswordUrl { get; init; } = default!;

    public string ExpirationMinutes { get; init; } = default!;
}
