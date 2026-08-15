using ECommerce.APP.Email;
using ECommerce.APP.Identity;
using ECommerce.APP.Mediator;
using ECommerce.APP.Settings;
using ECommerce.Domain.Entities.Errors;
using ECommerce.Domain.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;
using Microsoft.Extensions.Hosting;

namespace ECommerce.APP.Features.Auth.Commands.Register;

// Registrations doesn't have any token generations
// it creates user + IsConfirmed = false 
// then send code to email to confirm it => POST/confirm-email
public sealed class RegisterHandler(
    IIdentityService identityService,
    IEmailVerification emailVerification,
    IEmailSender emailSender,
    IOptions<EmailVerificationSettings> settings,
    IHostEnvironment env,
    ILogger<RegisterHandler> logger)
    : IRequestHandler<RegisterCommand, ResultOfT<EmailSentResponse>>
{
    public const string RegisteredMessage =
        "Registration successful. A verification code was sent to your email. Confirm your email before logging in.";

    public async Task<ResultOfT<EmailSentResponse>> Handle(
        RegisterCommand request,
        CancellationToken ct = default)
    {
        var email = request.Email.Trim();

        // prevents duplicate/orphaned registrations
        var existing = await identityService.GetUserByEmailAsync(email, ct);
        if (existing.IsSuccess)
        {
            // Confirmed account → Conflict (409)
            if (await identityService.IsEmailConfirmedAsync(email, ct))
                return ResultOfT<EmailSentResponse>.Failure(IdentityErrors.EmailAlreadyExists);

            return ResultOfT<EmailSentResponse>.Failure(
                IdentityErrors.RegistrationAlreadyStarted);
        }

        var createResult = await identityService.CreateUserAsync(
            email,
            request.Password,
            request.UserDisplayName,
            ct);

        if (createResult.IsFailure)
            return ResultOfT<EmailSentResponse>.Failure(createResult.Error!);

        var length = settings.Value.CodeLength;

        if (length is < 4 or > 10)
            length = 6;

        var max = (int)Math.Pow(10, length);

        var code = RandomNumberGenerator.GetInt32(0, max).ToString($"D{length}");

        // send code to verifiy email
        await emailVerification.SaveAsync(email, code, ct);

        var sendResult = await emailSender.SendAsync(
            email,
            "Confirm your ECommerce account",
            $"Your verification code is: {code}\n\nThis code expires in {settings.Value.ExpirationMinutes} minutes.\n\nYou cannot sign in until you confirm this email.",
            ct);

        if (sendResult.IsFailure)
        {
            if (env.IsDevelopment())
                logger.LogWarning("Email failed — verification code for {Email}: {Code}", email, code);

            return ResultOfT<EmailSentResponse>.Failure(sendResult.Error!);
        }

        return ResultOfT<EmailSentResponse>.Ok(
            new EmailSentResponse(email, VerificationCodeResent: false, RegisteredMessage));
    }
}
