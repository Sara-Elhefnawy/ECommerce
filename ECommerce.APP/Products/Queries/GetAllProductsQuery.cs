using ECommerce.APP.Products.Responses;
using ECommerce.APP.Products.Specifications;
using ECommerce.Domain.Abstractions.Repositories;
using ECommerce.Domain.Common;
using ECommerce.Domain.Entities;

namespace ECommerce.APP.Products.Queries;

public sealed class GetAllProductsQuery(IReadRepository<Product> repository)
{
    public async Task<ResultOfT<IReadOnlyList<GetAllProductsResponse>>> Execute(CancellationToken ct = default)
    {
        var products = await repository.ListAsync(new GetAllProductsSpecification(), ct);

        if (products is null || !products.Any())
            return ProductErrors.NotFound;

        return ResultOfT<IReadOnlyList<GetAllProductsResponse>>.Ok(products);
    }
}
