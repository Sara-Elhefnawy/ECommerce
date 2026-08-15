using FluentValidation;

namespace ECommerce.APP.Features.Auth.Commands.Register;

public sealed class RegisterValidator : AbstractValidator<RegisterCommand>
{
    public RegisterValidator()
    {
        RuleFor(x => x.Email)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
                .WithMessage("Email is required.")
                .WithErrorCode("Identity.Email.Required")
            .EmailAddress()
                .WithMessage("A valid email address is required.")
                .WithErrorCode("Identity.Email.Invalid");

        RuleFor(x => x.Password)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
                .WithMessage("Password is required.")
                .WithErrorCode("Identity.Password.Required")
            .MinimumLength(8)
                .WithMessage("Password must be at least 8 characters.")
                .WithErrorCode("Identity.Password.TooShort")
            .Matches("[0-9]")
                .WithMessage("Password must contain at least one digit.")
            .Matches("[A-Z]")
                .WithMessage("Password must contain at least one uppercase letter.")
            .Matches("[^a-zA-Z0-9]")
                .WithMessage("Password must contain at least one special character.");

        RuleFor(x => x.UserDisplayName)
            .MaximumLength(100)
            .When(x => x.UserDisplayName is not null)
            .WithMessage("Display name cannot exceed 100 characters.")
            .WithErrorCode("Identity.UserDisplayName.TooLong");
    }
}
