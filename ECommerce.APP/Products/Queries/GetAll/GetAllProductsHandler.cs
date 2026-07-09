using ECommerce.APP.Abstractions.Mediator;
using ECommerce.Domain.Abstractions.Repositories;
using ECommerce.Domain.Common;
using ECommerce.Domain.Entities;

namespace ECommerce.APP.Products.Queries.GetAll;

public sealed class GetAllProductsHandler(IReadRepository<Product> repository) : 
    IRequestHandler<GetAllProductsQuery, ResultOfT<IReadOnlyList<GetAllProductsResponse>>>
{
    public async Task<ResultOfT<IReadOnlyList<GetAllProductsResponse>>> Handle(
        GetAllProductsQuery request, CancellationToken ct = default)
    {
        var products = await repository.ListAsync(new GetAllProductsSpecification(), ct);

        if (products is null || !products.Any())
            return ProductErrors.NotFound;

        return ResultOfT<IReadOnlyList<GetAllProductsResponse>>.Ok(products);
    }
}
