namespace ECommerce.API.Endpoints.V1.Products.GetPagination;

public class GetProductsPaginationRequest
{
    public int? PageNumber { get; init; }
    public int? PageSize { get; init; }
    public string? SearchTerm { get; init; }
    public Guid? BrandId { get; init; }
    public Guid? TypeId { get; init; }
    public string? SortBy { get; init; }
    public bool? IsSortDescending { get; init; }
}
