using ECommerce.APP.Features.Inventories.Queries.GetAll;
using ECommerce.APP.Features.Inventories.Queries.GetByProductId;
using ECommerce.Domain.Abstractions.Repositories;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Entities.Errors;
using FluentAssertions;
using Moq;
using Xunit;

namespace ECommerce.Tests.Unit.Application.Inventories;

public class GetAllInventoriesHandlerTests
{
    private readonly Mock<IReadRepository<Inventory>> _repository = new();

    private readonly GetAllInventoriesHandler _sut;

    public GetAllInventoriesHandlerTests()
    {
        _sut = new GetAllInventoriesHandler(_repository.Object);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(51)]
    public async Task Handle_WithCountOutOfRange_ReturnsInvalidCount(int count)
    {
        var result = await _sut.Handle(
            new GetAllInventoriesQuery(Count: count),
            CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(InventoryErrors.InvalidCount);

        _repository.Verify(
            r => r.ListAsync(
                It.IsAny<GetAllInventoriesSpecification>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_WithValidCount_ReturnsInventories()
    {
        var inventory = new GetAllInventoriesResponse(
            ProductId: Guid.NewGuid(),
            ProductName: "Product",
            QuantityOnHand: 10,
            InStock: true);

        _repository
            .Setup(r => r.ListAsync(
                It.IsAny<GetAllInventoriesSpecification>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync([inventory]);

        var result = await _sut.Handle(
            new GetAllInventoriesQuery(Count: 20),
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Should().ContainSingle();

        result.Value![0].ProductId.Should().Be(inventory.ProductId);
        result.Value[0].ProductName.Should().Be("Product");
        result.Value[0].QuantityOnHand.Should().Be(10);
        result.Value[0].InStock.Should().BeTrue();
    }
}
