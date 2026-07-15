using ECommerce.APP.Features.Products.Queries.GetPagination;
using ECommerce.APP.Features.Products.Queries.GetPagination.Constants;

namespace ECommerce.API.Endpoints.V1.Products.GetPagination;

public static class GetProductsPaginationMapper
{
    public static GetProductsPaginationQuery ToQuery(this GetProductsPaginationRequest request)
        => new(
            request.PageNumber ?? ValidatorsConstant.DefaultPageNumber,
            request.PageSize ?? ValidatorsConstant.DefaultPageSize,
            request.SearchTerm,
            request.BrandId,
            request.TypeId,
            request.SortBy,
            request.IsSortDescending ?? false);
}
