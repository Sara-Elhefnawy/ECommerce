using Microsoft.AspNetCore.Http;

namespace ECommerce.APP.Products.Responses;

public class CreateProductRequest
{
    public required string Name { get; set; } = default!;
    public required string Description { get; set; } = default!;
    public required IFormFile ImageFile { get; set; } = default!;
    public required decimal Price { get; set; } = default!;
    public required Guid BrandId { get; set; } = default!;
    public required Guid TypeId { get; set; } = default!;
}
