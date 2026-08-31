using ECommerce.APP.Features.Users.Commands.UpdateUser;
using ECommerce.APP.Features.Users.Commands.UpdateUser.Common;
using ECommerce.APP.Identity;
using ECommerce.APP.Token;
using ECommerce.Domain.Entities.Errors;
using ECommerce.Domain.Results;
using FluentAssertions;
using Moq;
using Xunit;

namespace ECommerce.Tests.Unit.Application.Users;

public class UpdateUserHandlerTests
{
    private readonly Mock<ICurrentUserService> _currentUser = new();
    private readonly Mock<IIdentityService> _identityService = new();
    private readonly UpdateUserHandler _sut;

    public UpdateUserHandlerTests()
    {
        _sut = new UpdateUserHandler(_currentUser.Object, _identityService.Object);
    }

    // Assumes: UpdateUserCommand(Optional<string?> UserDisplayName) — adjust if the property name differs.
    private static UpdateUserCommand Command(Optional<string?> displayName) => new(displayName);

    [Fact]
    public async Task Handle_WhenUserNotAuthenticated_ReturnsInvalidCredentials()
    {
        _currentUser.Setup(c => c.UserId).Returns((Guid?)null);

        var result = await _sut.Handle(Command(Optional<string?>.Set("New Name")), CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(IdentityErrors.InvalidCredentials);
    }

    [Fact]
    public async Task Handle_PassesTheOptionalStraightThroughToIdentityService()
    {
        // The handler doesn't branch on IsSet/Unset itself — that logic
        // lives inside UpdateProfileAsync. This test just locks in that
        // the handler forwards the Optional value unmodified, since a
        // future refactor accidentally unwrapping/defaulting it here
        // would silently break the "field omitted vs explicitly null"
        // distinction the whole Optional<T> type exists to preserve.
        var userId = Guid.NewGuid();
        _currentUser.Setup(c => c.UserId).Returns(userId);

        var displayNameUpdate = Optional<string?>.Set("New Name");

        _identityService
            .Setup(s => s.UpdateProfileAsync(userId, displayNameUpdate, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new AuthUserSnapshot(userId, "jane@example.com", "New Name"));

        var result = await _sut.Handle(Command(displayNameUpdate), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.UserDisplayName.Should().Be("New Name");
        _identityService.Verify(
            s => s.UpdateProfileAsync(userId, displayNameUpdate, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenIdentityServiceFails_PropagatesError()
    {
        var userId = Guid.NewGuid();
        _currentUser.Setup(c => c.UserId).Returns(userId);

        _identityService
            .Setup(s => s.UpdateProfileAsync(userId, It.IsAny<Optional<string?>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(ResultOfT<AuthUserSnapshot>.Failure(IdentityErrors.UserNotFound));

        var result = await _sut.Handle(Command(Optional<string?>.Unset()), CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(IdentityErrors.UserNotFound);
    }
}
