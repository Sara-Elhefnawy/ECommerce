using ECommerce.Domain.Results;

namespace ECommerce.APP.Email;

public interface IEmailSender
{
    Task<Result> SendAsync(
        string toEmail,
        string subject,
        string body,
        CancellationToken ct = default);
}
