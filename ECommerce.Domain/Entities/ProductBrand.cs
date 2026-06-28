namespace ECommerce.Domain.Entities;

public class ProductBrand : BaseEntity
{
    public string Name { get; private set; } = default!;

    public ICollection<Product> Products { get; private set; } = [];

    private ProductBrand() { }

    // Factory Design Methods
    public static ProductBrand Create(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        return new() { Name = name.Trim() };
    }
}
