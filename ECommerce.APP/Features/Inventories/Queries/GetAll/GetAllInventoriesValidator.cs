using FluentValidation;

namespace ECommerce.APP.Features.Inventories.Queries.GetAll;

public sealed class GetAllInventoriesValidator : AbstractValidator<GetAllInventoriesQuery>
{
    public GetAllInventoriesValidator()
    {
        RuleFor(x => x.Count)
            .GreaterThan(0)
            .When(x => x.Count.HasValue)
            .WithMessage("Count must be greater than zero.")
            .WithErrorCode("Type.Count.Invalid");
    }
}
