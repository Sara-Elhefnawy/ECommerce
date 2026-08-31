using ECommerce.Domain.Entities;
using ECommerce.Domain.Entities.Errors;
using ECommerce.Domain.Results;
using FluentAssertions;
using Xunit;

namespace ECommerce.Tests.Unit.Domain;

public class UserAddressTests
{
    private static ResultOfT<UserAddress> CreateValid(
        Guid? userId = null,
        string firstName = "Jane",
        string lastName = "Doe",
        string phone = "0100000000",
        string country = "Egypt",
        string city = "Cairo",
        string street = "1 Main St",
        string postalCode = "12345") =>
        UserAddress.Create(userId ?? Guid.NewGuid(), firstName, lastName, phone, country, city, street, postalCode);

    [Fact]
    public void Create_WithValidData_Succeeds()
    {
        var result = CreateValid();

        result.IsSuccess.Should().BeTrue();
        result.Value!.IsDefault.Should().BeFalse(); // default when not specified
    }

    [Fact]
    public void Create_WithIsDefaultTrue_SetsIsDefault()
    {
        var result = UserAddress.Create(
            Guid.NewGuid(), "Jane", "Doe", "0100000000", "Egypt", "Cairo", "1 Main St", "12345", isDefault: true);

        result.Value!.IsDefault.Should().BeTrue();
    }

    [Fact]
    public void Create_WithEmptyUserId_ReturnsFailure()
    {
        var result = CreateValid(userId: Guid.Empty);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(UserAddressErrors.InvalidUserId);
    }

    [Theory]
    [InlineData("", "Doe")]
    [InlineData("Jane", "")]
    public void Create_WithMissingNameParts_ReturnsFailure(string first, string last)
    {
        var result = CreateValid(firstName: first, lastName: last);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(UserAddressErrors.InvalidName);
    }

    [Fact]
    public void Create_WithEmptyPhone_ReturnsFailure()
    {
        var result = CreateValid(phone: "");

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(UserAddressErrors.InvalidPhone);
    }

    [Theory]
    [InlineData("", "Cairo", "1 Main St")]
    [InlineData("Egypt", "", "1 Main St")]
    [InlineData("Egypt", "Cairo", "")]
    public void Create_WithMissingLocationPart_ReturnsFailure(string country, string city, string street)
    {
        var result = CreateValid(country: country, city: city, street: street);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(UserAddressErrors.InvalidLocation);
    }

    [Fact]
    public void Create_WithEmptyPostalCode_ReturnsFailure()
    {
        var result = CreateValid(postalCode: "");

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(UserAddressErrors.InvalidPostalCode);
    }

    // ---------- MarkAsDefault / ClearDefault ----------
    [Fact]
    public void MarkAsDefault_SetsIsDefaultTrue()
    {
        var address = CreateValid().Value!;

        address.MarkAsDefault();

        address.IsDefault.Should().BeTrue();
    }

    [Fact]
    public void ClearDefault_SetsIsDefaultFalse()
    {
        var address = UserAddress.Create(
            Guid.NewGuid(), "Jane", "Doe", "0100000000", "Egypt", "Cairo", "1 Main St", "12345", isDefault: true).Value!;

        address.ClearDefault();

        address.IsDefault.Should().BeFalse();
    }
}
