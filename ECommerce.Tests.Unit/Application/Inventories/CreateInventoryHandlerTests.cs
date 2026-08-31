using ECommerce.APP.Features.Inventories.Commands.CreateInventory;
using ECommerce.APP.Features.Products.Queries.GetById;
using ECommerce.Domain.Abstractions.Repositories;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Entities.Errors;
using ECommerce.Domain.Specifications;
using FluentAssertions;
using Moq;
using Xunit;

namespace ECommerce.Tests.Unit.Application.Inventories;

public class CreateInventoryHandlerTests
{
    private readonly Mock<IRepository<Inventory>> _repository = new();
    private readonly Mock<IReadRepository<Product>> _productRepository = new();
    private readonly Mock<IUnitOfWork> _uow = new();

    private readonly CreateInventoryHandler _sut;

    public CreateInventoryHandlerTests()
    {
        _sut = new CreateInventoryHandler(
            _repository.Object,
            _productRepository.Object,
            _uow.Object);
    }

    private static GetProductByIdResponse ValidProduct() =>
        new(
            Guid.NewGuid(),
            "Product",
            "Description",
            "url",
            10m,
            "Type",
            "Brand");

    [Fact]
    public async Task Handle_WhenProductNotFound_ReturnsProductNotFound()
    {
        _productRepository
            .Setup(r => r.FirstOrDefaultAsync(
                It.IsAny<GetProductByIdSpecification>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((GetProductByIdResponse?)null);

        var result = await _sut.Handle(
            new CreateInventoryCommand(
                ProductId: Guid.NewGuid(),
                Quantity: 10),
            CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ProductErrors.NotFound);

        _repository.Verify(
            r => r.AnyAsync(
                It.IsAny<ISpecification<Inventory>>(),
                It.IsAny<CancellationToken>()),
            Times.Never);

        _repository.Verify(
            r => r.Add(It.IsAny<Inventory>()),
            Times.Never);

        _uow.Verify(
            u => u.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_WhenInventoryAlreadyExistsForProduct_ReturnsAlreadyExists()
    {
        var product = ValidProduct();

        _productRepository
            .Setup(r => r.FirstOrDefaultAsync(
                It.IsAny<GetProductByIdSpecification>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);

        _repository
            .Setup(r => r.AnyAsync(
                It.IsAny<ISpecification<Inventory>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await _sut.Handle(
            new CreateInventoryCommand(
                ProductId: product.Id,
                Quantity: 10),
            CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(InventoryErrors.AlreadyExists);

        _repository.Verify(
            r => r.Add(It.IsAny<Inventory>()),
            Times.Never);

        _uow.Verify(
            u => u.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_WithInvalidQuantity_ReturnsFailureWithoutSaving()
    {
        var product = ValidProduct();

        _productRepository
            .Setup(r => r.FirstOrDefaultAsync(
                It.IsAny<GetProductByIdSpecification>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);

        _repository
            .Setup(r => r.AnyAsync(
                It.IsAny<ISpecification<Inventory>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var result = await _sut.Handle(
            new CreateInventoryCommand(
                ProductId: product.Id,
                Quantity: 0),
            CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(InventoryErrors.InvalidQuantity);

        _repository.Verify(
            r => r.Add(It.IsAny<Inventory>()),
            Times.Never);

        _uow.Verify(
            u => u.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_WithValidData_CreatesInventoryAndSaves()
    {
        var product = ValidProduct();

        _productRepository
            .Setup(r => r.FirstOrDefaultAsync(
                It.IsAny<GetProductByIdSpecification>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);

        _repository
            .Setup(r => r.AnyAsync(
                It.IsAny<ISpecification<Inventory>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var result = await _sut.Handle(
            new CreateInventoryCommand(
                ProductId: product.Id,
                Quantity: 25),
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();

        result.Value.Should().NotBeNull();
        result.Value!.ProductId.Should().Be(product.Id);
        result.Value.QuantityOnHand.Should().Be(25);

        _repository.Verify(
            r => r.Add(It.Is<Inventory>(
                i => i.ProductId == product.Id &&
                     i.QuantityOnHand == 25)),
            Times.Once);

        _uow.Verify(
            u => u.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
