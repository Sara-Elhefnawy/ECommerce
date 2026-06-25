namespace ECommerce.Domain.Entities;

public class Product : BaseEntity
{
    private const int MaxNameLength = 150;
    private const int MaxDescriptionLength = 1000;
    private const int MaxPictureUrlLength = 500;

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
        SetName(name);
        SetDescription(description);
        SetPictureUrl(pictureUrl);
        SetPrice(price);
        SetBrandId(brandId);
        SetTypeId(typeId); 
    }

    // Factory Design Methods
    public static Product Create(
        string name,
        string description,
        string pictureUrl,
        decimal price,
        Guid brandId,
        Guid typeId)
        => new(name, description, pictureUrl, price, brandId, typeId);

    private void SetName(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentNullException(nameof(value), "Product name cannot be null or whitespace");

        if (value.Length > MaxNameLength)
            throw new InvalidDataException($"Product name cannot exceed {MaxNameLength} characters");

        Name = value;
    }

    private void SetDescription(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentNullException(nameof(value), "Product description cannot be null or whitespace");

        if (value.Length > MaxDescriptionLength)
            throw new InvalidDataException($"Product description cannot exceed {MaxDescriptionLength} characters");

        Description = value;
    }

    private void SetPictureUrl(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentNullException(nameof(value), "Product picture URL cannot be null or whitespace");

        if (value.Length > MaxPictureUrlLength)
            throw new InvalidDataException($"Product picture URL cannot exceed {MaxPictureUrlLength} characters");

        PictureUrl = value;
    }

    private void SetPrice(decimal value)
    {
        if (value < 0)
            throw new InvalidDataException("Product price cannot be negative");

        Price = value;
    }

    private void SetBrandId(Guid brandId)
    {
        if (brandId == Guid.Empty)
            throw new ArgumentException("Brand ID cannot be empty", nameof(brandId));

        BrandId = brandId;
    }

    private void SetTypeId(Guid typeId)
    {
        if (typeId == Guid.Empty)
            throw new ArgumentException("Type ID cannot be empty", nameof(typeId));

        TypeId = typeId;
    }
}
