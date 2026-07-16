using ECommerce.APP.Features.Products.Queries.GetPagination.Enums;
using FluentValidation;

namespace ECommerce.API.Endpoints.V1.Products.GetPagination;

public class GetProductsPaginationRequestValidator : AbstractValidator<GetProductsPaginationRequest>
{
    public GetProductsPaginationRequestValidator()
    {
        RuleFor(x => x.SortBy)
            .Must(x => string.IsNullOrWhiteSpace(x) ||
                       Enum.TryParse<SortType>(x, true, out _))
            .WithMessage($"SortBy must be one of: {string.Join(", ", Enum.GetNames<SortType>())}");
    }
}
