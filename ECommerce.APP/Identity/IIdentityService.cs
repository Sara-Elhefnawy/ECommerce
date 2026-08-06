using ECommerce.APP.Features.Users.Commands.UpdateUser.Common;
using ECommerce.APP.Token;
using ECommerce.Domain.Results;

namespace ECommerce.APP.Identity;

public interface IIdentityService
{
    Task<ResultOfT<AuthUserSnapshot>> CreateUserAsync(
        string email,
        string password,
        string? displayName,
        CancellationToken ct = default);

    Task<ResultOfT<AuthUserSnapshot>> ValidateCredentialsAsync(
        string email,
        string password,
        CancellationToken ct = default);

    Task<ResultOfT<AuthUserSnapshot>> GetUserByEmailAsync(
        string email,
        CancellationToken ct = default);

    Task<ResultOfT<AuthUserSnapshot>> GetUserByIdAsync(
        Guid userId,
        CancellationToken ct = default);

    Task<ResultOfT<AuthUserSnapshot>> UpdateProfileAsync(
        Guid userId,
        Optional<string?> displayName,
        CancellationToken ct = default);

    Task<Result> ConfirmEmailAsync(
        string email,
        CancellationToken ct = default);

    Task<bool> IsEmailConfirmedAsync(
        string email,
        CancellationToken ct = default);

    Task<IReadOnlyList<string>> GetRolesAsync(
        Guid userId,
        CancellationToken ct = default);

    Task<ResultOfT<AuthUserSnapshot>> AddRoleAsync(
        Guid userId,
        string role,
        CancellationToken ct = default);

    Task<ResultOfT<AuthUserSnapshot>> RemoveRoleAsync(
        Guid userId,
        string role,
        CancellationToken ct = default);

    Task<Result> ResetPasswordAsync(
        Guid userId,
        string newPassword,
        CancellationToken ct = default);
}
