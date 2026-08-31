using ECommerce.APP.Features.Orders.Queries.GetById;
using ECommerce.APP.Identity;
using ECommerce.Domain.Abstractions.Repositories;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Entities.Errors;
using ECommerce.Domain.Specifications;
using FluentAssertions;
using Moq;
using Xunit;

namespace ECommerce.Tests.Unit.Application.Orders;

public class GetOrderByIdHandlerTests
{
    private readonly Mock<ICurrentUserService> _currentUser = new();
    private readonly Mock<IReadRepository<Order>> _orderRepository = new();
    private readonly GetOrderByIdHandler _sut;

    public GetOrderByIdHandlerTests()
    {
        _sut = new GetOrderByIdHandler(_currentUser.Object, _orderRepository.Object);
    }

    [Fact]
    public async Task Handle_WhenUserNotAuthenticated_ReturnsUnauthorized()
    {
        _currentUser.Setup(c => c.UserId).Returns((Guid?)null);

        var result = await _sut.Handle(new GetOrderByIdQuery(OrderId: Guid.NewGuid()), CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(OrderErrors.Unauthorized);
    }

    [Fact]
    public async Task Handle_WhenOrderNotFound_ReturnsNotFound()
    {
        _currentUser.Setup(c => c.UserId).Returns(Guid.NewGuid());

        _orderRepository
            .Setup(r => r.FirstOrDefaultAsync(It.IsAny<ISpecification<Order>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Order?)null);

        var result = await _sut.Handle(new GetOrderByIdQuery(OrderId: Guid.NewGuid()), CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(OrderErrors.NotFound);
    }

    [Fact]
    public async Task Handle_WhenOrderExists_ReturnsMappedResponse()
    {
        var userId = Guid.NewGuid();
        _currentUser.Setup(c => c.UserId).Returns(userId);

        var deliveryMethod = DeliveryMethod.Create("Standard", 5m, "3-5 days").Value!;
        var address = UserAddress.Create(
            userId, "Jane", "Doe", "0100000000", "Egypt", "Cairo", "1 Main St", "12345").Value!;
        var order = Order.Create(userId, deliveryMethod, address,
            [(Guid.NewGuid(), "Product", "url", 10m, 1)]).Value!;

        _orderRepository
            .Setup(r => r.FirstOrDefaultAsync(It.IsAny<ISpecification<Order>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(order);

        var result = await _sut.Handle(new GetOrderByIdQuery(OrderId: order.Id), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Id.Should().Be(order.Id);
    }
}
