using ECommerce.APP.Cachings.Carts;
using ECommerce.APP.Features.Orders.Commands.Create;
using ECommerce.APP.Identity;
using ECommerce.Domain.Abstractions.Repositories;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Entities.Errors;
using ECommerce.Domain.Results;
using ECommerce.Domain.Specifications;
using FluentAssertions;
using Moq;
using Xunit;

namespace ECommerce.Tests.Unit.Application.Orders;

public class CreateOrderHandlerTests
{
    private readonly Mock<ICurrentUserService> _currentUser = new();
    private readonly Mock<ICartRepository> _cartRepo = new();
    private readonly Mock<IReadRepository<UserAddress>> _addressRepository = new();
    private readonly Mock<IRepository<DeliveryMethod>> _deliveryMethodRepository = new();
    private readonly Mock<IRepository<Inventory>> _inventoryRepository = new();
    private readonly Mock<IRepository<Order>> _repository = new();
    private readonly Mock<IUnitOfWork> _uow = new();
    private readonly CreateOrderHandler _sut;

    private readonly Guid _userId = Guid.NewGuid();
    private readonly Guid _addressId = Guid.NewGuid();
    private readonly Guid _deliveryMethodId = Guid.NewGuid();
    private readonly Guid _productId = Guid.NewGuid();

    public CreateOrderHandlerTests()
    {
        _sut = new CreateOrderHandler(
            _currentUser.Object, _cartRepo.Object, _addressRepository.Object,
            _deliveryMethodRepository.Object, _inventoryRepository.Object,
            _repository.Object, _uow.Object);

        _currentUser.Setup(c => c.UserId).Returns(_userId);
    }

    private static CreateOrderCommand Command() => new(
        ShippingAddressId: Guid.NewGuid(), // overwritten per-test via _addressId where needed
        DeliveryMethodId: Guid.NewGuid());

    private UserAddress ValidAddress() => UserAddress.Create(
        _userId, "Jane", "Doe", "0100000000", "Egypt", "Cairo", "1 Main St", "12345").Value!;

    private DeliveryMethod ValidDeliveryMethod() =>
        DeliveryMethod.Create("Standard", 5m, "3-5 days").Value!;

    private Inventory ValidInventory(int quantity = 10) =>
        Inventory.Create(_productId, quantity).Value!;

    private ResultOfT<Cart?> CartWithOneItem(int quantity = 2)
    {
        var cart = Cart.CreateEmpty(_userId).Value!;
        cart.AddItem(_productId, "Product", "url", 10m, quantity);
        return ResultOfT<Cart?>.Ok(cart);
    }

    private void SetupHappyPathDependencies()
    {
        _addressRepository
            .Setup(r => r.FirstOrDefaultAsync(It.IsAny<ISpecification<UserAddress>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(ValidAddress());

        _deliveryMethodRepository
            .Setup(r => r.FirstOrDefaultAsync(It.IsAny<ISpecification<DeliveryMethod>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(ValidDeliveryMethod());

        _cartRepo
            .Setup(r => r.GetAsync(_userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(CartWithOneItem());

        _inventoryRepository
            .Setup(r => r.FirstOrDefaultAsync(It.IsAny<ISpecification<Inventory>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(ValidInventory());
    }

    [Fact]
    public async Task Handle_WhenUserNotAuthenticated_ReturnsUnauthorized()
    {
        _currentUser.Setup(c => c.UserId).Returns((Guid?)null);

        var result = await _sut.Handle(Command(), CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(OrderErrors.Unauthorized);
    }

    [Fact]
    public async Task Handle_WhenShippingAddressNotFound_ReturnsShippingAddressNotFound()
    {
        _addressRepository
            .Setup(r => r.FirstOrDefaultAsync(It.IsAny<ISpecification<UserAddress>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((UserAddress?)null);

        var result = await _sut.Handle(Command(), CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(OrderErrors.ShippingAddressNotFound);
    }

    [Fact]
    public async Task Handle_WhenDeliveryMethodNotFound_ReturnsNotFound()
    {
        _addressRepository
            .Setup(r => r.FirstOrDefaultAsync(It.IsAny<ISpecification<UserAddress>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(ValidAddress());

        _deliveryMethodRepository
            .Setup(r => r.FirstOrDefaultAsync(It.IsAny<ISpecification<DeliveryMethod>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((DeliveryMethod?)null);

        var result = await _sut.Handle(Command(), CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DeliveryMethodErrors.NotFound);
    }

    [Fact]
    public async Task Handle_WhenCartIsEmpty_ReturnsEmptyBasket()
    {
        _addressRepository
            .Setup(r => r.FirstOrDefaultAsync(It.IsAny<ISpecification<UserAddress>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(ValidAddress());

        _deliveryMethodRepository
            .Setup(r => r.FirstOrDefaultAsync(It.IsAny<ISpecification<DeliveryMethod>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(ValidDeliveryMethod());

        _cartRepo
            .Setup(r => r.GetAsync(_userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(ResultOfT<Cart?>.Ok(Cart.CreateEmpty(_userId).Value));

        var result = await _sut.Handle(Command(), CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(OrderErrors.EmptyBasket);
    }

    [Fact]
    public async Task Handle_WhenInventoryNotFoundForCartItem_ReturnsInventoryNotFound()
    {
        SetupHappyPathDependencies();
        _inventoryRepository
            .Setup(r => r.FirstOrDefaultAsync(It.IsAny<ISpecification<Inventory>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Inventory?)null);

        var result = await _sut.Handle(Command(), CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(InventoryErrors.NotFound);
        _repository.Verify(r => r.Add(It.IsAny<Order>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenInsufficientStock_ReturnsFailureAndDoesNotCreateOrder()
    {
        SetupHappyPathDependencies();
        _cartRepo
            .Setup(r => r.GetAsync(_userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(CartWithOneItem(quantity: 100)); // more than the 10 in stock

        var result = await _sut.Handle(Command(), CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(InventoryErrors.NotEnoughStock);
        _repository.Verify(r => r.Add(It.IsAny<Order>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WithValidCart_RemovesStockCreatesOrderAndClearsCart()
    {
        SetupHappyPathDependencies();

        var result = await _sut.Handle(Command(), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        _inventoryRepository.Verify(r => r.Update(It.IsAny<Inventory>()), Times.Once);
        _repository.Verify(r => r.Add(It.IsAny<Order>()), Times.Once);
        _uow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        _cartRepo.Verify(r => r.SaveAsync(It.Is<Cart>(c => c.Items.Count == 0), It.IsAny<CancellationToken>()), Times.Once);
    }
}
