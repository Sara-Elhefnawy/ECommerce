using ECommerce.Domain.Entities;
using ECommerce.Domain.Entities.Errors;
using ECommerce.Domain.Results;
using FluentAssertions;
using Xunit;

namespace ECommerce.Tests.Unit.Domain;

public class DeliveryMethodTests
{
    private static ResultOfT<DeliveryMethod> CreateValid(
        string name = "Standard",
        decimal price = 5m,
        string estimatedDeliveryTime = "3-5 days",
        string? description = "Standard shipping") =>
        DeliveryMethod.Create(name, price, estimatedDeliveryTime, description);

    // ---------- Create ----------
    [Fact]
    public void Create_WithValidData_Succeeds()
    {
        var result = CreateValid();

        result.IsSuccess.Should().BeTrue();
        result.Value!.IsAvailable.Should().BeTrue(); // default
        result.Value.DisplayOrder.Should().Be(0);     // default
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithEmptyName_ReturnsFailure(string name)
    {
        var result = CreateValid(name: name);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DeliveryMethodErrors.InvalidName);
    }

    [Fact]
    public void Create_NameOverMaxLength_ReturnsFailure()
    {
        var result = CreateValid(name: new string('a', 101));

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DeliveryMethodErrors.NameTooLong);
    }

    [Fact]
    public void Create_WithZeroPrice_Succeeds()
    {
        var result = CreateValid(price: 0m);

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public void Create_WithNegativePrice_ReturnsFailure()
    {
        var result = CreateValid(price: -1m);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DeliveryMethodErrors.InvalidPrice);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithEmptyDeliveryTime_ReturnsFailure(string time)
    {
        var result = CreateValid(estimatedDeliveryTime: time);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DeliveryMethodErrors.InvalidDeliveryTime);
    }

    [Fact]
    public void Create_DeliveryTimeOverMaxLength_ReturnsFailure()
    {
        var result = CreateValid(estimatedDeliveryTime: new string('a', 101));

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DeliveryMethodErrors.DeliveryTimeTooLong);
    }

    [Fact]
    public void Create_DescriptionOverMaxLength_ReturnsFailure()
    {
        var result = CreateValid(description: new string('a', 501));

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DeliveryMethodErrors.DescriptionTooLong);
    }

    [Fact]
    public void Create_WithNullDescription_Succeeds()
    {
        var result = CreateValid(description: null);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Description.Should().BeNull();
    }

    [Fact]
    public void Create_TrimsNameDescriptionAndDeliveryTime()
    {
        var result = DeliveryMethod.Create("  Express  ", 5m, "  1-2 days  ", "  Fast  ");

        result.Value!.Name.Should().Be("Express");
        result.Value.Description.Should().Be("Fast");
        result.Value.EstimatedDeliveryTime.Should().Be("1-2 days");
    }

    // ---------- Update ----------
    [Fact]
    public void Update_WithValidData_UpdatesAllFields()
    {
        var method = CreateValid().Value!;

        var result = method.Update("Express", 15m, "1-2 days", "Fast shipping", isAvailable: false);

        result.IsSuccess.Should().BeTrue();
        method.Name.Should().Be("Express");
        method.Price.Should().Be(15m);
        method.EstimatedDeliveryTime.Should().Be("1-2 days");
        method.Description.Should().Be("Fast shipping");
        method.IsAvailable.Should().BeFalse();
    }

    [Fact]
    public void Update_WithInvalidName_ReturnsFailureAndLeavesOriginalUnchanged()
    {
        var method = CreateValid(name: "Standard").Value!;

        var result = method.Update("", 15m, "1-2 days", null, true);

        result.IsFailure.Should().BeTrue();
        method.Name.Should().Be("Standard"); // unchanged
    }

    // ---------- SetDisplayOrder ----------
    [Fact]
    public void SetDisplayOrder_UpdatesValue()
    {
        var method = CreateValid().Value!;

        method.SetDisplayOrder(3);

        method.DisplayOrder.Should().Be(3);
    }
}
