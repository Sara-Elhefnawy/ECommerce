using FluentValidation;

namespace ECommerce.APP.Features.Auth.Commands.Login;

public sealed class LoginValidator : AbstractValidator<LoginCommand>
{
    public LoginValidator()
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
            .NotEmpty()
            .WithMessage("Password is required.")
            .WithErrorCode("Identity.Password.Required");
    }
}
