namespace ECommerce.APP.Features.Orders.DTOs;

public sealed record ShippingAddressResponse(
    string RecipientFirstName,
    string RecipientLastName,
    string PhoneNumber,
    string Country,
    string City,
    string Street,
    string PostalCode
    );
