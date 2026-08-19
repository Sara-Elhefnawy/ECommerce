using ECommerce.APP.Features.Orders.DTOs;
using ECommerce.APP.Features.Orders.Mapper;
using ECommerce.APP.Features.Orders.Specifications;
using ECommerce.APP.Identity;
using ECommerce.APP.Mediator;
using ECommerce.Domain.Abstractions.Repositories;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Entities.Errors;
using ECommerce.Domain.Results;

namespace ECommerce.APP.Features.Orders.Queries.GetUserOrders;

public sealed class GetCurrentUserOrdersHandler(
    ICurrentUserService currentUser,
    IReadRepository<Order> orderRepository)
    : IRequestHandler<GetCurrentUserOrdersQuery, ResultOfT<PagedResult<OrderResponse>>>
{
    public async Task<ResultOfT<PagedResult<OrderResponse>>> Handle(
        GetCurrentUserOrdersQuery request,
        CancellationToken ct = default)
    {
        if (currentUser.UserId is null)
            return OrderErrors.Unauthorized;

        var pageNumber = request.PageNumber < 1 ? 1 : request.PageNumber;
        var pageSize = request.PageSize is < 1 or > 50 ? 10 : request.PageSize;

        var searchTerm = string.IsNullOrWhiteSpace(request.SearchTerm)
            ? null
            : request.SearchTerm.Trim();
        var userId = currentUser.UserId.Value;

        var totalCount = await orderRepository.CountAsync(
            new GetCurrentUserOrdersCountSpecification(userId, searchTerm),
            ct);

        var orders = await orderRepository.ListAsync(
            new GetCurrentUserOrdersPagedSpecification(userId, request.SearchTerm, request.SortBy, request.IsSortDescending, pageNumber, pageSize),
            ct);

        var items = orders.Select(OrderMapper.ToResponse).ToList();

        return new PagedResult<OrderResponse>(items, totalCount, pageNumber, pageSize);
    }
}
