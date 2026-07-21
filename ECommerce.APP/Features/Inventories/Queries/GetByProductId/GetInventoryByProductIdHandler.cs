using ECommerce.APP.Mediator;
using ECommerce.Domain.Abstractions.Repositories;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Entities.Errors;
using ECommerce.Domain.Results;

namespace ECommerce.APP.Features.Inventories.Queries.GetByProductId;

public sealed class GetInventoryByProductIdHandler(IReadRepository<Inventory> repository) : 
    IRequestHandler<GetInventoryByProductIdQuery, ResultOfT<GetInventoryByProductIdResponse>>
{
    public async Task<ResultOfT<GetInventoryByProductIdResponse>> Handle(
        GetInventoryByProductIdQuery request, 
        CancellationToken ct = default)
    {
        var inventory = await repository.FirstOrDefaultAsync(new GetInventoryByProductIdSpecification(request.ProductId), ct);

        return inventory is null
            ? InventoryErrors.NotFound
            : inventory;
    }
}
