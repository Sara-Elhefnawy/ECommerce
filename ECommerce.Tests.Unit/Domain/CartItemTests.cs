using ECommerce.Domain.Entities;
using ECommerce.Domain.Entities.Errors;
using ECommerce.Domain.Results;
using FluentAssertions;
using Xunit;

namespace ECommerce.Tests.Unit.Domain;

public class CartItemTests
{
    private static ResultOfT<CartItem> CreateValid(
        Guid? productId = null,
        string name = "Product",
        string pictureUrl = "http://example.com/pic.jpg",
        decimal unitPrice = 10m,
        int quantity = 2) =>
        CartItem.Create(productId ?? Guid.NewGuid(), name, pictureUrl, unitPrice, quantity);

    // ---------- Create ----------
    [Fact]
    public void Create_WithValidData_Succeeds()
    {
        var result = CreateValid();

        result.IsSuccess.Should().BeTrue();
        result.Value!.Quantity.Should().Be(2);
        result.Value.SubTotalPrice.Should().Be(20m);
    }

    [Fact]
    public void Create_WithEmptyProductId_ReturnsFailure()
    {
        var result = CreateValid(productId: Guid.Empty);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(CartErrors.InvalidProductId);
    }

    [Fact]
    public void Create_WithEmptyName_ReturnsFailure()
    {
        var result = CreateValid(name: "");

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(CartErrors.InvalidProductName);
    }

    [Fact]
    public void Create_WithEmptyPictureUrl_ReturnsFailure()
    {
        var result = CreateValid(pictureUrl: "");

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(CartErrors.InvalidPictureUrl);
    }

    [Fact]
    public void Create_WithZeroOrNegativeUnitPrice_ReturnsFailure()
    {
        var result = CreateValid(unitPrice: 0m);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(CartErrors.InvalidUnitPrice);
    }

    [Fact]
    public void Create_WithNegativeQuantity_ReturnsFailure()
    {
        var result = CreateValid(quantity: -1);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(CartErrors.InvalidQuantity);
    }

    [Fact]
    public void Create_WithZeroQuantity_Succeeds()
    {
        // Only negative quantity is rejected at Create — zero is allowed here
        // (Cart.AddItem is what actually enforces quantity > 0 for new items).
        var result = CreateValid(quantity: 0);

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public void Create_TrimsNameAndPictureUrl()
    {
        var result = CreateValid(name: "  Product  ", pictureUrl: "  http://x.com  ");

        result.Value!.ProductName.Should().Be("Product");
        result.Value.PictureUrl.Should().Be("http://x.com");
    }

    // ---------- SetQuantity ----------
    [Fact]
    public void SetQuantity_WithPositiveValue_UpdatesQuantity()
    {
        var item = CreateValid().Value!;

        var result = item.SetQuantity(10);

        result.IsSuccess.Should().BeTrue();
        item.Quantity.Should().Be(10);
    }

    [Fact]
    public void SetQuantity_WithNegativeValue_ReturnsFailure()
    {
        var item = CreateValid().Value!;

        var result = item.SetQuantity(-1);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(CartErrors.InvalidQuantity);
    }

    // ---------- IncreaseQuantity ----------
    [Fact]
    public void IncreaseQuantity_WithPositiveAmount_AddsToQuantity()
    {
        var item = CreateValid(quantity: 2).Value!;

        var result = item.IncreaseQuantity(3);

        result.IsSuccess.Should().BeTrue();
        item.Quantity.Should().Be(5);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void IncreaseQuantity_WithNonPositiveAmount_ReturnsFailure(int amount)
    {
        var item = CreateValid(quantity: 2).Value!;

        var result = item.IncreaseQuantity(amount);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(CartErrors.InvalidQuantityIncrement);
        item.Quantity.Should().Be(2); // unchanged
    }

    // ---------- UpdateUnitPrice ----------
    [Fact]
    public void UpdateUnitPrice_WithPositiveValue_UpdatesPrice()
    {
        var item = CreateValid().Value!;

        var result = item.UpdateUnitPrice(15m);

        result.IsSuccess.Should().BeTrue();
        item.UnitPrice.Should().Be(15m);
    }

    [Fact]
    public void UpdateUnitPrice_WithZeroOrNegative_ReturnsFailure()
    {
        var item = CreateValid().Value!;

        var result = item.UpdateUnitPrice(0m);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(CartErrors.InvalidUnitPrice);
    }
}
