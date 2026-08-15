using FluentValidation;

namespace ECommerce.APP.Features.Users.Commands.ConfirmPasswordReset;

public sealed class ConfirmPasswordResetValidator : AbstractValidator<ConfirmPasswordResetCommand>
{
    public ConfirmPasswordResetValidator()
    {
        RuleFor(x => x.Email)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
                .WithMessage("Email is required.")
                .WithErrorCode("Identity.Email.Required")
            .EmailAddress()
                .WithMessage("A valid email address is required.")
                .WithErrorCode("Identity.Email.Invalid");

        RuleFor(x => x.PasswordResetToken)
            .NotEmpty()
            .WithMessage("Password reset token is required.")
            .WithErrorCode("Identity.PasswordResetToken.Required");

        RuleFor(x => x.NewPassword)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
                .WithMessage("New password is required.")
                .WithErrorCode("Identity.NewPassword.Required")
            .MinimumLength(8)
                .WithMessage("New password must be at least 8 characters.")
                .WithErrorCode("Identity.NewPassword.TooShort");
    }
}
