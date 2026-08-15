using FluentValidation;

namespace ECommerce.API.Endpoints.V1.DeliveryMethods.Reorder;

public sealed class ReorderDeliveryMethodsRequestValidator : AbstractValidator<ReorderDeliveryMethodsRequest>
{
    public ReorderDeliveryMethodsRequestValidator()
    {
        RuleFor(x => x.DeliveryMethodIds)
            .NotEmpty()
            .WithMessage("At least one delivery method ID is required.")
            .WithErrorCode("DeliveryMethod.Reorder.Required");

        RuleForEach(x => x.DeliveryMethodIds)
            .NotEmpty()
            .WithMessage("Delivery method ID cannot be empty.")
            .WithErrorCode("DeliveryMethod.Reorder.Id.Required")
            .Must(IsValidGuid)
            .WithMessage("Delivery method ID must be a valid GUID.")
            .WithErrorCode("DeliveryMethod.Reorder.Id.Invalid");

        RuleFor(x => x.DeliveryMethodIds)
            .Must(ids => ids.Distinct(StringComparer.OrdinalIgnoreCase).Count() == ids.Count)
            .WithMessage("Duplicate delivery method IDs are not allowed.")
            .WithErrorCode("DeliveryMethod.Reorder.DuplicateIds");
    }

    private static bool IsValidGuid(string id)
        => Guid.TryParse(id, out var guid) && guid != Guid.Empty;
}
