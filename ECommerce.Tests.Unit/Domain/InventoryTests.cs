using ECommerce.Domain.Entities;
using ECommerce.Domain.Entities.Errors;
using FluentAssertions;
using Xunit;

namespace ECommerce.Tests.Unit.Domain;

public class InventoryTests
{
    // ---------- Create ----------
    [Fact]
    public void Create_WithValidData_ReturnsCreatedInventory()
    {
        var result = Inventory.Create(Guid.NewGuid(), quantity: 10);

        result.IsSuccess.Should().BeTrue();
        result.Value!.QuantityOnHand.Should().Be(10);
    }

    [Fact]
    public void Create_WithEmptyProductId_ReturnsInvalidProductIdError()
    {
        var result = Inventory.Create(Guid.Empty, quantity: 10);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(InventoryErrors.InvalidProduct);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-5)]
    public void Create_WithNonPositiveQuantity_ReturnsInvalidQuantityError(int quantity)
    {
        var result = Inventory.Create(Guid.NewGuid(), quantity);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(InventoryErrors.InvalidQuantity);
    }

    // ---------- HasEnough ----------
    [Fact]
    public void HasEnough_WhenStockCoversRequestedQuantity_ReturnsTrue()
    {
        var inventory = Inventory.Create(Guid.NewGuid(), quantity: 10).Value!;

        inventory.HasEnough(5).Should().BeTrue();
        inventory.HasEnough(10).Should().BeTrue();
    }

    [Fact]
    public void HasEnough_WhenStockIsBelowRequestedQuantity_ReturnsFalse()
    {
        var inventory = Inventory.Create(Guid.NewGuid(), quantity: 3).Value!;

        inventory.HasEnough(4).Should().BeFalse();
    }

    // ---------- AddStock ----------
    [Fact]
    public void AddStock_WithPositiveQuantity_IncreasesQuantityOnHand()
    {
        var inventory = Inventory.Create(Guid.NewGuid(), quantity: 10).Value!;

        var result = inventory.AddStock(5);

        result.IsSuccess.Should().BeTrue();
        inventory.QuantityOnHand.Should().Be(15);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void AddStock_WithNonPositiveQuantity_ReturnsFailureAndDoesNotChangeStock(int quantity)
    {
        var inventory = Inventory.Create(Guid.NewGuid(), quantity: 10).Value!;

        var result = inventory.AddStock(quantity);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(InventoryErrors.InvalidQuantity);
        inventory.QuantityOnHand.Should().Be(10);
    }

    // ---------- RemoveStock ----------
    [Fact]
    public void RemoveStock_WithSufficientStock_DecreasesQuantityOnHand()
    {
        var inventory = Inventory.Create(Guid.NewGuid(), quantity: 10).Value!;

        var result = inventory.RemoveStock(4);

        result.IsSuccess.Should().BeTrue();
        inventory.QuantityOnHand.Should().Be(6);
    }

    [Fact]
    public void RemoveStock_WithInsufficientStock_ReturnsFailureAndDoesNotChangeStock()
    {
        var inventory = Inventory.Create(Guid.NewGuid(), quantity: 3).Value!;

        var result = inventory.RemoveStock(5);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(InventoryErrors.NotEnoughStock);
        inventory.QuantityOnHand.Should().Be(3);
    }

    [Fact]
    public void RemoveStock_ExactlyAllRemainingStock_SucceedsAndLeavesZero()
    {
        var inventory = Inventory.Create(Guid.NewGuid(), quantity: 5).Value!;

        var result = inventory.RemoveStock(5);

        result.IsSuccess.Should().BeTrue();
        inventory.QuantityOnHand.Should().Be(0);
    }

    // ---------- SetStock ----------
    [Fact]
    public void SetStock_WithValidQuantity_OverwritesQuantityOnHand()
    {
        var inventory = Inventory.Create(Guid.NewGuid(), quantity: 10).Value!;

        var result = inventory.SetStock(50);

        result.IsSuccess.Should().BeTrue();
        inventory.QuantityOnHand.Should().Be(50);
    }

    [Fact]
    public void SetStock_WithZero_Succeeds()
    {
        var inventory = Inventory.Create(Guid.NewGuid(), quantity: 10).Value!;

        var result = inventory.SetStock(0);

        result.IsSuccess.Should().BeTrue();
        inventory.QuantityOnHand.Should().Be(0);
    }

    [Fact]
    public void SetStock_WithNegativeQuantity_ReturnsFailure()
    {
        var inventory = Inventory.Create(Guid.NewGuid(), quantity: 10).Value!;

        var result = inventory.SetStock(-1);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(InventoryErrors.InvalidQuantity);
        inventory.QuantityOnHand.Should().Be(10);
    }
}
