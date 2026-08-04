using ECommerce.Domain.Results;

namespace ECommerce.APP.Token.RefreshTokens;

public interface IRefreshTokenService
{
    Task<RefreshTokenIssueResult> IssueAsync(
        Guid userId,
        CancellationToken ct = default);

    Task<ResultOfT<RefreshTokenIssueResult>> RotateAsync(
        string refreshToken,
        CancellationToken ct = default);

    Task<Result> RevokeAsync(
        string refreshToken,
        CancellationToken ct = default);
}
