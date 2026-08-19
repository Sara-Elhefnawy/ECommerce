using ECommerce.APP.Features.Inventories.Queries.GetByProductId;
using ECommerce.APP.Features.Orders.DTOs;
using ECommerce.APP.Features.Orders.Mapper;
using ECommerce.APP.Features.Orders.Specifications;
using ECommerce.APP.Identity;
using ECommerce.APP.Mediator;
using ECommerce.Domain.Abstractions.Repositories;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Entities.Errors;
using ECommerce.Domain.Results;

namespace ECommerce.APP.Features.Orders.Commands.Cancel;

public sealed class CancelOrderHandler(
    ICurrentUserService currentUser,
    IRepository<Order> orderRepository,
    IRepository<Inventory> inventoryRepository,
    IUnitOfWork uow)
    : IRequestHandler<CancelOrderCommand, ResultOfT<OrderResponse>>
{
    public async Task<ResultOfT<OrderResponse>> Handle(
        CancelOrderCommand request,
        CancellationToken ct = default)
    {
        if (currentUser.UserId is null)
            return OrderErrors.Unauthorized;

        var order = await orderRepository.FirstOrDefaultAsync(
            new GetOrderByIdSpecification(request.OrderId, currentUser.UserId.Value, tracking: true),
            ct);

        if (order is null)
            return OrderErrors.NotFound;

        var cancelResult = order.Cancel();
        if (cancelResult.IsFailure)
            return cancelResult.Error!;

        foreach (var item in order.Items)
        {
            var inventory = await inventoryRepository.FirstOrDefaultAsync(
                new GetInventoryByProductIdEntitySpecification(item.ItemOrdered.ProductId, tracking: true),
                ct);

            if (inventory is null)
                continue; // product's inventory row was removed/never existed — nothing to restore

            inventory.AddStock(item.Quantity);
            inventoryRepository.Update(inventory);
        }

        orderRepository.Update(order);
        await uow.SaveChangesAsync(ct);

        return OrderMapper.ToResponse(order);
    }
}
