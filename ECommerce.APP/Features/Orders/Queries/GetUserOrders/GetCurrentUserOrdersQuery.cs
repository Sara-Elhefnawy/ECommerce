using ECommerce.APP.Features.Orders.DTOs;
using ECommerce.APP.Features.Orders.Enums;
using ECommerce.APP.Features.Products.Queries.GetPagination.Constants;
using ECommerce.APP.Mediator;
using ECommerce.Domain.Results;

namespace ECommerce.APP.Features.Orders.Queries.GetUserOrders;

public sealed record GetCurrentUserOrdersQuery(
    int PageNumber = ValidatorsConstant.DefaultPageNumber,
    int PageSize = ValidatorsConstant.DefaultPageSize,
    string? SearchTerm = null,
    OrderSortType? SortBy = OrderSortType.CreatedAt,
    bool IsSortDescending = false) : IRequest<ResultOfT<PagedResult<OrderResponse>>>;
