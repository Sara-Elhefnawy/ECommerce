using ECommerce.APP.Features.Orders.Commands.Cancel;
using ECommerce.APP.Identity;
using ECommerce.Domain.Abstractions.Repositories;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Entities.Enums;
using ECommerce.Domain.Entities.Errors;
using ECommerce.Domain.Specifications;
using FluentAssertions;
using Moq;
using Xunit;

namespace ECommerce.Tests.Unit.Application.Orders;

public class CancelOrderHandlerTests
{
    private readonly Mock<ICurrentUserService> _currentUser = new();
    private readonly Mock<IRepository<Order>> _orderRepository = new();
    private readonly Mock<IRepository<Inventory>> _inventoryRepository = new();
    private readonly Mock<IUnitOfWork> _uow = new();
    private readonly CancelOrderHandler _sut;

    private readonly Guid _userId = Guid.NewGuid();

    public CancelOrderHandlerTests()
    {
        _sut = new CancelOrderHandler(
            _currentUser.Object, _orderRepository.Object, _inventoryRepository.Object, _uow.Object);
        _currentUser.Setup(c => c.UserId).Returns(_userId);
    }

    private Order ValidPendingOrder(Guid productId, int quantity = 2)
    {
        var deliveryMethod = DeliveryMethod.Create("Standard", 5m, "3-5 days").Value!;
        var address = UserAddress.Create(
            _userId, "Jane", "Doe", "0100000000", "Egypt", "Cairo", "1 Main St", "12345").Value!;

        return Order.Create(_userId, deliveryMethod, address,
            [(productId, "Product", "url", 10m, quantity)]).Value!;
    }

    [Fact]
    public async Task Handle_WhenUserNotAuthenticated_ReturnsUnauthorized()
    {
        _currentUser.Setup(c => c.UserId).Returns((Guid?)null);

        var result = await _sut.Handle(new CancelOrderCommand(OrderId: Guid.NewGuid()), CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(OrderErrors.Unauthorized);
    }

    [Fact]
    public async Task Handle_WhenOrderNotFound_ReturnsNotFound()
    {
        _orderRepository
            .Setup(r => r.FirstOrDefaultAsync(It.IsAny<ISpecification<Order>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Order?)null);

        var result = await _sut.Handle(new CancelOrderCommand(OrderId: Guid.NewGuid()), CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(OrderErrors.NotFound);
    }

    [Fact]
    public async Task Handle_WhenOrderCannotBeCancelled_ReturnsFailureAndDoesNotRestock()
    {
        var productId = Guid.NewGuid();
        var order = ValidPendingOrder(productId);
        order.MarkAsPaid("pi_123"); // now Processing — no longer cancellable

        _orderRepository
            .Setup(r => r.FirstOrDefaultAsync(It.IsAny<ISpecification<Order>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(order);

        var result = await _sut.Handle(new CancelOrderCommand(OrderId: order.Id), CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(OrderErrors.CannotCancel);
        _inventoryRepository.Verify(r => r.Update(It.IsAny<Inventory>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WithValidOrder_CancelsAndRestocksInventoryForEveryItem()
    {
        // This is the exact fix verified manually earlier in the
        // conversation: cancelling a PaymentFailed/Pending order must
        // restore stock for every line item.
        var productId = Guid.NewGuid();
        var order = ValidPendingOrder(productId, quantity: 3);

        _orderRepository
            .Setup(r => r.FirstOrDefaultAsync(It.IsAny<ISpecification<Order>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(order);

        var inventory = Inventory.Create(productId, quantity: 5).Value!;
        _inventoryRepository
            .Setup(r => r.FirstOrDefaultAsync(It.IsAny<ISpecification<Inventory>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(inventory);

        var result = await _sut.Handle(new CancelOrderCommand(OrderId: order.Id), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        order.Status.Should().Be(OrderStatus.Cancelled);
        inventory.QuantityOnHand.Should().Be(8); // 5 + 3 restocked
        _inventoryRepository.Verify(r => r.Update(inventory), Times.Once);
        _uow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenInventoryRowNoLongerExists_SkipsRestockForThatItemWithoutFailing()
    {
        var productId = Guid.NewGuid();
        var order = ValidPendingOrder(productId);

        _orderRepository
            .Setup(r => r.FirstOrDefaultAsync(It.IsAny<ISpecification<Order>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(order);

        _inventoryRepository
            .Setup(r => r.FirstOrDefaultAsync(It.IsAny<ISpecification<Inventory>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Inventory?)null); // matches the handler's `continue` comment

        var result = await _sut.Handle(new CancelOrderCommand(OrderId: order.Id), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        order.Status.Should().Be(OrderStatus.Cancelled);
    }
}
