using ECommerce.APP.Mediator;
using ECommerce.Domain.Results;

namespace ECommerce.APP.Features.Inventories.Commands.Restock;

public sealed record RestockInventoryCommand(Guid ProductId, int Quantity)
    : IRequest<ResultOfT<RestockInventoryResponse>>;
