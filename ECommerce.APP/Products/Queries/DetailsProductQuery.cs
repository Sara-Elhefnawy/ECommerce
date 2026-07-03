using ECommerce.APP.Products.Responses;
using ECommerce.Domain.Common;

namespace ECommerce.APP.Products.Queries;

public sealed class DetailsProductQuery(IProductQueryService queryService)
{
    // DetailsProductQuery's constructor takes id directly
    // and minimal API DI can't inject a runtime route value (id)
    // into a constructor automatically,
    //      so you had to fall back to manually grabbing IProductQueryService
    //      and new-ing it up yourself, which then made it easy to "forget"
    //      and just call queryService.GetByIdAsync directly instead
    // Solution:
    // move id out of the constructor and into Execute(),
    // so the query becomes a normal injectable service
    // like GetAllProductsQuery
    public async Task<ResultOfT<DetailsProductResponse>> Execute(Guid id, CancellationToken ct = default)
    {
        var product = await queryService.GetByIdAsync(id, ct);

        return product is null
            ? ProductErrors.NotFound
            : product;
    }
}
