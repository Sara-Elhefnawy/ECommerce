namespace ECommerce.APP.Features.Users;

public sealed record UserAddressResponse(
    Guid Id,
    string RecipientFirstName,
    string RecipientLastName,
    string PhoneNumber,
    string Country,
    string City,
    string Street,
    string PostalCode,
    bool IsDefault);
