using FluentValidation;

namespace ECommerce.APP.Features.Types.Queries.GetByName;

public sealed class GetTypeByNameValidator : AbstractValidator<GetTypeByNameQuery>
{
    public GetTypeByNameValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(100)
            .Must(name => name.Any(char.IsLetter))
            .WithMessage("Type name can not be empty.")
            .WithErrorCode("Type.Name.Required");
    }
}
