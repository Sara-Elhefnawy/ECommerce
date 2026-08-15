using ECommerce.Domain.Entities.Errors;
using ECommerce.Domain.Results;

namespace ECommerce.Domain.Entities;

public sealed class DeliveryMethod : BaseEntity
{
    public const int MaxNameLength = 100;
    public const int MaxDescriptionLength = 500;
    public const int MaxDeliveryTimeLength = 100;

    public string Name { get; private set; } = default!;
    public string? Description { get; private set; }
    public decimal Price { get; private set; }
    public string EstimatedDeliveryTime { get; private set; } = default!;
    public bool IsAvailable { get; private set; }
    public int DisplayOrder { get; private set; }

    private DeliveryMethod()
    {
    }

    public DeliveryMethod(
        string name, 
        string? description, 
        decimal price, 
        string estimatedDeliveryTime, 
        bool isAvailable, 
        int displayOrder)
    {
        Id = Guid.NewGuid();

        Name = name;
        Description = description;
        Price = price;
        EstimatedDeliveryTime = estimatedDeliveryTime;
        IsAvailable = isAvailable;
        DisplayOrder = displayOrder;
    }

    public static ResultOfT<DeliveryMethod> Create(
        string name,
        decimal price,
        string estimatedDeliveryTime,
        string? description = null,
    bool isAvailable = true,
    int displayOrder = 0)
    {
        if (string.IsNullOrWhiteSpace(name))
            return ResultOfT<DeliveryMethod>.Failure(DeliveryMethodErrors.InvalidName);

        if (name.Trim().Length > MaxNameLength)
            return ResultOfT<DeliveryMethod>.Failure(DeliveryMethodErrors.NameTooLong);

        if (price < 0)
            return ResultOfT<DeliveryMethod>.Failure(DeliveryMethodErrors.InvalidPrice);

        if (string.IsNullOrWhiteSpace(estimatedDeliveryTime))
            return ResultOfT<DeliveryMethod>.Failure(DeliveryMethodErrors.InvalidDeliveryTime);

        if (estimatedDeliveryTime.Trim().Length > MaxDeliveryTimeLength)
            return ResultOfT<DeliveryMethod>.Failure(DeliveryMethodErrors.DeliveryTimeTooLong);

        if (description?.Trim().Length > MaxDescriptionLength)
            return ResultOfT<DeliveryMethod>.Failure(DeliveryMethodErrors.DescriptionTooLong);

        return ResultOfT<DeliveryMethod>.Ok(new DeliveryMethod(
            name.Trim(),
            string.IsNullOrWhiteSpace(description) ? null : description.Trim(),
            price,
            estimatedDeliveryTime.Trim(),
            isAvailable,
            displayOrder
            ));
    }

    public Result Update(
        string name,
        decimal price,
        string estimatedDeliveryTime,
        string? description,
        bool isAvailable)
    {
        if (string.IsNullOrWhiteSpace(name))
            return Result.Failure(DeliveryMethodErrors.InvalidName);

        if (name.Trim().Length > MaxNameLength)
            return Result.Failure(DeliveryMethodErrors.NameTooLong);

        if (price < 0)
            return Result.Failure(DeliveryMethodErrors.InvalidPrice);

        if (string.IsNullOrWhiteSpace(estimatedDeliveryTime))
            return Result.Failure(DeliveryMethodErrors.InvalidDeliveryTime);

        if (estimatedDeliveryTime.Trim().Length > MaxDeliveryTimeLength)
            return Result.Failure(DeliveryMethodErrors.DeliveryTimeTooLong);

        if (description?.Trim().Length > MaxDescriptionLength)
            return Result.Failure(DeliveryMethodErrors.DescriptionTooLong);

        Name = name.Trim();
        Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim();
        Price = price;
        EstimatedDeliveryTime = estimatedDeliveryTime.Trim();
        IsAvailable = isAvailable;

        return Result.Ok();
    }

    public void SetDisplayOrder(int displayOrder)
        => DisplayOrder = displayOrder;
}
