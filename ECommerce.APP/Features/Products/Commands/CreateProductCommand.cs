using ECommerce.APP.Abstractions.Mediator;
using ECommerce.Domain.Common;

namespace ECommerce.APP.Features.Products.Commands;

public sealed record CreateProductCommand(
    string Name,
    string Description,
    decimal Price,
    string BrandName,
    string TypeName,
    Stream ImageStream,
    string ImageFileName,
    long ImageLength,
    string ImageContentType) : IRequest<ResultOfT<CreateProductResponse>>;
