using ECommerce.APP.Features.Products.Queries.GetPagination.Constants;
using FluentValidation;

namespace ECommerce.APP.Features.Products.Queries.GetPagination;

public sealed class GetProductsPaginationValidator : AbstractValidator<GetProductsPaginationQuery>
{
    public GetProductsPaginationValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThan(0)
            .WithMessage("Page number must be greater than zero.")
            .WithErrorCode("Product.PageNumber.Invalid");

        RuleFor(x => x.PageSize)
            .GreaterThan(0)
            .WithMessage("Page size must be greater than zero.")
            .WithErrorCode("Product.PageSize.Invalid")
            .LessThanOrEqualTo(ValidatorsConstant.MaxPageSize)
            .WithMessage($"Page size cannot exceed {ValidatorsConstant.MaxPageSize}.")
            .WithErrorCode("Product.PageSize.TooLarge");

        RuleFor(x => x.SearchTerm)
            .MaximumLength(150)
            .When(x => x.SearchTerm is not null)
            .WithMessage("Search term cannot exceed 150 characters.")
            .WithErrorCode("Product.SearchTerm.TooLong");

        RuleFor(x => x.BrandId)
            .NotEqual(Guid.Empty)
            .When(x => x.BrandId.HasValue)
            .WithMessage("Brand ID must be a valid identifier.")
            .WithErrorCode("Product.BrandId.Invalid");

        RuleFor(x => x.TypeId)
            .NotEqual(Guid.Empty)
            .When(x => x.TypeId.HasValue)
            .WithMessage("Type ID must be a valid identifier.")
            .WithErrorCode("Product.TypeId.Invalid");
    }
}
