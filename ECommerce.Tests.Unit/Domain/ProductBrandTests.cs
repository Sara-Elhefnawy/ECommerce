using ECommerce.Domain.Entities;
using ECommerce.Domain.Entities.Errors;
using FluentAssertions;
using Xunit;

namespace ECommerce.Tests.Unit.Domain;

public class ProductBrandTests
{
    [Fact]
    public void Create_WithValidName_Succeeds()
    {
        var result = ProductBrand.Create("Nike");

        result.IsSuccess.Should().BeTrue();
        result.Value!.Name.Should().Be("Nike");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithEmptyName_ReturnsFailure(string name)
    {
        var result = ProductBrand.Create(name);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(BrandErrors.InvalidName);
    }

    [Fact]
    public void Create_NameOverMaxLength_ReturnsFailure()
    {
        var result = ProductBrand.Create(new string('a', ProductBrand.MaxNameLength + 1));

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(BrandErrors.NameTooLong);
    }

    [Fact]
    public void Rename_WithValidName_UpdatesAndTrims()
    {
        var brand = ProductBrand.Create("Nike").Value!;

        var result = brand.Rename("  Adidas  ");

        result.IsSuccess.Should().BeTrue();
        brand.Name.Should().Be("Adidas");
    }

    [Fact]
    public void Rename_WithEmptyName_ReturnsFailure()
    {
        var brand = ProductBrand.Create("Nike").Value!;

        var result = brand.Rename("");

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(BrandErrors.InvalidName);
    }
}
