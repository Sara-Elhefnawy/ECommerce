using ECommerce.APP.Products.Responses;
using ECommerce.Domain.Common;

namespace ECommerce.APP.Products.Queries;

public sealed class DetailsProductQuery(IProductQueryService queryService, Guid id)
{
    public async Task<ResultOfT<DetailsProductResponse>> Execute(CancellationToken ct = default)
    {
        var product = await queryService.GetByIdAsync(id, ct);

        return product is null
            ? ProductErrors.NotFound
            : product;
    }
}
