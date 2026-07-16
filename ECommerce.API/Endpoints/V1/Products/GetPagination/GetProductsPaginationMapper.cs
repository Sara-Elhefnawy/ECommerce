using ECommerce.APP.Features.Products.Queries.GetPagination;
using ECommerce.APP.Features.Products.Queries.GetPagination.Constants;
using ECommerce.APP.Features.Products.Queries.GetPagination.Enums;

namespace ECommerce.API.Endpoints.V1.Products.GetPagination;

public static class GetProductsPaginationMapper
{
    public static GetProductsPaginationQuery ToQuery(this GetProductsPaginationRequest request)
    {
        // Enum.Parse instead of TryParse. The validator already guaranteed it's valid.
        SortType? sortBy = string.IsNullOrWhiteSpace(request.SortBy)
        ? null
        : Enum.Parse<SortType>(request.SortBy, ignoreCase: true);

        return new(
            request.PageNumber ?? ValidatorsConstant.DefaultPageNumber,
            request.PageSize ?? ValidatorsConstant.DefaultPageSize,
            request.SearchTerm,
            request.BrandId,
            request.TypeId,
            sortBy,
            request.IsSortDescending ?? false);
    }
}
