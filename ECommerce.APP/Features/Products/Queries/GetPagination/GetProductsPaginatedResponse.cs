namespace ECommerce.APP.Features.Products.Queries.GetPagination;

public record GetProductsPaginatedResponse(
    Guid Id,
    string Name,
    string Description,
    string PictureUrl,
    decimal Price,
    string TypeName,
    string BrandName
)
{ }
