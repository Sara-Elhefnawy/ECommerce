using ECommerce.APP.Cachings.Carts;
using ECommerce.APP.Features.DeliveryMethods.Queries.GetById;
using ECommerce.APP.Features.Inventories.Queries.GetByProductId;
using ECommerce.APP.Features.Orders.DTOs;
using ECommerce.APP.Features.Orders.Mapper;
using ECommerce.APP.Features.Users.Queries.GetUserAddresses;
using ECommerce.APP.Identity;
using ECommerce.APP.Mediator;
using ECommerce.Domain.Abstractions.Repositories;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Entities.Errors;
using ECommerce.Domain.Results;

namespace ECommerce.APP.Features.Orders.Commands.Create;

public sealed class CreateOrderHandler(
    ICurrentUserService userService,
    ICartRepository cartRepo,
    IReadRepository<UserAddress> addressRepository,
    IRepository<DeliveryMethod> deliveryMethodRepository,
    IRepository<Inventory> inventoryRepository,
    IRepository<Order> repository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<CreateOrderCommand, ResultOfT<OrderResponse>>
{
    public async Task<ResultOfT<OrderResponse>> Handle(
        CreateOrderCommand request,
        CancellationToken ct = default)
    {
        if (userService.UserId is null)
            return OrderErrors.Unauthorized;

        var address = await addressRepository.FirstOrDefaultAsync(
            new GetUserAddressesEntitySpecification(request.ShippingAddressId, userService.UserId.Value),
            ct);

        if (address is null)
            return OrderErrors.ShippingAddressNotFound;

        var deliveryMethod = await deliveryMethodRepository.FirstOrDefaultAsync(
            new GetDeliveryMethodEntityByIdSpecification(request.DeliveryMethodId), 
            ct);

        if (deliveryMethod is null)
            return DeliveryMethodErrors.NotFound;

        var cart = await cartRepo.GetAsync(userService.UserId.Value, ct);

        if (cart is null || cart.Value!.Items.Count == 0)
            return OrderErrors.EmptyBasket;

        foreach (var item in cart.Value.Items)
        {
            var inventory = await inventoryRepository.FirstOrDefaultAsync(
                new GetInventoryByProductIdEntitySpecification(item.ProductId, tracking: true), ct);

            if (inventory is null)
                return InventoryErrors.NotFound;

            var removeResult = inventory.RemoveStock(item.Quantity);

            if (removeResult.IsFailure)
                return removeResult.Error!;

            inventoryRepository.Update(inventory);
        }

        var items = cart.Value.Items
            .Select(i => (i.ProductId, i.ProductName, i.PictureUrl, i.UnitPrice, i.Quantity))
            .ToList();


        var createResult = Order.Create(
            userService.UserId.Value,
            deliveryMethod,
            address,
            items);

        if (createResult.IsFailure)
            return createResult.Error!;

        repository.Add(createResult.Value);
        await unitOfWork.SaveChangesAsync(ct);

        cart.Value.Clear();
        await cartRepo.SaveAsync(cart.Value, ct);

        return ResultOfT<OrderResponse>.Created(OrderMapper.ToResponse(createResult.Value));
    }
}
