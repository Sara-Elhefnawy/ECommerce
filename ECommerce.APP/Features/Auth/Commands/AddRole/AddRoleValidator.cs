using FluentValidation;

namespace ECommerce.APP.Features.Auth.Commands.AddRole;

public sealed class AddRoleValidator : AbstractValidator<AddRoleCommand>
{
    public AddRoleValidator()
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
