namespace ECommerce.APP.Features.Orders.Queries.GetUserOrders;

public sealed class GetCurrentUserOrdersCountSpecification : GetCurrentUserOrdersSpecification
{
    public GetCurrentUserOrdersCountSpecification(
        Guid userId,
        string? searchTerm) 
        : base(userId, searchTerm)
    {
    }
}
