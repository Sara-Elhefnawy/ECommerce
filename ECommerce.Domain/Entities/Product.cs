using ECommerce.Domain.Common;

namespace ECommerce.Domain.Entities;

public class Product : BaseEntity
{
    public const int MaxNameLength = 150;
    public const int MaxDescriptionLength = 1000;
    public const int MaxPictureUrlLength = 500;

    public string Name { get; private set; } = default!;
    public string Description { get; private set; } = default!;
    public string PictureUrl { get; private set; } = default!;
    public decimal Price { get; private set; }

    public ProductBrand Brand { get; private set; } = default!;
    public Guid BrandId { get; private set; }

    public ProductType Type { get; private set; } = default!;
    public Guid TypeId { get; private set; }

    private Product() { }
    
    private Product(
        string name, 
        string description, 
        string pictureUrl, 
        decimal price, 
        Guid brandId, 
        Guid typeId)
    {
        Name = name;
        Description = description;
        PictureUrl = pictureUrl;
        Price = price;
        BrandId = brandId;
        TypeId = typeId;
    }

    // Factory Design Methods with Result Pattern
    public static Result<Product> Create(
        string name,
        string description,
        string pictureUrl,
        decimal price,
        Guid brandId,
        Guid typeId)
    {
        var validationResult = ValidateAll(name, description, pictureUrl, price, brandId, typeId);

        if (validationResult.IsFailure)
            return Result<Product>.BadRequest(validationResult.Error!);

        return Result<Product>.Created(new Product(
            name, description, pictureUrl, price, brandId, typeId));
    }

    private static Result ValidateAll(
        string name,
        string description,
        string pictureUrl,
        decimal price,
        Guid brandId,
        Guid typeId)
    {
        var nameResult = ValidateName(name);
        if (nameResult.IsFailure)
            return nameResult;

        var descriptionResult = ValidateDescription(description);
        if (descriptionResult.IsFailure)
            return descriptionResult;

        var pictureResult = ValidatePictureUrl(pictureUrl);
        if (pictureResult.IsFailure)
            return pictureResult;

        var priceResult = ValidatePrice(price);
        if (priceResult.IsFailure)
            return priceResult;

        var brandResult = ValidateBrandId(brandId);
        if (brandResult.IsFailure)
            return brandResult;

        var typeResult = ValidateTypeId(typeId);
        if (typeResult.IsFailure)
            return typeResult;

        return Result.Ok();
    }

    private static Result ValidateName(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return Result.BadRequest(ProductErrors.InvalidName);

        if (value.Length > MaxNameLength)
            return Result.BadRequest(ProductErrors.InvalidName);

        return Result.Ok();
    }

    private static Result ValidateDescription(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return Result.BadRequest(ProductErrors.InvalidDescription);

        if (value.Length > MaxDescriptionLength)
            return Result.BadRequest(ProductErrors.InvalidDescription);

        return Result.Ok();
    }

    private static Result ValidatePictureUrl(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return Result.BadRequest(ProductErrors.InvalidPictureUrl);

        if (value.Length > MaxPictureUrlLength)
            return Result.BadRequest(ProductErrors.InvalidPictureUrl);

        return Result.Ok();
    }

    private static Result ValidatePrice(decimal value)
    {
        if (value < 0)
            return Result.BadRequest(ProductErrors.InvalidPrice);

        return Result.Ok();
    }

    private static Result ValidateBrandId(Guid brandId)
    {
        if (brandId == Guid.Empty)
            return Result.BadRequest(ProductErrors.InvalidBrand);

        return Result.Ok();
    }

    private static Result ValidateTypeId(Guid typeId)
    {
        if (typeId == Guid.Empty)
            return Result.BadRequest(ProductErrors.InvalidType);

        return Result.Ok();
    }
}
