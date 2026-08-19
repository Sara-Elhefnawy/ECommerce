using ECommerce.APP.Features.Orders.Enums;

namespace ECommerce.APP.Features.Orders.Specifications;

public sealed class GetCurrentUserOrdersPagedSpecification : GetCurrentUserOrdersSpecification
{
    public GetCurrentUserOrdersPagedSpecification(
        Guid userId,
        string? searchTerm,
        OrderSortType? sortBy,
        bool isSortDescending,
        int pageNumber,
        int pageSize)
        : base(userId, searchTerm)
    {
        Query
            .Include(o => o.Items)
            .AsNoTracking();

        ApplySorting(sortBy, isSortDescending);

        var skip = (pageNumber - 1) * pageSize;

        Query
            .Skip(skip)
            .Take(pageSize);
    }

    private void ApplySorting(
        OrderSortType? sortBy,
        bool isSortDescending)
    {
        switch (sortBy)
        {
            case OrderSortType.Total:
                if (isSortDescending)
                {
                    Query.OrderByDescending(o => o.Total)
                         .ThenByDescending(o => o.CreatedAt)
                         .ThenByDescending(o => o.Id);
                }
                else
                {
                    Query.OrderBy(o => o.Total)
                         .ThenByDescending(o => o.CreatedAt)
                         .ThenBy(o => o.Id);
                }

                break;

            case OrderSortType.ItemsTotal:
                if (isSortDescending)
                {
                    Query.OrderByDescending(o => o.ItemsTotal)
                         .ThenByDescending(o => o.CreatedAt)
                         .ThenByDescending(o => o.Id);
                }
                else
                {
                    Query.OrderBy(o => o.ItemsTotal)
                         .ThenByDescending(o => o.CreatedAt)
                         .ThenBy(o => o.Id);
                }

                break;

            case OrderSortType.ShippingCost:
                if (isSortDescending)
                {
                    Query.OrderByDescending(o => o.ShippingCost)
                         .ThenByDescending(o => o.CreatedAt)
                         .ThenByDescending(o => o.Id);
                }
                else
                {
                    Query.OrderBy(o => o.ShippingCost)
                         .ThenByDescending(o => o.CreatedAt)
                         .ThenBy(o => o.Id);
                }

                break;

            case OrderSortType.Status:
                if (isSortDescending)
                {
                    Query.OrderByDescending(o => o.Status)
                         .ThenByDescending(o => o.CreatedAt)
                         .ThenByDescending(o => o.Id);
                }
                else
                {
                    Query.OrderBy(o => o.Status)
                         .ThenByDescending(o => o.CreatedAt)
                         .ThenBy(o => o.Id);
                }

                break;

            case OrderSortType.CreatedAt:
            default:
                if (isSortDescending)
                {
                    Query.OrderByDescending(o => o.CreatedAt)
                         .ThenByDescending(o => o.Id);
                }
                else
                {
                    Query.OrderBy(o => o.CreatedAt)
                         .ThenBy(o => o.Id);
                }

                break;
        }
    }
}
