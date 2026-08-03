using ECommerce.APP.Mediator;
using ECommerce.Domain.Results;

namespace ECommerce.APP.Features.Users.Commands.AddUserAddersses;

public sealed record AddUserAddressesCommand(
    string RecipientFirstName,
    string RecipientLastName,
    string PhoneNumber,
    string Country,
    string City,
    string Street,
    string PostalCode,
    bool IsDefault = false) : IRequest<ResultOfT<UserAddressResponse>>;
