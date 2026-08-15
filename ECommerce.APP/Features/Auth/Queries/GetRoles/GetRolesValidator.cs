using FluentValidation;

namespace ECommerce.APP.Features.Auth.Queries.GetRoles;

public sealed class GetRolesValidator : AbstractValidator<GetRolesQuery>
{
    public GetRolesValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("User ID is required.")
            .WithErrorCode("Identity.UserId.Required");
    }
}
