using ECommerce.APP.Abstractions.Mediator;
using ECommerce.APP.Features.Products.Queries.GetById;
using ECommerce.Domain.Abstractions.Repositories;
using ECommerce.Domain.Common;
using ECommerce.Domain.Entities;

namespace ECommerce.APP.Features.Products.Queries.GetById;

public sealed class GetProductByIdHandler(IReadRepository<Product> repository) :
    
    IRequestHandler<GetProductByIdQuery, ResultOfT<GetProductByIdResponse>>
{
    public async Task<ResultOfT<GetProductByIdResponse>> Handle(
        GetProductByIdQuery request, CancellationToken ct = default)
    {
        var product = await repository.FirstOrDefaultAsync(new GetProductByIdSpecification(request.Id), ct);

        return product is null
            ? ProductErrors.NotFound
            : product;
    }
}
