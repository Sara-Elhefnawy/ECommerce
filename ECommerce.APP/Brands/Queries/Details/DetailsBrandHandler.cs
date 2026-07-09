using ECommerce.APP.Abstractions.Mediator;
using ECommerce.Domain.Abstractions.Repositories;
using ECommerce.Domain.Common;
using ECommerce.Domain.Entities;

namespace ECommerce.APP.Brands.Queries.Details;

public sealed class DetailsBrandHandler(IReadRepository<ProductBrand> repository) :

    IRequestHandler<DetailsBrandQuery, ResultOfT<DetailsBrandResponse>>
{
    public async Task<ResultOfT<DetailsBrandResponse>> Handle(
        DetailsBrandQuery request, CancellationToken ct = default)
    {
        var brand = await repository.FirstOrDefaultAsync(new DetailsBrandSpecification(request.Id), ct);

        return brand is null
            ? ProductErrors.NotFound
            : brand;
    }
}
