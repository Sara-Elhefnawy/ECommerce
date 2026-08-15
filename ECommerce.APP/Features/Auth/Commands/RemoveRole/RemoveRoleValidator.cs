using FluentValidation;

namespace ECommerce.APP.Features.Auth.Commands.RemoveRole;

public sealed class RemoveRoleValidator : AbstractValidator<RemoveRoleCommand>
{
    public RemoveRoleValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("User ID is required.")
            .WithErrorCode("Identity.UserId.Required");

        RuleFor(x => x.Role)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
                .WithMessage("Role is required.")
                .WithErrorCode("Identity.Role.Required")
            .MaximumLength(50)
                .WithMessage("Role cannot exceed 50 characters.")
                .WithErrorCode("Identity.Role.TooLong");
    }
}
