using ECommerce.APP.Features.Inventories.Queries.GetByProductId;
using ECommerce.Domain.Abstractions.Repositories;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Entities.Errors;
using FluentAssertions;
using Moq;
using Xunit;

namespace ECommerce.Tests.Unit.Application.Inventories;

public class GetInventoryByProductIdHandlerTests
{
    private readonly Mock<IReadRepository<Inventory>> _repository = new();

    private readonly GetInventoryByProductIdHandler _sut;

    public GetInventoryByProductIdHandlerTests()
    {
        _sut = new GetInventoryByProductIdHandler(_repository.Object);
    }

    [Fact]
    public async Task Handle_WhenInventoryNotFound_ReturnsNotFound()
    {
        _repository
            .Setup(r => r.FirstOrDefaultAsync(
                It.IsAny<GetInventoryByProductIdSpecification>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((GetInventoryByProductIdResponse?)null);

        var result = await _sut.Handle(
            new GetInventoryByProductIdQuery(
                ProductId: Guid.NewGuid()),
            CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(InventoryErrors.NotFound);
    }

    [Fact]
    public async Task Handle_WhenInventoryExists_ReturnsIt()
    {
        var productId = Guid.NewGuid();

        var inventory = new GetInventoryByProductIdResponse(
            ProductId: productId,
            ProductName: "Product",
            QuantityOnHand: 10,
            InStock: true);

        _repository
            .Setup(r => r.FirstOrDefaultAsync(
                It.IsAny<GetInventoryByProductIdSpecification>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(inventory);

        var result = await _sut.Handle(
            new GetInventoryByProductIdQuery(ProductId: productId),
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();

        result.Value!.ProductId.Should().Be(productId);
        result.Value.ProductName.Should().Be("Product");
        result.Value.QuantityOnHand.Should().Be(10);
        result.Value.InStock.Should().BeTrue();
    }
}
