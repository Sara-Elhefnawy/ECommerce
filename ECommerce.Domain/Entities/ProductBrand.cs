using ECommerce.Domain.Common;
using ECommerce.Domain.Common.Errors;

namespace ECommerce.Domain.Entities;

public class ProductBrand : BaseEntity
{
    public const int MaxNameLength = 100;

    public string Name { get; private set; } = default!;
    public ICollection<Product> Products { get; private set; } = [];

    private ProductBrand() { }

    private ProductBrand(string name) 
    { 
        Name = name;

        Id = Guid.NewGuid();
    }

    // Factory Design Methods
    public static ResultOfT<ProductBrand> Create(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return ResultOfT<ProductBrand>.Failure(BrandErrors.InvalidName);

        if (name.Length > MaxNameLength)
            return ResultOfT<ProductBrand>.Failure(BrandErrors.NameTooLong);

        return ResultOfT<ProductBrand>.Created(new ProductBrand(name));
    }
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        return new() { Name = name.Trim() };
    }
}
