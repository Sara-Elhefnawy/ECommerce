using ECommerce.APP.Features.Products.Queries.GetPagination.Constants;
using FluentValidation;

namespace ECommerce.APP.Features.Orders.Queries.GetUserOrders;

public sealed class GetCurrentUserOrdersValidator : AbstractValidator<GetCurrentUserOrdersQuery>
{
    public GetCurrentUserOrdersValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThan(0)
            .WithMessage("Page number must be greater than zero.")
            .WithErrorCode("Order.PageNumber.Invalid");

        RuleFor(x => x.PageSize)
            .GreaterThan(0)
            .WithMessage("Page size must be greater than zero.")
            .WithErrorCode("Order.PageSize.Invalid")
            .LessThanOrEqualTo(ValidatorsConstant.MaxPageSize)
            .WithMessage($"Page size cannot exceed {ValidatorsConstant.MaxPageSize}.")
            .WithErrorCode("Order.PageSize.TooLarge");

        RuleFor(x => x.SearchTerm)
            .MaximumLength(150)
            .When(x => x.SearchTerm is not null)
            .WithMessage("Search term cannot exceed 150 characters.")
            .WithErrorCode("Order.SearchTerm.TooLong");
    }
}
