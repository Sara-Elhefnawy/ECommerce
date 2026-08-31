using ECommerce.Domain.Entities;
using ECommerce.Domain.Entities.Errors;
using FluentAssertions;
using Xunit;

namespace ECommerce.Tests.Unit.Domain;

public class CartTests
{
    private static readonly Guid ProductA = Guid.NewGuid();
    private static readonly Guid ProductB = Guid.NewGuid();

    private static Cart CreateCart() => Cart.CreateEmpty(Guid.NewGuid()).Value!;

    // ---------- CreateEmpty ----------
    [Fact]
    public void CreateEmpty_WithValidBuyerId_ReturnsCartWithNoItems()
    {
        var buyerId = Guid.NewGuid();

        var result = Cart.CreateEmpty(buyerId);

        result.IsSuccess.Should().BeTrue();
        result.Value!.BuyerId.Should().Be(buyerId);
        result.Value.Items.Should().BeEmpty();
        result.Value.TotalQuantity.Should().Be(0);
        result.Value.ItemsTotal.Should().Be(0m);
    }

    [Fact]
    public void CreateEmpty_WithEmptyBuyerId_ReturnsFailure()
    {
        var result = Cart.CreateEmpty(Guid.Empty);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(CartErrors.InvalidBuyerId);
    }

    // ---------- AddItem ----------
    [Fact]
    public void AddItem_NewProduct_AddsAsNewLineItem()
    {
        var cart = CreateCart();

        var result = cart.AddItem(ProductA, "Product A", "url", 10m, 2);

        result.IsSuccess.Should().BeTrue();
        cart.Items.Should().ContainSingle(i => i.ProductId == ProductA && i.Quantity == 2);
    }

    [Fact]
    public void AddItem_ExistingProduct_IncreasesQuantityInsteadOfDuplicating()
    {
        var cart = CreateCart();
        cart.AddItem(ProductA, "Product A", "url", 10m, 2);

        var result = cart.AddItem(ProductA, "Product A", "url", 10m, 3);

        result.IsSuccess.Should().BeTrue();
        cart.Items.Should().ContainSingle(); // still one line, not two
        cart.Items.Single().Quantity.Should().Be(5);
    }

    [Fact]
    public void AddItem_WithNonPositiveQuantity_ReturnsFailure()
    {
        var cart = CreateCart();

        var result = cart.AddItem(ProductA, "Product A", "url", 10m, 0);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(CartErrors.InvalidQuantity);
        cart.Items.Should().BeEmpty();
    }

    // ---------- RemoveItem ----------
    [Fact]
    public void RemoveItem_ExistingProduct_RemovesIt()
    {
        var cart = CreateCart();
        cart.AddItem(ProductA, "Product A", "url", 10m, 2);

        var result = cart.RemoveItem(ProductA);

        result.IsSuccess.Should().BeTrue();
        cart.Items.Should().BeEmpty();
    }

    [Fact]
    public void RemoveItem_NotInCart_ReturnsFailure()
    {
        var cart = CreateCart();

        var result = cart.RemoveItem(ProductA);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(CartErrors.ItemNotFound);
    }

    // ---------- UpdateItemQuantity ----------
    [Fact]
    public void UpdateItemQuantity_WithPositiveQuantity_SetsQuantity()
    {
        var cart = CreateCart();
        cart.AddItem(ProductA, "Product A", "url", 10m, 2);

        var result = cart.UpdateItemQuantity(ProductA, 7);

        result.IsSuccess.Should().BeTrue();
        cart.Items.Single().Quantity.Should().Be(7);
    }

    [Fact]
    public void UpdateItemQuantity_WithZero_RemovesItem()
    {
        var cart = CreateCart();
        cart.AddItem(ProductA, "Product A", "url", 10m, 2);

        var result = cart.UpdateItemQuantity(ProductA, 0);

        result.IsSuccess.Should().BeTrue();
        cart.Items.Should().BeEmpty();
    }

    [Fact]
    public void UpdateItemQuantity_WithNegative_ReturnsFailure()
    {
        var cart = CreateCart();
        cart.AddItem(ProductA, "Product A", "url", 10m, 2);

        var result = cart.UpdateItemQuantity(ProductA, -1);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(CartErrors.InvalidQuantity);
    }

    [Fact]
    public void UpdateItemQuantity_ProductNotInCart_ReturnsFailure()
    {
        var cart = CreateCart();

        var result = cart.UpdateItemQuantity(ProductA, 5);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(CartErrors.ItemNotFound);
    }

    // ---------- Clear ----------
    [Fact]
    public void Clear_RemovesAllItems()
    {
        var cart = CreateCart();
        cart.AddItem(ProductA, "Product A", "url", 10m, 2);
        cart.AddItem(ProductB, "Product B", "url", 5m, 1);

        cart.Clear();

        cart.Items.Should().BeEmpty();
    }

    // ---------- MergeCartFromGuestCart ----------
    [Fact]
    public void MergeCartFromGuestCart_WithDisjointProducts_AddsAllItems()
    {
        var userCart = CreateCart();
        userCart.AddItem(ProductA, "Product A", "url", 10m, 1);

        var guestCart = CreateCart();
        guestCart.AddItem(ProductB, "Product B", "url", 5m, 2);

        var result = userCart.MergeCartFromGuestCart(guestCart);

        result.IsSuccess.Should().BeTrue();
        userCart.Items.Should().HaveCount(2);
    }

    [Fact]
    public void MergeCartFromGuestCart_WithOverlappingProduct_CombinesQuantities()
    {
        var userCart = CreateCart();
        userCart.AddItem(ProductA, "Product A", "url", 10m, 2);

        var guestCart = CreateCart();
        guestCart.AddItem(ProductA, "Product A", "url", 10m, 3);

        userCart.MergeCartFromGuestCart(guestCart);

        userCart.Items.Should().ContainSingle();
        userCart.Items.Single().Quantity.Should().Be(5);
    }

    [Fact]
    public void MergeCartFromGuestCart_WithSameCartInstance_ReturnsFailure()
    {
        var cart = CreateCart();

        var result = cart.MergeCartFromGuestCart(cart);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(CartErrors.CannotMergeSameCart);
    }

    // ---------- Computed properties ----------
    [Fact]
    public void TotalQuantityAndItemsTotal_ReflectAllItems()
    {
        var cart = CreateCart();
        cart.AddItem(ProductA, "Product A", "url", 10m, 2); // 20
        cart.AddItem(ProductB, "Product B", "url", 5m, 3);  // 15

        cart.TotalQuantity.Should().Be(5);
        cart.ItemsTotal.Should().Be(35m);
    }
}
