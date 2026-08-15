using ECommerce.Domain.Entities;
using FluentValidation;
using System.Text.RegularExpressions;

namespace ECommerce.APP.Features.DeliveryMethods.Commands.Create;

public sealed class CreateDeliveryMethodValidator : AbstractValidator<CreateDeliveryMethodCommand>
{
    private static readonly Regex DeliveryTimeFormatRegex = new(
        @"^\d+(-\d+)?\s+(business\s+)?(hour|hours|day|days|week|weeks)$",
        RegexOptions.IgnoreCase | RegexOptions.Compiled);

    public CreateDeliveryMethodValidator()
    {
        RuleFor(x => x.Name)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
                .WithMessage("Delivery Method name is required.")
                .WithErrorCode("DeliveryMethod.Name.Required")
            .MaximumLength(DeliveryMethod.MaxNameLength)
                .WithMessage($"Delivery Method name cannot exceed {DeliveryMethod.MaxNameLength} characters.")
                .WithErrorCode("DeliveryMethod.Name.TooLong")
            .Must(name => name.Any(char.IsLetter))
                .WithMessage("Delivery Method name must contain at least one letter.")
                .WithErrorCode("DeliveryMethod.Name.MustContainLetter");

        RuleFor(x => x.Description)
            .MaximumLength(DeliveryMethod.MaxDescriptionLength)
            .WithMessage($"Description cannot exceed {DeliveryMethod.MaxDescriptionLength} characters.")
            .WithErrorCode("DeliveryMethod.Description.TooLong");

        RuleFor(x => x.Price)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Delivery Method price cannot be negative.")
            .WithErrorCode("DeliveryMethod.Price.Invalid");

        RuleFor(x => x.EstimatedDeliveryTime)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
                .WithMessage("Estimated delivery time is required.")
                .WithErrorCode("DeliveryMethod.EstimatedDeliveryTime.Required")
            .MaximumLength(DeliveryMethod.MaxDeliveryTimeLength)
                .WithMessage($"Estimated delivery time cannot exceed {DeliveryMethod.MaxDeliveryTimeLength} characters.")
                .WithErrorCode("DeliveryMethod.EstimatedDeliveryTime.TooLong")
            .Must(BeAValidDeliveryTimeFormat)
                .WithMessage("Estimated delivery time must be a duration like '3-5 days', '24 hours', '1-2 weeks', or 'Same day'.")
                .WithErrorCode("DeliveryMethod.EstimatedDeliveryTime.InvalidFormat");
    }

    private static bool BeAValidDeliveryTimeFormat(string value)
    {
        var trimmed = value.Trim();

        // Special-cased phrases that don't fit the "number + unit" pattern
        // but are still legitimate, common delivery descriptions.
        if (trimmed.Equals("Same day", StringComparison.OrdinalIgnoreCase) ||
            trimmed.Equals("Next day", StringComparison.OrdinalIgnoreCase))
            return true;

        return DeliveryTimeFormatRegex.IsMatch(trimmed);
    }
}
