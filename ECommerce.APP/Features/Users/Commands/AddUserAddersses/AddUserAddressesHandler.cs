using ECommerce.APP.Identity;
using ECommerce.APP.Mediator;
using ECommerce.Domain.Abstractions.Repositories;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Entities.Errors;
using ECommerce.Domain.Results;
using Microsoft.Extensions.Logging;

namespace ECommerce.APP.Features.Users.Commands.AddUserAddersses;

public sealed class AddUserAddressesHandler(
    ICurrentUserService currentUser,
    IRepository<UserAddress> addressRepository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<AddUserAddressesCommand, ResultOfT<UserAddressResponse>>
{
    public async Task<ResultOfT<UserAddressResponse>> Handle(
        AddUserAddressesCommand request,
        CancellationToken ct = default)
    {
        if (currentUser.UserId is null)
            return IdentityErrors.InvalidCredentials;

        var existingAddress = await addressRepository.FirstOrDefaultAsync(
            new ExistingUserAddressSpecification(
                currentUser.UserId.Value,
                request.RecipientFirstName,
                request.RecipientLastName,
                request.PhoneNumber,
                request.Country,
                request.City,
                request.Street,
                request.PostalCode),
            ct);

        if (existingAddress is not null)
            return UserAddressErrors.AlreadyExists;

        if (request.IsDefault)
        {
            // clear IsDefault on any existing default address(es) BEFORE
            // adding the new one — both changes get committed together in
            // the single SaveChangesAsync call below, so a crash mid-way
            // can't leave the user with zero (or two) default addresses
            var existingDefaults = await addressRepository.ListAsync(
                new AddUserAddressesSpecification(currentUser.UserId.Value), ct);

            foreach (var addr in existingDefaults)
            {
                addr.ClearDefault();
                addressRepository.Update(addr);
            }
        }

        var createResult = UserAddress.Create(
        currentUser.UserId.Value,
        request.RecipientFirstName,
        request.RecipientLastName,
        request.PhoneNumber,
        request.Country,
        request.City,
        request.Street,
        request.PostalCode,
        request.IsDefault);

        if (createResult.IsFailure)
            return createResult.Error!;

        var address = createResult.Value;
        addressRepository.Add(address);
        await unitOfWork.SaveChangesAsync(ct);

        return new UserAddressResponse(
            address.Id,
            address.RecipientFirstName,
            address.RecipientLastName,
            address.PhoneNumber,
            address.Country,
            address.City,
            address.Street,
            address.PostalCode,
            address.IsDefault);
    }
}
