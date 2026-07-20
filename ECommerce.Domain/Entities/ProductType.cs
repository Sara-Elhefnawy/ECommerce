using ECommerce.Domain.Common;
using ECommerce.Domain.Common.Errors;

namespace ECommerce.Domain.Entities;

public class ProductType : BaseEntity
{
    public const int MaxNameLength = 100;

    public string Name { get; private set; } = default!;
    public ICollection<Product> Products { get; private set; } = [];

    private ProductType() { }

    private ProductType(string name)
    {
        Name = name;

        Id = Guid.NewGuid();
    }

    // Factory Design Methods
    public static ResultOfT<ProductType> Create(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return ResultOfT<ProductType>.Failure(TypeErrors.InvalidName);

        if (name.Length > MaxNameLength)
            return ResultOfT<ProductType>.Failure(TypeErrors.NameTooLong);

        return ResultOfT<ProductType>.Created(new ProductType(name));
    }

    }
}
