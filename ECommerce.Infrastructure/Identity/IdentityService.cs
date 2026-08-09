using ECommerce.APP.Features.Users.Commands.UpdateUser.Common;
using ECommerce.APP.Identity;
using ECommerce.APP.Token;
using ECommerce.Domain.Constants;
using ECommerce.Domain.Entities.Errors;
using ECommerce.Domain.Results;
using Microsoft.AspNetCore.Identity;

namespace ECommerce.Infrastructure.Identity;

public sealed class IdentityService(
    UserManager<ApplicationUser> userManager,
    RoleManager<ApplicationRole> roleManager) 
    : IIdentityService
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

    // Resets a user's password to a new value.
    // ASP.NET Core Identity requires generating a password reset token first,
    // then using that token to reset the password. However, in a password reset flow,
    // the token is managed by our application (stored in Redis), not Identity.
    //      Solution: Use GeneratePasswordResetTokenAsync and ResetPasswordAsync together,
    //      passing a dummy token that won't be validated (since we already validated our token).
    public async Task<Result> ResetPasswordAsync(
        Guid userId,
        string newPassword,
        CancellationToken ct = default)
    {
        try
        {
            var user = await userManager.FindByIdAsync(userId.ToString());

            if (user == null)
                return Result.Failure(IdentityErrors.UserNotFound);

            // Using Identity's Token-Based Reset
            // Generate a password reset token from Identity
            // This token is separate from our Redis-managed token
            string identityToken = await userManager.GeneratePasswordResetTokenAsync(user);

            // Reset the password using the Identity token
            var result = await userManager.ResetPasswordAsync(user, identityToken, newPassword);

            if (!result.Succeeded)
            {
                // Collect all Identity errors
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                return Result.Failure(IdentityErrors.OperationFailed($"Password reset failed: {errors}"));
            }

            return Result.Ok();
        }
        catch (Exception ex)
        {
            return Result.Failure(
                IdentityErrors.OperationFailed($"An unexpected error occurred: {ex.Message}"));
        }
    }

    public async Task<ResultOfT<AuthUserSnapshot>> AddRoleAsync(Guid userId, string role, CancellationToken ct = default)
    {
        var user = await userManager.FindByIdAsync(userId.ToString());

        if (user == null)
            return ResultOfT<AuthUserSnapshot>.Failure(IdentityErrors.UserNotFound);

        if (!await roleManager.RoleExistsAsync(role))
            return ResultOfT<AuthUserSnapshot>.Failure(
                IdentityErrors.OperationFailed($"Role '{role}' does not exist."));

        if (!RoleHierarchy.Inherits.TryGetValue(role, out var rolesToGrant))
            rolesToGrant = [role];

        var currentRoles = await userManager.GetRolesAsync(user);

        var rolesToAdd = rolesToGrant.Except(currentRoles, StringComparer.OrdinalIgnoreCase).ToList();

        if (rolesToAdd.Count == 0)
            return ResultOfT<AuthUserSnapshot>.Failure(
                IdentityErrors.OperationFailed($"User already has the role '{role}' and everything it includes."));

        var addResult = await userManager.AddToRolesAsync(user, rolesToAdd);

        if (!addResult.Succeeded)
            return ResultOfT<AuthUserSnapshot>.Failure(
                IdentityErrors.OperationFailed(
                    string.Join("; ", addResult.Errors.Select(e => e.Description))));

        return ResultOfT<AuthUserSnapshot>.Ok(new AuthUserSnapshot(
            user.Id,
            user.Email!,
            user.UserDisplayName
            ));
    }

    public async Task<ResultOfT<AuthUserSnapshot>> RemoveRoleAsync(Guid userId, string role, CancellationToken ct = default)
    {
        var user = await userManager.FindByIdAsync(userId.ToString());

        if (user == null)
            return ResultOfT<AuthUserSnapshot>.Failure(IdentityErrors.UserNotFound);

        if (!await roleManager.RoleExistsAsync(role))
            return ResultOfT<AuthUserSnapshot>.Failure(
                IdentityErrors.OperationFailed($"Role '{role}' does not exist."));

        var currentRoles = await userManager.GetRolesAsync(user);

        if (!currentRoles.Contains(role, StringComparer.OrdinalIgnoreCase))
            return ResultOfT<AuthUserSnapshot>.Failure(
                IdentityErrors.OperationFailed($"User does not have the role '{role}'."));

        if (!RoleHierarchy.Dependents.TryGetValue(role, out var rolesToRemove))
            rolesToRemove = [role];

        // Removing "Admin" from someone who never had "SuperAdmin" should just remove "Admin"
        // Removing "Admin" from someone who had "SuperAdmin" should remove "Admin" and "SuperAdmin"
        var actualRolesToRemove = rolesToRemove
            .Intersect(currentRoles, StringComparer.OrdinalIgnoreCase)
            .ToList();

        var removeResult = await userManager.RemoveFromRolesAsync(user, actualRolesToRemove);

        if (!removeResult.Succeeded)
            return ResultOfT<AuthUserSnapshot>.Failure(
                IdentityErrors.OperationFailed(string.Join("; ", removeResult.Errors.Select(e => e.Description))));

        return ResultOfT<AuthUserSnapshot>.Ok(new AuthUserSnapshot(
            user.Id, user.Email!, user.UserDisplayName));
    }
}
