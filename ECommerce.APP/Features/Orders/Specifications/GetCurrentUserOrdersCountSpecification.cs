namespace ECommerce.APP.Features.Orders.Specifications;

public sealed class GetCurrentUserOrdersCountSpecification : GetCurrentUserOrdersSpecification
{
    public GetCurrentUserOrdersCountSpecification(
        Guid userId,
        string? searchTerm) 
        : base(userId, searchTerm)
    {
    }
}
