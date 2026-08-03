namespace ECommerce.APP.Email;

public sealed class EmailSettings
{
    public const string SectionName = "Email";

    public string FromEmail { get; set; } = default!;
    public string FromName { get; set; } = "ECommerce";
    public string Host { get; set; } = "localhost";
    public int Port { get; set; } = 25;
    public string? Username { get; set; }
    public string? Password { get; set; }
}
