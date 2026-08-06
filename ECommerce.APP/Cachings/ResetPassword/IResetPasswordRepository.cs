namespace ECommerce.APP.Cachings.ResetPassword;

public interface IResetPasswordRepository
{
    public Task SaveAsync(
        string hashedToken,
        Guid userId,
        CancellationToken ct = default);

    Task<Guid?> GetUserIdAsync(
        string hashedToken, 
        CancellationToken ct = default);

    Task DeleteAsync(
        string hashedToken,
        CancellationToken ct = default);
}
