using ECommerce.APP.Cachings.ResetPassword;
using ECommerce.APP.Email;
using ECommerce.APP.Identity;
using ECommerce.APP.Mediator;
using ECommerce.APP.Settings;
using ECommerce.Domain.Results;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace ECommerce.APP.Features.Users.Commands.ResetPassword;

public sealed class ResetPasswordHandler(
    IIdentityService identityService,
    IResetPasswordRepository resetPasswordRepository,
    IEmailSender emailSender,
    IOptions<ResetPasswordSettings> settings,
    IHostEnvironment env,
    ILogger<ResetPasswordHandler> logger)
    : IRequestHandler<ResetPasswordCommand, ResultOfT<ResetPasswordResponse>>
{
    private const string SuccessMessage =
        "If an account with that email exists, a password reset link has been sent.";

    public async Task<ResultOfT<ResetPasswordResponse>> Handle(
        ResetPasswordCommand request,
        CancellationToken ct = default)
    {
        var email = request.Email.Trim();

        var userResult = await identityService.GetUserByEmailAsync(email, ct);

        if (userResult.IsSuccess)
        {
            var user = userResult.Value;

            var token = Convert.ToHexString(
                RandomNumberGenerator.GetBytes(32))
                .ToLowerInvariant();

            var saveResult = await resetPasswordRepository.SaveAsync(
                HashToken(token),
                user.UserId,
                ct);

            if (saveResult.IsFailure)
                return ResultOfT<ResetPasswordResponse>.Failure(saveResult.Error!);

            var resetLink =
                $"{settings.Value.FrontendResetPasswordUrl}?token={HttpUtility.UrlEncode(token)}";

            var sendResult = await emailSender.SendAsync(
                user.Email,
                "Reset your password",
                $"Reset your password using the link below:\n\n{resetLink}",
                ct);

            if (sendResult.IsFailure)
            {
                if (env.IsDevelopment())
                    logger.LogWarning("Email failed — reset link for {Email}: {ResetLink}", user.Email, resetLink);

                return ResultOfT<ResetPasswordResponse>.Failure(sendResult.Error!);
            }
        }

        return ResultOfT<ResetPasswordResponse>.Ok(
            new ResetPasswordResponse(SuccessMessage));
    }

    private static string HashToken(string token)
    {
        return Convert.ToHexString(
            SHA256.HashData(Encoding.UTF8.GetBytes(token)))
            .ToLowerInvariant();
    }
}
