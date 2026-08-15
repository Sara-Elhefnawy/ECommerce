using ECommerce.APP.Email;
using ECommerce.APP.Identity;
using ECommerce.APP.Mediator;
using ECommerce.APP.Settings;
using ECommerce.Domain.Entities.Errors;
using ECommerce.Domain.Results;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;

namespace ECommerce.APP.Features.Auth.Commands.ResendVerification;

public sealed class ResendVerificationHandler(
    IIdentityService identityService,
    IEmailVerification emailVerification,
    IEmailSender emailSender,
    IOptions<EmailVerificationSettings> settings,
    IHostEnvironment env,
    ILogger<ResendVerificationHandler> logger)
    : IRequestHandler<ResendVerificationCommand, ResultOfT<EmailSentResponse>>
{
    private const string ResentMessage =
        "A new verification code was sent to your email.";

    public async Task<ResultOfT<EmailSentResponse>> Handle(
        ResendVerificationCommand request,
        CancellationToken ct = default)
    {
        var email = request.Email.Trim();

        var userResult = await identityService.GetUserByEmailAsync(email, ct);

        if (userResult.IsFailure)
            return ResultOfT<EmailSentResponse>.Failure(
                IdentityErrors.UserNotFound);

        if (await identityService.IsEmailConfirmedAsync(email, ct))
            return ResultOfT<EmailSentResponse>.Failure(
                IdentityErrors.EmailAlreadyConfirmed);

        var length = settings.Value.CodeLength;

        if (length is < 4 or > 10)
            length = 6;

        var max = (int)Math.Pow(10, length);

        var code = RandomNumberGenerator
            .GetInt32(0, max)
            .ToString($"D{length}");

        await emailVerification.SaveAsync(email, code, ct);

        var sendResult = await emailSender.SendAsync(
            email,
            "Confirm your ECommerce account",
            $"Your verification code is: {code}\n\n" +
            $"This code expires in {settings.Value.ExpirationMinutes} minutes.\n\n" +
            "You cannot sign in until you confirm this email.",
            ct);

        if (sendResult.IsFailure)
        {
            if (env.IsDevelopment())
                logger.LogWarning("Email failed — verification code for {Email}: {Code}", email, code);

            return ResultOfT<EmailSentResponse>.Failure(sendResult.Error!);
        }

        return ResultOfT<EmailSentResponse>.Ok(
            new EmailSentResponse(
                email,
                VerificationCodeResent: true,
                ResentMessage));
    }
}
