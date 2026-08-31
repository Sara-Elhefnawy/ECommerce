using ECommerce.APP.Features.Users.Commands.AddUserAddersses;
using ECommerce.APP.Identity;
using ECommerce.Domain.Abstractions.Repositories;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Entities.Errors;
using ECommerce.Domain.Specifications;
using FluentAssertions;
using Moq;
using Xunit;

namespace ECommerce.Tests.Unit.Application.Users;

public class AddUserAddressesHandlerTests
{
    private readonly Mock<ICurrentUserService> _currentUser = new();
    private readonly Mock<IRepository<UserAddress>> _addressRepository = new();
    private readonly Mock<IUnitOfWork> _uow = new();
    private readonly AddUserAddressesHandler _sut;

    private readonly Guid _userId = Guid.NewGuid();

    public AddUserAddressesHandlerTests()
    {
        _sut = new AddUserAddressesHandler(_currentUser.Object, _addressRepository.Object, _uow.Object);
        _currentUser.Setup(c => c.UserId).Returns(_userId);
    }

    private static AddUserAddressesCommand ValidCommand(bool isDefault = false) => new(
        RecipientFirstName: "Jane",
        RecipientLastName: "Doe",
        PhoneNumber: "0100000000",
        Country: "Egypt",
        City: "Cairo",
        Street: "1 Main St",
        PostalCode: "12345",
        IsDefault: isDefault);

    [Fact]
    public async Task Handle_WhenUserNotAuthenticated_ReturnsInvalidCredentials()
    {
        _currentUser.Setup(c => c.UserId).Returns((Guid?)null);

        var result = await _sut.Handle(ValidCommand(), CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(IdentityErrors.InvalidCredentials);
    }

    [Fact]
    public async Task Handle_WhenIdenticalAddressAlreadyExists_ReturnsAlreadyExists()
    {
        _addressRepository
            .Setup(r => r.FirstOrDefaultAsync(It.IsAny<ISpecification<UserAddress>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(UserAddress.Create(
                _userId, "Jane", "Doe", "0100000000", "Egypt", "Cairo", "1 Main St", "12345").Value);

        var result = await _sut.Handle(ValidCommand(), CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(UserAddressErrors.AlreadyExists);
        _addressRepository.Verify(r => r.Add(It.IsAny<UserAddress>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WithValidNewAddress_AddsAndSaves()
    {
        _addressRepository
            .Setup(r => r.FirstOrDefaultAsync(It.IsAny<ISpecification<UserAddress>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((UserAddress?)null);

        var result = await _sut.Handle(ValidCommand(), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        _addressRepository.Verify(r => r.Add(It.Is<UserAddress>(a => a.City == "Cairo")), Times.Once);
        _uow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenNewAddressIsDefault_ClearsExistingDefaultsFirst()
    {
        // Regression-style test for the ordering comment in the handler:
        // existing defaults must be cleared BEFORE the new address is
        // added, and both changes committed in the same SaveChangesAsync
        // call, so the user never ends up with zero or two defaults.
        var existingDefault = UserAddress.Create(
            _userId, "Old", "Recipient", "0111111111", "Egypt", "Giza", "2 Old St", "54321", isDefault: true).Value!;

        _addressRepository
            .Setup(r => r.FirstOrDefaultAsync(It.IsAny<ISpecification<UserAddress>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((UserAddress?)null);

        _addressRepository
            .Setup(r => r.ListAsync(It.IsAny<ISpecification<UserAddress>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([existingDefault]);

        var result = await _sut.Handle(ValidCommand(isDefault: true), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        existingDefault.IsDefault.Should().BeFalse(); // cleared
        _addressRepository.Verify(r => r.Update(existingDefault), Times.Once);
        _addressRepository.Verify(r => r.Add(It.Is<UserAddress>(a => a.IsDefault)), Times.Once);
    }

    [Fact]
    public async Task Handle_WithInvalidAddressData_ReturnsFailureWithoutSaving()
    {
        _addressRepository
            .Setup(r => r.FirstOrDefaultAsync(It.IsAny<ISpecification<UserAddress>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((UserAddress?)null);

        var invalidCommand = ValidCommand() with { PostalCode = "" };

        var result = await _sut.Handle(invalidCommand, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(UserAddressErrors.InvalidPostalCode);
        _uow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}
