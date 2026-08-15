using FluentValidation;

namespace ECommerce.APP.Features.Users.Commands.UpdateUser;

public sealed class UpdateUserValidator : AbstractValidator<UpdateUserCommand>
{
    public UpdateUserValidator()
    {
        RuleFor(x => x.UserDisplayName.Value)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
                .WithMessage("Display name cannot be empty.")
                .WithErrorCode("Identity.UserDisplayName.Required")
            .MaximumLength(100)
                .WithMessage("Display name cannot exceed 100 characters.")
                .WithErrorCode("Identity.UserDisplayName.TooLong")
            .When(x => x.UserDisplayName.IsSet);
    }
}
