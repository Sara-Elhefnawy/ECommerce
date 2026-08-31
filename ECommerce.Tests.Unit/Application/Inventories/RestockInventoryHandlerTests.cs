using ECommerce.APP.Features.Inventories.Commands.Restock;
using ECommerce.Domain.Abstractions.Repositories;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Entities.Errors;
using ECommerce.Domain.Specifications;
using FluentAssertions;
using Moq;
using Xunit;

namespace ECommerce.Tests.Unit.Application.Inventories;

public class RestockInventoryHandlerTests
{
    private readonly Mock<IRepository<Inventory>> _repository = new();
    private readonly Mock<IUnitOfWork> _uow = new();
    private readonly RestockInventoryHandler _sut;

    public RestockInventoryHandlerTests()
    {
        _sut = new RestockInventoryHandler(_repository.Object, _uow.Object);
    }

    [Fact]
    public async Task Handle_WhenInventoryNotFound_ReturnsNotFound()
    {
        _repository
            .Setup(r => r.FirstOrDefaultAsync(It.IsAny<ISpecification<Inventory>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Inventory?)null);

        var result = await _sut.Handle(
            new RestockInventoryCommand(ProductId: Guid.NewGuid(), Quantity: 5), CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(InventoryErrors.NotFound);
    }

    [Fact]
    public async Task Handle_WithNonPositiveQuantity_ReturnsFailureWithoutSaving()
    {
        var inventory = Inventory.Create(Guid.NewGuid(), 10).Value!;
        _repository
            .Setup(r => r.FirstOrDefaultAsync(It.IsAny<ISpecification<Inventory>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(inventory);

        var result = await _sut.Handle(
            new RestockInventoryCommand(ProductId: inventory.ProductId, Quantity: 0), CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(InventoryErrors.InvalidQuantity);
        _uow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WithValidQuantity_AddsStockAndSaves()
    {
        var inventory = Inventory.Create(Guid.NewGuid(), 10).Value!;
        _repository
            .Setup(r => r.FirstOrDefaultAsync(It.IsAny<ISpecification<Inventory>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(inventory);

        var result = await _sut.Handle(
            new RestockInventoryCommand(ProductId: inventory.ProductId, Quantity: 15), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        inventory.QuantityOnHand.Should().Be(25);
        _repository.Verify(r => r.Update(inventory), Times.Once);
        _uow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
