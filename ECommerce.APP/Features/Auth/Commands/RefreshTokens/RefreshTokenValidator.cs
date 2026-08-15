using FluentValidation;

namespace ECommerce.APP.Features.Auth.Commands.RefreshTokens;

public sealed class RefreshTokenValidator : AbstractValidator<RefreshTokenCommand>
{
    public RefreshTokenValidator()
    {
        RuleFor(x => x.RefreshToken)
            .NotEmpty()
            .WithMessage("Refresh token is required.")
            .WithErrorCode("Identity.RefreshToken.Required");
    }
}
