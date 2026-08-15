using FluentValidation;

namespace ECommerce.APP.Features.Types.Queries.GetAll;

public sealed class GetAllTypesValidator : AbstractValidator<GetAllTypesQuery>
{
    public GetAllTypesValidator()
    {
        RuleFor(x => x.Count)
            .GreaterThan(0)
            .When(x => x.Count.HasValue)
            .WithMessage("Count must be greater than zero.")
            .WithErrorCode("Type.Count.Invalid");
    }
}
