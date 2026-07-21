using ECommerce.APP.Mediator;
using ECommerce.Domain.Abstractions.Repositories;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Entities.Errors;
using ECommerce.Domain.Results;

namespace ECommerce.APP.Features.Products.Queries.GetAll;

public sealed class GetAllProductsHandler(IReadRepository<Product> repository) : 
    IRequestHandler<GetAllProductsQuery, ResultOfT<IReadOnlyList<GetAllProductsResponse>>>
{
    public async Task<ResultOfT<IReadOnlyList<GetAllProductsResponse>>> Handle(
        GetAllProductsQuery request, CancellationToken ct = default)
    {
        if (request.Count < 0 || request.Count > 50)
            return ResultOfT<IReadOnlyList<GetAllProductsResponse>>.BadRequest(ProductErrors.InvalidCount);

        var products = await repository.ListAsync(new GetAllProductsSpecification(request.Count), ct);

        return ResultOfT<IReadOnlyList<GetAllProductsResponse>>.Ok(products);
    }
}
