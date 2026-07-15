using ECommerce.APP.Features.Products.Queries.GetPagination.Constants;
using FluentValidation;

namespace ECommerce.APP.Features.Products.Queries.GetPagination;

public class GetProductsPaginationValidator : AbstractValidator<GetProductsPaginationQuery>
{
    public GetProductsPaginationValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThanOrEqualTo(ValidatorsConstant.DefaultPageNumber)
            .WithMessage("Invalid page number");

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, ValidatorsConstant.DefaultMaxPageSize)
            .WithMessage("Invalid page size");
    }
}
