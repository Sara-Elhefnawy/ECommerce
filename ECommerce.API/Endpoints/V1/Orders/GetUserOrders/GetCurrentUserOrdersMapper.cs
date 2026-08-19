using ECommerce.APP.Features.Orders.Enums;
using ECommerce.APP.Features.Orders.Queries.GetUserOrders;
using ECommerce.APP.Features.Products.Queries.GetPagination.Constants;
using ECommerce.APP.Features.Products.Queries.GetPagination.Enums;

namespace ECommerce.API.Endpoints.V1.Orders.GetUserOrders;

public static class GetCurrentUserOrdersMapper
{
    public static GetCurrentUserOrdersQuery ToQuery(this GetCurrentUserOrdersRequest request)
    {
        // Enum.Parse instead of TryParse. The validator already guaranteed it's valid.
        OrderSortType? sortBy = string.IsNullOrWhiteSpace(request.SortBy)
        ? null
        : Enum.Parse<OrderSortType>(request.SortBy, ignoreCase: true);

        return new(
            request.PageNumber ?? ValidatorsConstant.DefaultPageNumber,
            request.PageSize ?? ValidatorsConstant.DefaultPageSize,
            request.SearchTerm,
            sortBy,
            request.IsSortDescending ?? false);
    }
}
