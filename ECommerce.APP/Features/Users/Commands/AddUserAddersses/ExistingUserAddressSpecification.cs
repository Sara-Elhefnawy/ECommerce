using ECommerce.APP.Specifications;
using ECommerce.Domain.Entities;

namespace ECommerce.APP.Features.Users.Commands.AddUserAddersses;

public sealed class ExistingUserAddressSpecification : Specification<UserAddress>
{
    public ExistingUserAddressSpecification(
        Guid userId,
        string recipientFirstName,
        string recipientLastName,
        string phoneNumber,
        string country,
        string city,
        string street,
        string postalCode)
    {
        Query.Where(a =>
            a.UserId == userId &&
            a.RecipientFirstName.ToUpper().Trim() == recipientFirstName.ToUpper().Trim() &&
            a.RecipientLastName.ToUpper().Trim() == recipientLastName.ToUpper().Trim() &&
            a.PhoneNumber.Trim() == phoneNumber.Trim() &&
            a.Country.ToUpper().Trim() == country.ToUpper().Trim() &&
            a.City.ToUpper().Trim() == city.ToUpper().Trim() &&
            a.Street.ToUpper().Trim() == street.ToUpper().Trim() &&
            a.PostalCode.Trim() == postalCode.Trim());
    }
}
