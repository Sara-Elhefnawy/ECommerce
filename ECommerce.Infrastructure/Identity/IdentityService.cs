using ECommerce.APP.Features.Users.Commands.UpdateUser.Common;
using ECommerce.APP.Identity;
using ECommerce.APP.Token;
using ECommerce.Domain.Constants;
using ECommerce.Domain.Entities.Errors;
using ECommerce.Domain.Results;
using Microsoft.AspNetCore.Identity;

namespace ECommerce.Infrastructure.Identity;

public sealed class IdentityService(UserManager<ApplicationUser> userManager) : IIdentityService
{
    public async Task<ResultOfT<AuthUserSnapshot>> CreateUserAsync(
        string email,
        string password,
        string? displayName,
        CancellationToken ct = default)
    {
        var user = new ApplicationUser
        {
            UserName = email,
            Email = email,
            UserDisplayName = displayName,
            EmailConfirmed = false
        };

        var result = await userManager.CreateAsync(user, password);
        if (!result.Succeeded)
        {
            if (result.Errors.Any(e =>
                    e.Code is "DuplicateEmail" or "DuplicateUserName"))
            {
                return ResultOfT<AuthUserSnapshot>.Failure(IdentityErrors.EmailAlreadyExists);
            }

            var message = string.Join(" ", result.Errors.Select(e => e.Description));
            return ResultOfT<AuthUserSnapshot>.Failure(IdentityErrors.OperationFailed(message));
        }

        await userManager.AddToRoleAsync(user, Roles.User);

        return ResultOfT<AuthUserSnapshot>.Ok(
            new AuthUserSnapshot(user.Id, user.Email, user.UserDisplayName));
    }

    public async Task<ResultOfT<AuthUserSnapshot>> ValidateCredentialsAsync(
        string email,
        string password,
        CancellationToken ct = default)
    {
        var user = await userManager.FindByEmailAsync(email);
        if (user is null)
            return ResultOfT<AuthUserSnapshot>.Failure(IdentityErrors.InvalidCredentials);

        var isValid = await userManager.CheckPasswordAsync(user, password);
        if (!isValid)
            return ResultOfT<AuthUserSnapshot>.Failure(IdentityErrors.InvalidCredentials);

        if (!user.EmailConfirmed)
            return ResultOfT<AuthUserSnapshot>.Failure(IdentityErrors.EmailNotConfirmed);

        return ResultOfT<AuthUserSnapshot>.Ok(
            new AuthUserSnapshot(user.Id, user.Email!, user.UserDisplayName));
    }

    public async Task<ResultOfT<AuthUserSnapshot>> GetUserByEmailAsync(
        string email,
        CancellationToken ct = default)
    {
        var user = await userManager.FindByEmailAsync(email);
        if (user is null)
            return ResultOfT<AuthUserSnapshot>.Failure(IdentityErrors.UserNotFound);

        return ResultOfT<AuthUserSnapshot>.Ok(
            new AuthUserSnapshot(user.Id, user.Email!, user.UserDisplayName));
    }

    public async Task<ResultOfT<AuthUserSnapshot>> GetUserByIdAsync(
        Guid userId,
        CancellationToken ct = default)
    {
        var user = await userManager.FindByIdAsync(userId.ToString());
        if (user is null)
            return ResultOfT<AuthUserSnapshot>.Failure(IdentityErrors.UserNotFound);

        return ResultOfT<AuthUserSnapshot>.Ok(
            new AuthUserSnapshot(user.Id, user.Email!, user.UserDisplayName));
    }

    // update only Display name
    public async Task<ResultOfT<AuthUserSnapshot>> UpdateProfileAsync(
        Guid userId,
        Optional<string?> displayName,
        CancellationToken ct = default)
    {
        var user = await userManager.FindByIdAsync(userId.ToString());
        if (user is null)
            return ResultOfT<AuthUserSnapshot>.Failure(IdentityErrors.UserNotFound);

        // Only touch UserDisplayName if the JSON actually included the key.
        // IsSet = false → key was missing entirely → leave user.UserDisplayName untouched.
        // IsSet = true, Value = null → client explicitly wants to clear it.
        // IsSet = true, Value = "..." → client wants to set it.
        if (displayName.IsSet)
        {
            user.UserDisplayName = string.IsNullOrWhiteSpace(displayName.Value)
                ? null
                : displayName.Value.Trim();
        }

        var result = await userManager.UpdateAsync(user);

        if (!result.Succeeded)
        {
            var message = string.Join(" ", result.Errors.Select(e => e.Description));
            return ResultOfT<AuthUserSnapshot>.Failure(IdentityErrors.OperationFailed(message));
        }

        return ResultOfT<AuthUserSnapshot>.Ok(
            new AuthUserSnapshot(user.Id, user.Email!, user.UserDisplayName));
    }

    public async Task<Result> ConfirmEmailAsync(
        string email,
        CancellationToken ct = default)
    {
        var user = await userManager.FindByEmailAsync(email);

        if (user is null)
            return Result.Failure(IdentityErrors.UserNotFound);

        if (user.EmailConfirmed)
            return Result.Failure(IdentityErrors.EmailAlreadyConfirmed);

        user.EmailConfirmed = true;
        var result = await userManager.UpdateAsync(user);

        return result.Succeeded
            ? Result.Ok()
            : Result.Failure(IdentityErrors.OperationFailed(
                string.Join(" ", result.Errors.Select(e => e.Description))));
    }

    public async Task<bool> IsEmailConfirmedAsync(
        string email,
        CancellationToken ct = default)
    {
        var user = await userManager.FindByEmailAsync(email);
        return user?.EmailConfirmed ?? false;
    }

    public async Task<IReadOnlyList<string>> GetRolesAsync(
        Guid userId,
        CancellationToken ct = default)
    {
        var user = await userManager.FindByIdAsync(userId.ToString());
        if (user is null)
            return [];

        var roles = await userManager.GetRolesAsync(user);
        return roles.ToList();
    }
}
