using FluentValidation;

namespace ECommerce.APP.Features.DeliveryMethods.Commands.Delete;

public sealed class DeleteDeliveryMethodValidator : AbstractValidator<DeleteDeliveryMethodCommand>
{
    public DeleteDeliveryMethodValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Delivery method ID must not be empty.")
            .WithErrorCode("DeliveryMethod.Id.Required");
    }
}
