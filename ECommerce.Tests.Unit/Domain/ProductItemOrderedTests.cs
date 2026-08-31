using ECommerce.Domain.Entities;
using ECommerce.Domain.Entities.Errors;
using FluentAssertions;
using Xunit;

namespace ECommerce.Tests.Unit.Domain;

public class ProductItemOrderedTests
{
    // This is the immutable snapshot of a product taken at order time
    // (see Order.Create in ECommerce.Domain.Entities.Order) — its own
    // validation just needs to guard against a corrupted/incomplete
    // snapshot, not re-validate catalog business rules.

    [Fact]
    public void Create_WithValidData_Succeeds()
    {
        var result = ProductItemOrdered.Create(Guid.NewGuid(), "Shoe", "url", 10m);

        result.IsSuccess.Should().BeTrue();
        result.Value!.ProductName.Should().Be("Shoe");
    }

    [Fact]
    public void Create_WithEmptyProductId_ReturnsFailure()
    {
        var result = ProductItemOrdered.Create(Guid.Empty, "Shoe", "url", 10m);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(OrderErrors.InvalidProductId);
    }

    [Fact]
    public void Create_WithEmptyName_ReturnsFailure()
    {
        var result = ProductItemOrdered.Create(Guid.NewGuid(), "", "url", 10m);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(OrderErrors.InvalidProductName);
    }

    [Fact]
    public void Create_WithEmptyPictureUrl_ReturnsFailure()
    {
        var result = ProductItemOrdered.Create(Guid.NewGuid(), "Shoe", "", 10m);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(OrderErrors.InvalidPictureUrl);
    }

    [Fact]
    public void Create_WithNegativeUnitPrice_ReturnsFailure()
    {
        var result = ProductItemOrdered.Create(Guid.NewGuid(), "Shoe", "url", -1m);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(OrderErrors.InvalidUnitPrice);
    }

    [Fact]
    public void Create_WithZeroUnitPrice_Succeeds()
    {
        // Only < 0 is rejected — a free promotional item is a valid snapshot.
        var result = ProductItemOrdered.Create(Guid.NewGuid(), "Shoe", "url", 0m);

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public void Create_TrimsNameAndPictureUrl()
    {
        var result = ProductItemOrdered.Create(Guid.NewGuid(), "  Shoe  ", "  url  ", 10m);

        result.Value!.ProductName.Should().Be("Shoe");
        result.Value.PictureUrl.Should().Be("url");
    }
}
