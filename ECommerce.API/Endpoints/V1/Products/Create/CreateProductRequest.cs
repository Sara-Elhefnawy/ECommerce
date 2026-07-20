using ECommerce.APP.Features.Products.Commands;
using ECommerce.APP.Mediator;
using ECommerce.Domain.Common;

namespace ECommerce.API.Endpoints.V1.Products.Create;

public sealed class CreateProductRequest : IRequest<ResultOfT<CreateProductResponse>>
{
    public required string Name { get; set; } = default!;
    public required string Description { get; set; } = default!;
    public required IFormFile ImageFile { get; set; } = default!;
    public required decimal Price { get; set; } = default!;
    public required string BrandName { get; set; } = default!;
    public required string TypeName{ get; set; } = default!;
}
