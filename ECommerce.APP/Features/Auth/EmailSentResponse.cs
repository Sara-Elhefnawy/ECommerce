namespace ECommerce.APP.Features.Auth;

public sealed record EmailSentResponse(
    string Email,                        // Address the verification code was sent to
    bool VerificationCodeResent,         // True when the email already belonged to an unconfirmed account and a fresh code was sent
    string Message);                     // User-facing success message for this outcome
