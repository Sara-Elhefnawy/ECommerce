using ECommerce.APP.Cachings.ResetPassword;
using ECommerce.APP.Identity;
using ECommerce.APP.Mediator;
using ECommerce.Domain.Entities.Errors;
using ECommerce.Domain.Results;
using System.Security.Cryptography;
using System.Text;

namespace ECommerce.APP.Features.Users.Commands.ConfirmPasswordReset;

public sealed class ConfirmPasswordResetHandler(
    IIdentityService identityService,
    IResetPasswordRepository resetPasswordRepository)
    : IRequestHandler<ConfirmPasswordResetCommand, ResultOfT<ConfirmPasswordResetResponse>>
{
    public async Task<ResultOfT<ConfirmPasswordResetResponse>> Handle(
        ConfirmPasswordResetCommand request,
        CancellationToken ct = default)
    {
        // 1. Validate input
        if (string.IsNullOrWhiteSpace(request.PasswordResetToken) || string.IsNullOrWhiteSpace(request.NewPassword))
            return IdentityErrors.InvalidResetInput("Token and new password are required.");

        var userEmail = await identityService.GetUserByEmailAsync(request.Email, ct);

        if (userEmail.IsFailure)
            return userEmail.Error!;

        // 2. Hash the token the user sent (must match the hash in cache)
        string hashedToken = HashToken(request.PasswordResetToken);

        // 3. PEEK the token — do NOT delete yet.
        // We only want to burn it once we know the password reset actually succeeded.
        var userIdResult = await resetPasswordRepository.GetUserIdAsync(hashedToken, ct);

        // 4. If token is invalid or expired, return error
        if (userIdResult.IsFailure)
            return userIdResult.Error!;

        // Make sure the token actually belongs to this email.
        // Prevents someone from pairing a valid token for user A with user B's email.
        var userId = userIdResult.Value;

        if (!userId.HasValue)
            return IdentityErrors.InvalidOrExpiredResetLink;

        if (userId.Value != userEmail.Value.UserId)
            return IdentityErrors.InvalidOrExpiredResetLink;

        // 5. Attempt the password reset
        var resetResult = await identityService.ResetPasswordAsync(
            userIdResult.Value!.Value,
            request.NewPassword,
            ct);

        // 6. If it failed (e.g. weak password), leave the token intact so the user can retry
        if (resetResult.IsFailure)
            return resetResult.Error!;

        // 7. Only now that the reset succeeded, consume (delete) the token — one-time use
        await resetPasswordRepository.DeleteAsync(hashedToken, ct);

        return new ConfirmPasswordResetResponse(
                UserId: userId.Value,
                Message: "Your password has been reset successfully. You can now log in with your new password.");
    }

    // Hashes a token using SHA256. Must match the hashing logic in ResetPasswordHandler.
    private static string HashToken(string token)
    {
        byte[] tokenBytes = Encoding.UTF8.GetBytes(token);
        byte[] hashBytes = SHA256.HashData(tokenBytes);
        return Convert.ToHexString(hashBytes).ToLowerInvariant();
    }
}
