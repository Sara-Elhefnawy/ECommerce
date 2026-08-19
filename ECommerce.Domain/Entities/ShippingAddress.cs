using ECommerce.Domain.Entities.Errors;
using ECommerce.Domain.Results;

namespace ECommerce.Domain.Entities;

public sealed class ShippingAddress
{
    public const int MaxNameLength = 100;
    public const int MaxPhoneLength = 32;
    public const int MaxCountryLength = 100;
    public const int MaxCityLength = 100;
    public const int MaxStreetLength = 200;
    public const int MaxPostalCodeLength = 20;

    public string RecipientFirstName { get; private set; } = default!;
    public string RecipientLastName { get; private set; } = default!;
    public string PhoneNumber { get; private set; } = default!;
    public string Country { get; private set; } = default!;
    public string City { get; private set; } = default!;
    public string Street { get; private set; } = default!;
    public string PostalCode { get; private set; } = default!;

    private ShippingAddress()
    {
    }

    public ShippingAddress(
        string recipientFirstName, 
        string recipientLastName, 
        string phoneNumber, 
        string country, 
        string city, 
        string street, 
        string postalCode)
    {
        RecipientFirstName = recipientFirstName;
        RecipientLastName = recipientLastName;
        PhoneNumber = phoneNumber;
        Country = country;
        City = city;
        Street = street;
        PostalCode = postalCode;
    }

    public static ResultOfT<ShippingAddress> Create(
        string recipientFirstName,
        string recipientLastName,
        string phoneNumber,
        string country,
        string city,
        string street,
        string postalCode)
    {
        if (string.IsNullOrWhiteSpace(recipientFirstName) || string.IsNullOrWhiteSpace(recipientLastName))
            return ResultOfT<ShippingAddress>.Failure(OrderErrors.InvalidShippingName);

        if (string.IsNullOrWhiteSpace(phoneNumber))
            return ResultOfT<ShippingAddress>.Failure(OrderErrors.InvalidShippingPhone);

        if (string.IsNullOrWhiteSpace(country)
            || string.IsNullOrWhiteSpace(city)
            || string.IsNullOrWhiteSpace(street))
            return ResultOfT<ShippingAddress>.Failure(OrderErrors.InvalidShippingLocation);

        if (string.IsNullOrWhiteSpace(postalCode))
            return ResultOfT<ShippingAddress>.Failure(OrderErrors.InvalidShippingPostalCode);

        return ResultOfT<ShippingAddress>.Created(new ShippingAddress(
            recipientFirstName,
            recipientLastName,
            phoneNumber,
            country,
            city,
            street,
            postalCode));
    }

    public static ResultOfT<ShippingAddress> FromUserAddress(UserAddress address)
    {
        if (address is null)
            return ResultOfT<ShippingAddress>.Failure(OrderErrors.ShippingAddressRequired);

        return Create(
            address.RecipientFirstName,
            address.RecipientLastName,
            address.PhoneNumber,
            address.Country,
            address.City,
            address.Street,
            address.PostalCode);
    }
}
