using FluentValidation;

namespace ECommerce.APP.Features.Users.Commands.AddUserAddersses;

public sealed class AddUserAddressesValidator : AbstractValidator<AddUserAddressesCommand>
{
    public AddUserAddressesValidator()
    {
        RuleFor(x => x.RecipientFirstName)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
                .WithMessage("Recipient first name is required.")
                .WithErrorCode("UserAddress.RecipientFirstName.Required")
            .MaximumLength(100)
                .WithMessage("Recipient first name cannot exceed 100 characters.")
                .WithErrorCode("UserAddress.RecipientFirstName.TooLong");

        RuleFor(x => x.RecipientLastName)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
                .WithMessage("Recipient last name is required.")
                .WithErrorCode("UserAddress.RecipientLastName.Required")
            .MaximumLength(100)
                .WithMessage("Recipient last name cannot exceed 100 characters.")
                .WithErrorCode("UserAddress.RecipientLastName.TooLong");

        RuleFor(x => x.PhoneNumber)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
                .WithMessage("Phone number is required.")
                .WithErrorCode("UserAddress.PhoneNumber.Required")
            .MaximumLength(30)
                .WithMessage("Phone number cannot exceed 30 characters.")
                .WithErrorCode("UserAddress.PhoneNumber.TooLong");

        RuleFor(x => x.Country)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
                .WithMessage("Country is required.")
                .WithErrorCode("UserAddress.Country.Required")
            .MaximumLength(100)
                .WithMessage("Country cannot exceed 100 characters.")
                .WithErrorCode("UserAddress.Country.TooLong");

        RuleFor(x => x.City)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
                .WithMessage("City is required.")
                .WithErrorCode("UserAddress.City.Required")
            .MaximumLength(100)
                .WithMessage("City cannot exceed 100 characters.")
                .WithErrorCode("UserAddress.City.TooLong");

        RuleFor(x => x.Street)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
                .WithMessage("Street is required.")
                .WithErrorCode("UserAddress.Street.Required")
            .MaximumLength(250)
                .WithMessage("Street cannot exceed 250 characters.")
                .WithErrorCode("UserAddress.Street.TooLong");

        RuleFor(x => x.PostalCode)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
                .WithMessage("Postal code is required.")
                .WithErrorCode("UserAddress.PostalCode.Required")
            .MaximumLength(20)
                .WithMessage("Postal code cannot exceed 20 characters.")
                .WithErrorCode("UserAddress.PostalCode.TooLong");
    }
}
