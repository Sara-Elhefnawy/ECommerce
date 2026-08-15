using FluentValidation;

namespace ECommerce.APP.Features.Brands.Queries.GetByName;

public sealed class GetBrandByNameValidator : AbstractValidator<GetBrandByNameQuery>
{
    public GetBrandByNameValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(100)
            .Must(name => name.Any(char.IsLetter))
            .WithMessage("Brand name can not be empty.")
            .WithErrorCode("Brand.Name.Required");
    }
}
