using ECommerce.APP.Features.Users.Queries.GetUser;
using ECommerce.APP.Identity;
using ECommerce.APP.Token;
using ECommerce.Domain.Entities.Errors;
using ECommerce.Domain.Results;
using FluentAssertions;
using Moq;
using Xunit;

namespace ECommerce.Tests.Unit.Application.Users;

public class GetCurrentUserHandlerTests
{
    private readonly Mock<IIdentityService> _identityService = new();
    private readonly Mock<ICurrentUserService> _currentUser = new();
    private readonly GetCurrentUserHandler _sut;

    public GetCurrentUserHandlerTests()
    {
        _sut = new GetCurrentUserHandler(_identityService.Object, _currentUser.Object);
    }

    [Fact]
    public async Task Handle_WhenUserNotAuthenticated_ReturnsInvalidCredentials()
    {
        _currentUser.Setup(c => c.UserId).Returns((Guid?)null);

        // Assumes GetCurrentUserQuery is parameterless — adjust if not.
        var result = await _sut.Handle(new GetCurrentUserQuery(), CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(IdentityErrors.InvalidCredentials);
        _identityService.Verify(s => s.GetUserByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenUserExists_ReturnsProfile()
    {
        var userId = Guid.NewGuid();
        _currentUser.Setup(c => c.UserId).Returns(userId);

        var snapshot = new AuthUserSnapshot(userId, "jane@example.com", "Jane");
        _identityService
            .Setup(s => s.GetUserByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(snapshot);

        var result = await _sut.Handle(new GetCurrentUserQuery(), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Email.Should().Be("jane@example.com");
    }

    [Fact]
    public async Task Handle_WhenIdentityServiceFails_PropagatesError()
    {
        var userId = Guid.NewGuid();
        _currentUser.Setup(c => c.UserId).Returns(userId);

        _identityService
            .Setup(s => s.GetUserByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(ResultOfT<AuthUserSnapshot>.Failure(IdentityErrors.UserNotFound));

        var result = await _sut.Handle(new GetCurrentUserQuery(), CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(IdentityErrors.UserNotFound);
    }
}
