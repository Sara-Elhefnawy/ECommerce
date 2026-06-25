namespace ECommerce.Domain.Entities;

public class ProductType : BaseEntity
{
    public string Name { get; private set; } = default!;

    public ICollection<Product> Products { get; private set; } = [];

    private ProductType() { }

    // Factory Design Methods
    public static ProductType Create(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        return new() { Name = name.Trim() };
    }
}