namespace ECommerce.Domain.Entities;

public class ProductBrand : BaseEntity
{
    public string Name { get; private set; } = default!;

    public IReadOnlyList<Product> Products { get; set; } = [];
}