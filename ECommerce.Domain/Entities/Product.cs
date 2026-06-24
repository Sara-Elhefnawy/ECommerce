namespace ECommerce.Domain.Entities;

public class Product : BaseEntity
{
    public string Name { get; private set; } = default!;
    public string Description { get; private set; } = default!;
    public string PictureUrl { get; private set; } = default!;
    public decimal Price { get; private set; }

    public ProductBrand Brand { get; private set; } = default!;
    public Guid BrandId { get; private set; }

    public ProductType Type { get; private set; } = default!;
    public Guid TypeId { get; private set; }
}
