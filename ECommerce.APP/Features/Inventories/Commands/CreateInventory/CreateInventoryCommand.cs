using ECommerce.APP.Mediator;
using ECommerce.Domain.Results;

namespace ECommerce.APP.Features.Inventories.Commands.CreateInventory;

public sealed record CreateInventoryCommand(Guid ProductId, int Quantity)
    : IRequest<ResultOfT<CreateInventoryResponse>>;
