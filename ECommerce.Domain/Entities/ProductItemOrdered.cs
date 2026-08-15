using ECommerce.Domain.Entities.Errors;
using ECommerce.Domain.Results;

namespace ECommerce.Domain.Entities;

public sealed class ProductItemOrdered
{
    public const int MaxProductNameLength = 200;
    public const int MaxPictureUrlLength = 500;

    public Guid ProductId { get; private set; }
    public string ProductName { get; private set; } = null!;
    public string PictureUrl { get; private set; } = null!;
    public decimal UnitPrice { get; private set; }

    private ProductItemOrdered()
    {
    }

    public ProductItemOrdered(
        Guid productId, 
        string productName, 
        string pictureUrl, 
        decimal unitPrice)
    {
        ProductId = productId;
        ProductName = productName;
        PictureUrl = pictureUrl;
        UnitPrice = unitPrice;
    }

    public static ResultOfT<ProductItemOrdered> Create(
        Guid productId,
        string productName,
        string pictureUrl,
        decimal unitPrice)
    {
        if (productId == Guid.Empty)
            return ResultOfT<ProductItemOrdered>.Failure(OrderErrors.InvalidProductId);

        if (string.IsNullOrWhiteSpace(productName))
            return ResultOfT<ProductItemOrdered>.Failure(OrderErrors.InvalidProductName);

        if (string.IsNullOrWhiteSpace(pictureUrl))
            return ResultOfT<ProductItemOrdered>.Failure(OrderErrors.InvalidPictureUrl);

        if (unitPrice < 0)
            return ResultOfT<ProductItemOrdered>.Failure(OrderErrors.InvalidUnitPrice);

        return ResultOfT<ProductItemOrdered>.Ok(new ProductItemOrdered(
            productId,
            productName.Trim(),
            pictureUrl.Trim(),
            unitPrice
            ));
    }
}
