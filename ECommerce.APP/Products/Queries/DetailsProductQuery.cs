using ECommerce.APP.Products.Responses;
using ECommerce.APP.Products.Specifications;
using ECommerce.Domain.Abstractions.Repositories;
using ECommerce.Domain.Common;
using ECommerce.Domain.Entities;

namespace ECommerce.APP.Products.Queries;

public sealed class DetailsProductQuery(IReadRepository<Product> repository)
{
    public async Task<ResultOfT<DetailsProductResponse>> Execute(Guid id, CancellationToken ct = default)
    {
        var product = await repository.FirstOrDefaultAsync(new DetailsProductSpecification(id), ct);

        return product is null
            ? ProductErrors.NotFound
            : product;
    }
}
