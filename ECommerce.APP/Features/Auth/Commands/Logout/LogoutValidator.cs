using FluentValidation;

namespace ECommerce.APP.Features.Auth.Commands.Logout;

public sealed class LogoutValidator : AbstractValidator<LogoutCommand>
{
    public LogoutValidator()
    {
        RuleFor(x => x.RefreshToken)
            .NotEmpty()
            .WithMessage("Refresh token is required.")
            .WithErrorCode("Identity.RefreshToken.Required");
    }
}
