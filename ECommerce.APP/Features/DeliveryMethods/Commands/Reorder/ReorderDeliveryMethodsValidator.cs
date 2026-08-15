using FluentValidation;

namespace ECommerce.APP.Features.DeliveryMethods.Commands.Reorder;

public sealed class ReorderDeliveryMethodsValidator : AbstractValidator<ReorderDeliveryMethodsCommand>
{
    public ReorderDeliveryMethodsValidator()
    {
        RuleFor(x => x.DeliveryMethodIds)
            .NotEmpty()
            .WithMessage("At least one delivery method is required.")
            .WithErrorCode("DeliveryMethod.Reorder.Required");

        RuleFor(x => x.DeliveryMethodIds)
            .Must(ids => ids.Distinct().Count() == ids.Count)
            .WithMessage("Duplicate delivery method IDs are not allowed.")
            .WithErrorCode("DeliveryMethod.Reorder.DuplicateIds");

        RuleForEach(x => x.DeliveryMethodIds)
            .NotEqual(Guid.Empty)
            .WithMessage("Delivery method ID cannot be empty.")
            .WithErrorCode("DeliveryMethod.Reorder.InvalidId");
    }
}
