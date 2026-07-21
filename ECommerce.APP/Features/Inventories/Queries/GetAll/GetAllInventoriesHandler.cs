using ECommerce.APP.Mediator;
using ECommerce.Domain.Abstractions.Repositories;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Entities.Errors;
using ECommerce.Domain.Results;

namespace ECommerce.APP.Features.Inventories.Queries.GetAll;

public sealed class GetAllInventoriesHandler(IReadRepository<Inventory> repository) : 
    IRequestHandler<GetAllInventoriesQuery, ResultOfT<IReadOnlyList<GetAllInventoriesResponse>>>
{
    public async Task<ResultOfT<IReadOnlyList<GetAllInventoriesResponse>>> Handle(
        GetAllInventoriesQuery request, CancellationToken ct = default)
    {
        if (request.Count < 0 || request.Count > 50)
            return ResultOfT<IReadOnlyList<GetAllInventoriesResponse>>.BadRequest(InventoryErrors.InvalidCount);

        var inventories = await repository.ListAsync(new GetAllInventoriesSpecification(request.Count), ct);

        return ResultOfT<IReadOnlyList<GetAllInventoriesResponse>>.Ok(inventories);
    }
}
