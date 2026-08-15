using ECommerce.Domain.Entities;
using FluentValidation;

namespace ECommerce.APP.Features.DeliveryMethods.Commands.Update;

public sealed class UpdateDeliveryMethodValidator : AbstractValidator<UpdateDeliveryMethodCommand>
{
    public UpdateDeliveryMethodValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Delivery method ID is required.")
            .WithErrorCode("DeliveryMethod.Id.Required");

        RuleFor(x => x.Name)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
                .WithMessage("Delivery method name is required.")
                .WithErrorCode("DeliveryMethod.Name.Required")
            .MaximumLength(DeliveryMethod.MaxNameLength)
                .WithMessage($"Delivery method name cannot exceed {DeliveryMethod.MaxNameLength} characters.")
                .WithErrorCode("DeliveryMethod.Name.TooLong")
            .Must(name => name.Any(char.IsLetter))
                .WithMessage("Delivery method name must contain at least one letter.")
                .WithErrorCode("DeliveryMethod.Name.Invalid");

        RuleFor(x => x.Description)
            .MaximumLength(DeliveryMethod.MaxDescriptionLength)
            .When(x => x.Description is not null)
            .WithMessage($"Delivery method description cannot exceed {DeliveryMethod.MaxDescriptionLength} characters.")
            .WithErrorCode("DeliveryMethod.Description.TooLong");

        RuleFor(x => x.Price)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Delivery method price cannot be negative.")
            .WithErrorCode("DeliveryMethod.Price.Invalid");

        RuleFor(x => x.EstimatedDeliveryTime)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
                .WithMessage("Estimated delivery time is required.")
                .WithErrorCode(
                    "DeliveryMethod.EstimatedDeliveryTime.Required")
            .MinimumLength(3)
                .WithMessage(
                    "Estimated delivery time must be at least 3 characters.")
                .WithErrorCode(
                    "DeliveryMethod.EstimatedDeliveryTime.TooShort")
            .MaximumLength(DeliveryMethod.MaxDeliveryTimeLength)
                .WithMessage(
                    $"Estimated delivery time cannot exceed {DeliveryMethod.MaxDeliveryTimeLength} characters.")
                .WithErrorCode(
                    "DeliveryMethod.EstimatedDeliveryTime.TooLong");
    }
}
