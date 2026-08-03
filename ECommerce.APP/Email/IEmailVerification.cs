namespace ECommerce.APP.Email;

public interface IEmailVerification
{
    Task SaveAsync(
        string email,
        string code,
        CancellationToken ct = default);

    // Returns true when the code matches and removes it (one-time use).
    Task<bool> ValidateAndConsumeAsync(
        string email,
        string code,
        CancellationToken ct = default);
}
