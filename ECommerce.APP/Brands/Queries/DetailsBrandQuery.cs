using ECommerce.APP.Brands.Response;
using ECommerce.APP.Brands.Specifications;
using ECommerce.Domain.Abstractions.Repositories;
using ECommerce.Domain.Common;
using ECommerce.Domain.Entities;

namespace ECommerce.APP.Brands.Queries;

public sealed class DetailsBrandQuery(IReadRepository<ProductBrand> repository)
{
    public async Task<ResultOfT<DetailsBrandResponse>> Execute(Guid id, CancellationToken ct = default)
    {
        var brand = await repository.FirstOrDefaultAsync(new DetailsBrandSpecification(id), ct);

        return brand is null
            ? ProductErrors.NotFound
            : brand;
    }
}
