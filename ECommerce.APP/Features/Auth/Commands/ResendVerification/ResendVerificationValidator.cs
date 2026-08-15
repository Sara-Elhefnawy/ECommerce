using FluentValidation;

namespace ECommerce.APP.Features.Auth.Commands.ResendVerification;

public sealed class ResendVerificationCommandValidator : AbstractValidator<ResendVerificationCommand>
{
    public ResendVerificationCommandValidator()
    {
        RuleFor(x => x.Email)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
                .WithMessage("Email is required.")
                .WithErrorCode("Identity.Email.Required")
            .EmailAddress()
                .WithMessage("A valid email address is required.")
                .WithErrorCode("Identity.Email.Invalid");
    }
}
