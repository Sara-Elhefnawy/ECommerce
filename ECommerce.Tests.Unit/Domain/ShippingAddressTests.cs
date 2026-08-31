using ECommerce.Domain.Entities;
using ECommerce.Domain.Entities.Errors;
using ECommerce.Domain.Results;
using FluentAssertions;
using Xunit;

namespace ECommerce.Tests.Unit.Domain;

public class ShippingAddressTests
{
    private static ResultOfT<ShippingAddress> CreateValid(
        string firstName = "Jane",
        string lastName = "Doe",
        string phone = "0100000000",
        string country = "Egypt",
        string city = "Cairo",
        string street = "1 Main St",
        string postalCode = "12345") =>
        ShippingAddress.Create(firstName, lastName, phone, country, city, street, postalCode);

    [Fact]
    public void Create_WithValidData_Succeeds()
    {
        var result = CreateValid();

        result.IsSuccess.Should().BeTrue();
    }

    [Theory]
    [InlineData("", "Doe")]
    [InlineData("Jane", "")]
    public void Create_WithMissingNameParts_ReturnsFailure(string first, string last)
    {
        var result = CreateValid(firstName: first, lastName: last);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(OrderErrors.InvalidShippingName);
    }

    [Fact]
    public void Create_WithEmptyPhone_ReturnsFailure()
    {
        var result = CreateValid(phone: "");

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(OrderErrors.InvalidShippingPhone);
    }

    [Theory]
    [InlineData("", "Cairo", "1 Main St")]
    [InlineData("Egypt", "", "1 Main St")]
    [InlineData("Egypt", "Cairo", "")]
    public void Create_WithMissingLocationPart_ReturnsFailure(string country, string city, string street)
    {
        var result = CreateValid(country: country, city: city, street: street);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(OrderErrors.InvalidShippingLocation);
    }

    [Fact]
    public void Create_WithEmptyPostalCode_ReturnsFailure()
    {
        var result = CreateValid(postalCode: "");

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(OrderErrors.InvalidShippingPostalCode);
    }

    // ---------- FromUserAddress ----------
    [Fact]
    public void FromUserAddress_WithValidAddress_CopiesAllFields()
    {
        var userAddress = UserAddress.Create(
            Guid.NewGuid(), "Jane", "Doe", "0100000000", "Egypt", "Cairo", "1 Main St", "12345").Value!;

        var result = ShippingAddress.FromUserAddress(userAddress);

        result.IsSuccess.Should().BeTrue();
        result.Value!.RecipientFirstName.Should().Be("Jane");
        result.Value.City.Should().Be("Cairo");
        result.Value.PostalCode.Should().Be("12345");
    }

    [Fact]
    public void FromUserAddress_WithNull_ReturnsShippingAddressRequiredError()
    {
        var result = ShippingAddress.FromUserAddress(null!);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(OrderErrors.ShippingAddressRequired);
    }
}
