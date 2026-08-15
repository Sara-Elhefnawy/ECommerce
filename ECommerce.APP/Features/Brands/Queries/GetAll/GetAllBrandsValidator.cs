using FluentValidation;

namespace ECommerce.APP.Features.Brands.Queries.GetAll;

public sealed class GetAllBrandsValidator : AbstractValidator<GetAllBrandsQuery>
{
    public GetAllBrandsValidator()
    {
        RuleFor(x => x.Count)
            .GreaterThan(0)
            .When(x => x.Count.HasValue)
            .WithMessage("Count must be greater than zero.")
            .WithErrorCode("Type.Count.Invalid");
    }
}
