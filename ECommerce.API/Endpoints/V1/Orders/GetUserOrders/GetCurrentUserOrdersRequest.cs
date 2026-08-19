namespace ECommerce.API.Endpoints.V1.Orders.GetUserOrders;

public sealed class GetCurrentUserOrdersRequest
{
    public int? PageNumber { get; init; }
    public int? PageSize { get; init; }
    public string? SearchTerm { get; init; }
    public string? SortBy { get; init; }
    public bool? IsSortDescending { get; init; }
}
