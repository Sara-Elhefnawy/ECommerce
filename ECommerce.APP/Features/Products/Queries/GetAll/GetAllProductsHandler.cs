using ECommerce.APP.Abstractions.Mediator;
using ECommerce.Domain.Abstractions.Repositories;
using ECommerce.Domain.Common;
using ECommerce.Domain.Entities;

namespace ECommerce.APP.Features.Products.Queries.GetAll;

public sealed class GetAllProductsHandler(IReadRepository<Product> repository) : 
    IRequestHandler<GetAllProductsQuery, ResultOfT<IReadOnlyList<GetAllProductsResponse>>>
{
    public async Task<ResultOfT<IReadOnlyList<GetAllProductsResponse>>> Handle(
        GetAllProductsQuery request, CancellationToken ct = default)
    {
        var products = await repository.ListAsync(new GetAllProductsSpecification(request.Count), ct);

        return ResultOfT<IReadOnlyList<GetAllProductsResponse>>.Ok(products);
    }
}
