using FluentValidation;

namespace ECommerce.APP.Features.Auth.Commands.ResendVerification;

public sealed class ResendVerificationCommandValidator
    : AbstractValidator<ResendVerificationCommand>
{
    public ResendVerificationCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress();
    }
}
