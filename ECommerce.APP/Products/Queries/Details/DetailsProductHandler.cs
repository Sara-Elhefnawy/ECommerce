using ECommerce.APP.Abstractions.Mediator;
using ECommerce.Domain.Abstractions.Repositories;
using ECommerce.Domain.Common;
using ECommerce.Domain.Entities;

namespace ECommerce.APP.Products.Queries.Details;

public sealed class DetailsProductHandler(IReadRepository<Product> repository) :
    
    IRequestHandler<DetailsProductQuery, ResultOfT<DetailsProductResponse>>
{
    public async Task<ResultOfT<DetailsProductResponse>> Handle(
        DetailsProductQuery request, CancellationToken ct = default)
    {
        var product = await repository.FirstOrDefaultAsync(new DetailsProductSpecification(request.Id), ct);

        return product is null
            ? ProductErrors.NotFound
            : product;
    }
}
