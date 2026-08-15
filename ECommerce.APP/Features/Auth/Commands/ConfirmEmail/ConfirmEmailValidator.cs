using FluentValidation;

namespace ECommerce.APP.Features.Auth.Commands.ConfirmEmail;

public sealed class ConfirmEmailValidator : AbstractValidator<ConfirmEmailCommand>
{
    public ConfirmEmailValidator()
    {
        RuleFor(x => x.Email)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
                .WithMessage("Email is required.")
                .WithErrorCode("Identity.Email.Required")
            .EmailAddress()
                .WithMessage("A valid email address is required.")
                .WithErrorCode("Identity.Email.Invalid");

        RuleFor(x => x.Code)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
                .WithMessage("Verification code is required.")
                .WithErrorCode("Identity.VerificationCode.Required")
            .Matches(@"^\d{4,10}$")
                .WithMessage("Verification code must contain between 4 and 10 digits.")
                .WithErrorCode("Identity.VerificationCode.Invalid");
    }
}
