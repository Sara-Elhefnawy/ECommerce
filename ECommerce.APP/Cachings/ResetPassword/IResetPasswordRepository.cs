using ECommerce.Domain.Results;

namespace ECommerce.APP.Cachings.ResetPassword;

public interface IResetPasswordRepository
{
    public Task<Result> SaveAsync(
        string hashedToken,
        Guid userId,
        CancellationToken ct = default);

    Task<ResultOfT<Guid?>> GetUserIdAsync(
        string hashedToken, 
        CancellationToken ct = default);

    Task<Result> DeleteAsync(
        string hashedToken,
        CancellationToken ct = default);
}
