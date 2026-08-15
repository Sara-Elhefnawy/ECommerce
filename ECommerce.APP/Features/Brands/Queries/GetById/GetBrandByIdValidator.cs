using FluentValidation;

namespace ECommerce.APP.Features.Brands.Queries.GetById;

public sealed class GetBrandByIdValidator : AbstractValidator<GetBrandByIdQuery>
{
    public GetBrandByIdValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("ID must not be empty.")
            .WithErrorCode("Brand.Id.Required");
    }
}
