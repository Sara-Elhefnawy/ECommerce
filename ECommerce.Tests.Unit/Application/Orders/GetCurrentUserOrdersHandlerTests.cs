using ECommerce.APP.Features.Orders.Queries.GetUserOrders;
using ECommerce.APP.Identity;
using ECommerce.Domain.Abstractions.Repositories;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Entities.Errors;
using ECommerce.Domain.Specifications;
using FluentAssertions;
using Moq;
using Xunit;

namespace ECommerce.Tests.Unit.Application.Orders;

public class GetCurrentUserOrdersHandlerTests
{
    private readonly Mock<ICurrentUserService> _currentUser = new();
    private readonly Mock<IReadRepository<Order>> _orderRepository = new();
    private readonly GetCurrentUserOrdersHandler _sut;

    public GetCurrentUserOrdersHandlerTests()
    {
        _sut = new GetCurrentUserOrdersHandler(_currentUser.Object, _orderRepository.Object);
    }

    private static GetCurrentUserOrdersQuery Query(int pageNumber = 1, int pageSize = 10, string? searchTerm = null) =>
        new(PageNumber: pageNumber, PageSize: pageSize, SearchTerm: searchTerm, SortBy: null, IsSortDescending: false);

    [Fact]
    public async Task Handle_WhenUserNotAuthenticated_ReturnsUnauthorized()
    {
        _currentUser.Setup(c => c.UserId).Returns((Guid?)null);

        var result = await _sut.Handle(Query(), CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(OrderErrors.Unauthorized);
    }

    [Theory]
    [InlineData(0, 5)]  // page 0 -> normalized to 1
    [InlineData(-5, 5)] // negative -> normalized to 1
    public async Task Handle_WithInvalidPageNumber_NormalizesToPageOne(int requestedPage, int expectedCount)
    {
        var userId = Guid.NewGuid();
        _currentUser.Setup(c => c.UserId).Returns(userId);

        _orderRepository
            .Setup(r => r.CountAsync(It.IsAny<ISpecification<Order>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedCount);
        _orderRepository
            .Setup(r => r.ListAsync(It.IsAny<ISpecification<Order>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        var result = await _sut.Handle(Query(pageNumber: requestedPage), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.PageNumber.Should().Be(1); // adjust property name if PagedResult names it differently
    }

    [Theory]
    [InlineData(0)]
    [InlineData(51)]
    public async Task Handle_WithInvalidPageSize_NormalizesToTen(int requestedSize)
    {
        var userId = Guid.NewGuid();
        _currentUser.Setup(c => c.UserId).Returns(userId);

        _orderRepository
            .Setup(r => r.CountAsync(It.IsAny<ISpecification<Order>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(0);
        _orderRepository
            .Setup(r => r.ListAsync(It.IsAny<ISpecification<Order>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        var result = await _sut.Handle(Query(pageSize: requestedSize), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.PageSize.Should().Be(10); // adjust property name if PagedResult names it differently
    }

    [Fact]
    public async Task Handle_WithValidPaging_ReturnsMappedOrders()
    {
        var userId = Guid.NewGuid();
        _currentUser.Setup(c => c.UserId).Returns(userId);

        var deliveryMethod = DeliveryMethod.Create("Standard", 5m, "3-5 days").Value!;
        var address = UserAddress.Create(
            userId, "Jane", "Doe", "0100000000", "Egypt", "Cairo", "1 Main St", "12345").Value!;
        var order = Order.Create(userId, deliveryMethod, address,
            [(Guid.NewGuid(), "Product", "url", 10m, 1)]).Value!;

        _orderRepository
            .Setup(r => r.CountAsync(It.IsAny<ISpecification<Order>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);
        _orderRepository
            .Setup(r => r.ListAsync(It.IsAny<ISpecification<Order>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([order]);

        var result = await _sut.Handle(Query(), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Items.Should().ContainSingle(o => o.Id == order.Id); // adjust "Items" if PagedResult names it differently
    }
}
