namespace ECommerce.Domain.Entities;

public class ProductType : BaseEntity
{
    public string Name { get; private set; } = default!;

    public IReadOnlyList<Product> Products { get; set; } = [];
}