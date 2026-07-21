using ECommerce.APP.Mediator;
using ECommerce.Domain.Abstractions.Repositories;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Entities.Errors;
using ECommerce.Domain.Results;

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
