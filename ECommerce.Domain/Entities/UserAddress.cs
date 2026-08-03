using ECommerce.Domain.Entities.Errors;
using ECommerce.Domain.Results;

namespace ECommerce.Domain.Entities;

public sealed class UserAddress : BaseEntity
{
    public const int MaxNameLength = 100;
    public const int MaxPhoneLength = 32;
    public const int MaxCountryLength = 100;
    public const int MaxCityLength = 100;
    public const int MaxStreetLength = 200;
    public const int MaxPostalCodeLength = 20;

    public Guid UserId { get; private set; }
    public string RecipientFirstName { get; private set; } = default!;   // if someone else recieved the order like a present
    public string RecipientLastName { get; private set; } = default!;
    public string PhoneNumber { get; private set; } = default!;
    public string Country { get; private set; } = default!;
    public string City { get; private set; } = default!;
    public string Street { get; private set; } = default!;
    public string PostalCode { get; private set; } = default!;
    public bool IsDefault { get; private set; }

    public void ClearDefault() => IsDefault = false;

    public void MarkAsDefault() => IsDefault = true;

    private UserAddress() { }

    public UserAddress(Guid userId, string recipientFirstName, string recipientLastName, string phoneNumber, string country, string city, string street, string postalCode, bool isDefault)
    {
        UserId = userId;
        RecipientFirstName = recipientFirstName;
        RecipientLastName = recipientLastName;
        PhoneNumber = phoneNumber;
        Country = country;
        City = city;
        Street = street;
        PostalCode = postalCode;
        IsDefault = isDefault;

        Id = Guid.NewGuid();
    }

    public static ResultOfT<UserAddress> Create(
        Guid userId,
        string recipientFirstName,
        string recipientLastName,
        string phoneNumber,
        string country,
        string city,
        string street,
        string postalCode,
        bool isDefault = false)
    {
        if (userId == Guid.Empty)
            return ResultOfT<UserAddress>.Failure(UserAddressErrors.InvalidUserId);

        if (string.IsNullOrWhiteSpace(recipientFirstName) || string.IsNullOrWhiteSpace(recipientLastName))
            return ResultOfT<UserAddress>.Failure(UserAddressErrors.InvalidName);

        if (string.IsNullOrWhiteSpace(phoneNumber))
            return ResultOfT<UserAddress>.Failure(UserAddressErrors.InvalidPhone);

        if (string.IsNullOrWhiteSpace(country) || string.IsNullOrWhiteSpace(city) || string.IsNullOrWhiteSpace(street))
            return ResultOfT<UserAddress>.Failure(UserAddressErrors.InvalidLocation);

        if (string.IsNullOrWhiteSpace(postalCode))
            return ResultOfT<UserAddress>.Failure(UserAddressErrors.InvalidPostalCode);

        return ResultOfT<UserAddress>.Created(new UserAddress (
            userId, recipientFirstName, recipientLastName, phoneNumber, country, city, street, postalCode, isDefault));
    }
}
