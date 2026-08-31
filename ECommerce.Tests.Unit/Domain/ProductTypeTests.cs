using ECommerce.Domain.Entities;
using ECommerce.Domain.Entities.Errors;
using FluentAssertions;
using Xunit;

namespace ECommerce.Tests.Unit.Domain;

public class ProductTypeTests
{
    [Fact]
    public void Create_WithValidName_Succeeds()
    {
        var result = ProductType.Create("Shoes");

        result.IsSuccess.Should().BeTrue();
        result.Value!.Name.Should().Be("Shoes");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithEmptyName_ReturnsFailure(string name)
    {
        var result = ProductType.Create(name);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(TypeErrors.InvalidName);
    }

    [Fact]
    public void Create_NameOverMaxLength_ReturnsFailure()
    {
        var result = ProductType.Create(new string('a', ProductType.MaxNameLength + 1));

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(TypeErrors.NameTooLong);
    }

    [Fact]
    public void Rename_WithValidName_UpdatesAndTrims()
    {
        var type = ProductType.Create("Shoes").Value!;

        var result = type.Rename("  Boots  ");

        result.IsSuccess.Should().BeTrue();
        type.Name.Should().Be("Boots");
    }
}
