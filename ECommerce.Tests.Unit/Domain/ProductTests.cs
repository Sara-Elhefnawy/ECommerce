using ECommerce.Domain.Entities;
using ECommerce.Domain.Entities.Errors;
using ECommerce.Domain.Results;
using FluentAssertions;
using Xunit;

namespace ECommerce.Tests.Unit.Domain;

public class ProductTests
{
    private static readonly Guid ValidBrandId = Guid.NewGuid();
    private static readonly Guid ValidTypeId = Guid.NewGuid();

    private static ResultOfT<Product> CreateValid(
        string name = "Test Product",
        string description = "A description",
        string pictureUrl = "http://example.com/pic.jpg",
        decimal price = 10m) =>
        Product.Create(name, description, pictureUrl, price, ValidBrandId, ValidTypeId);

    // ---------- Create ----------
    [Fact]
    public void Create_WithValidData_Succeeds()
    {
        var result = CreateValid();

        result.IsSuccess.Should().BeTrue();
        result.Value!.Name.Should().Be("Test Product");
        result.Value.BrandId.Should().Be(ValidBrandId);
        result.Value.TypeId.Should().Be(ValidTypeId);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithEmptyName_ReturnsInvalidNameError(string name)
    {
        var result = CreateValid(name: name);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ProductErrors.InvalidName);
    }

    [Fact]
    public void Create_WithNameOverMaxLength_ReturnsInvalidNameError()
    {
        var tooLong = new string('a', Product.MaxNameLength + 1);

        var result = CreateValid(name: tooLong);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ProductErrors.InvalidName);
    }

    [Fact]
    public void Create_WithDescriptionOverMaxLength_ReturnsInvalidDescriptionError()
    {
        var tooLong = new string('a', Product.MaxDescriptionLength + 1);

        var result = CreateValid(description: tooLong);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ProductErrors.InvalidDescription);
    }

    [Fact]
    public void Create_WithPictureUrlOverMaxLength_ReturnsInvalidPictureUrlError()
    {
        var tooLong = "http://" + new string('a', Product.MaxPictureUrlLength);

        var result = CreateValid(pictureUrl: tooLong);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ProductErrors.InvalidPictureUrl);
    }

    [Fact]
    public void Create_WithZeroPrice_Succeeds()
    {
        // Create only rejects price < 0, not price == 0 — unlike ChangePrice
        // below, which requires strictly positive. That asymmetry is in the
        // real code, so the test documents it rather than "fixing" it silently.
        var result = CreateValid(price: 0m);

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public void Create_WithNegativePrice_ReturnsInvalidPriceError()
    {
        var result = CreateValid(price: -1m);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ProductErrors.InvalidPrice);
    }

    [Fact]
    public void Create_WithEmptyBrandId_ReturnsInvalidBrandError()
    {
        var result = Product.Create("Name", "Desc", "url", 10m, Guid.Empty, ValidTypeId);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ProductErrors.InvalidBrand);
    }

    [Fact]
    public void Create_WithEmptyTypeId_ReturnsInvalidTypeError()
    {
        var result = Product.Create("Name", "Desc", "url", 10m, ValidBrandId, Guid.Empty);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ProductErrors.InvalidType);
    }

    // ---------- Rename ----------
    [Fact]
    public void Rename_WithValidName_UpdatesNameAndTrims()
    {
        var product = CreateValid().Value!;

        var result = product.Rename("  New Name  ");

        result.IsSuccess.Should().BeTrue();
        product.Name.Should().Be("New Name");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Rename_WithEmptyName_ReturnsFailure(string name)
    {
        var product = CreateValid().Value!;

        var result = product.Rename(name);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ProductErrors.InvalidName);
    }

    [Fact]
    public void Rename_OverMaxLength_ReturnsFailure()
    {
        var product = CreateValid().Value!;
        var tooLong = new string('a', Product.MaxNameLength + 1);

        var result = product.Rename(tooLong);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ProductErrors.InvalidName);
    }

    // ---------- ChangeDescription ----------
    [Fact]
    public void ChangeDescription_WithValidValue_UpdatesAndTrims()
    {
        var product = CreateValid().Value!;

        var result = product.ChangeDescription("  New description  ");

        result.IsSuccess.Should().BeTrue();
        product.Description.Should().Be("New description");
    }

    [Fact]
    public void ChangeDescription_Empty_ReturnsFailure()
    {
        var product = CreateValid().Value!;

        var result = product.ChangeDescription("");

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ProductErrors.InvalidDescription);
    }

    // ---------- ChangePictureUrl ----------
    [Fact]
    public void ChangePictureUrl_WithValidUrl_UpdatesAndTrims()
    {
        var product = CreateValid().Value!;

        var result = product.ChangePictureUrl("  http://new.com/pic.jpg  ");

        result.IsSuccess.Should().BeTrue();
        product.PictureUrl.Should().Be("http://new.com/pic.jpg");
    }

    [Fact]
    public void ChangePictureUrl_Empty_ReturnsInvalidPictureUrlError()
    {
        var product = CreateValid().Value!;

        var result = product.ChangePictureUrl("");

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ProductErrors.InvalidPictureUrl);
    }

    [Fact]
    public void ChangePictureUrl_OverMaxLength_ReturnsImageTooLargeError()
    {
        // Distinct from Create/empty: this specific branch returns
        // ImageTooLarge, not InvalidPictureUrl — worth locking in explicitly
        // since it's an easy copy-paste error to introduce later.
        var product = CreateValid().Value!;
        var tooLong = "http://" + new string('a', Product.MaxPictureUrlLength);

        var result = product.ChangePictureUrl(tooLong);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ProductErrors.ImageTooLarge);
    }

    // ---------- ChangePrice ----------
    [Fact]
    public void ChangePrice_WithPositiveValue_Succeeds()
    {
        var product = CreateValid().Value!;

        var result = product.ChangePrice(25m);

        result.IsSuccess.Should().BeTrue();
        product.Price.Should().Be(25m);
    }

    [Fact]
    public void ChangePrice_WithZero_ReturnsFailure()
    {
        // Unlike Create, ChangePrice requires strictly > 0.
        var product = CreateValid().Value!;

        var result = product.ChangePrice(0m);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ProductErrors.InvalidPrice);
    }

    // ---------- ChangeBrand / ChangeType ----------
    [Fact]
    public void ChangeBrand_WithValidId_Succeeds()
    {
        var product = CreateValid().Value!;
        var newBrandId = Guid.NewGuid();

        var result = product.ChangeBrand(newBrandId);

        result.IsSuccess.Should().BeTrue();
        product.BrandId.Should().Be(newBrandId);
    }

    [Fact]
    public void ChangeBrand_WithEmptyId_ReturnsFailure()
    {
        var product = CreateValid().Value!;

        var result = product.ChangeBrand(Guid.Empty);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ProductErrors.InvalidBrand);
    }

    [Fact]
    public void ChangeType_WithValidId_Succeeds()
    {
        var product = CreateValid().Value!;
        var newTypeId = Guid.NewGuid();

        var result = product.ChangeType(newTypeId);

        result.IsSuccess.Should().BeTrue();
        product.TypeId.Should().Be(newTypeId);
    }

    [Fact]
    public void ChangeType_WithEmptyId_ReturnsFailure()
    {
        var product = CreateValid().Value!;

        var result = product.ChangeType(Guid.Empty);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ProductErrors.InvalidType);
    }
}
