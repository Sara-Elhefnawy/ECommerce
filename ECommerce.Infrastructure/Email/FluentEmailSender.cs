using ECommerce.APP.Email;
using ECommerce.Domain.Entities.Errors;
using ECommerce.Domain.Results;
using FluentEmail.Core;
using Microsoft.Extensions.Logging;

namespace ECommerce.Infrastructure.Email;

public sealed class FluentEmailSender(
    IFluentEmailFactory emailFactory,
    ILogger<FluentEmailSender> logger) 
    : IEmailSender
{
    public async Task<Result> SendAsync(
        string toEmail,
        string subject,
        string body,
        CancellationToken ct = default)
    {
        _ = ct;

        try
        {
            var response = await emailFactory
                .Create()
                .To(toEmail)
                .Subject(subject)
                .Body(body)
                .SendAsync();

            if (!response.Successful)
                return Result.Failure(IdentityErrors.EmailSendFailed);

            return Result.Ok();
        }
        catch (Exception ex)
        {
            // Widened from SmtpException on purpose, temporarily: we don't yet
            // know which SMTP backend FluentEmail is using under the hood
            // (System.Net.Mail vs MailKit throw different exception types),
            // and the previous narrow catch let the real exception slip
            // through uncaught. Logging ex here is the point — once we see
            // the actual type in the logs, narrow this back down to that
            // specific type instead of bare Exception.
            logger.LogError(ex, "Failed to send email to {ToEmail}", toEmail);

            // SMTP server unreachable/refused/auth failure, etc. — this is an
            // expected failure mode (server down, network blip), not a bug.
            // Convert it into the same Result.Failure the caller already
            // handles, instead of letting it become an unhandled 500 via
            // GlobalExceptionMiddleware.
            return Result.Failure(IdentityErrors.EmailSendFailed);
        }
    }
}
