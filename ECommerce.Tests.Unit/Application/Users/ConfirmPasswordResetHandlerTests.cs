using ECommerce.APP.Cachings.ResetPassword;
using ECommerce.APP.Features.Users.Commands.ConfirmPasswordReset;
using ECommerce.APP.Identity;
using ECommerce.APP.Token;
using ECommerce.Domain.Entities.Errors;
using ECommerce.Domain.Results;
using FluentAssertions;
using Moq;
using Xunit;

namespace ECommerce.Tests.Unit.Application.Users;

public class ConfirmPasswordResetHandlerTests
{
    private readonly Mock<IIdentityService> _identityService = new();
    private readonly Mock<IResetPasswordRepository> _resetRepo = new();
    private readonly ConfirmPasswordResetHandler _sut;

    public ConfirmPasswordResetHandlerTests()
    {
        _sut = new ConfirmPasswordResetHandler(_identityService.Object, _resetRepo.Object);
    }

    // Assumes ConfirmPasswordResetCommand(string Email, string PasswordResetToken, string NewPassword).
    private static ConfirmPasswordResetCommand Command(
        string email = "jane@example.com",
        string token = "raw-token",
        string newPassword = "NewP@ssw0rd") => new(email, token, newPassword);

    [Theory]
    [InlineData("", "NewP@ssw0rd")]
    [InlineData("token", "")]
    public async Task Handle_WithMissingTokenOrPassword_ReturnsInvalidResetInput(string token, string password)
    {
        var result = await _sut.Handle(Command(token: token, newPassword: password), CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        _identityService.Verify(
            s => s.GetUserByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenEmailNotFound_PropagatesError()
    {
        _identityService
            .Setup(s => s.GetUserByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(ResultOfT<AuthUserSnapshot>.Failure(IdentityErrors.UserNotFound));

        var result = await _sut.Handle(Command(), CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(IdentityErrors.UserNotFound);
    }

    [Fact]
    public async Task Handle_WhenTokenNotFoundInRepository_ReturnsInvalidOrExpiredResetLink()
    {
        var userId = Guid.NewGuid();
        _identityService
            .Setup(s => s.GetUserByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new AuthUserSnapshot(userId, "jane@example.com", "Jane"));

        _resetRepo
            .Setup(r => r.GetUserIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(ResultOfT<Guid?>.Ok(null));

        var result = await _sut.Handle(Command(), CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(IdentityErrors.InvalidOrExpiredResetLink);
    }

    [Fact]
    public async Task Handle_WhenTokenBelongsToDifferentUser_ReturnsInvalidOrExpiredResetLink()
    {
        // Security-critical case: prevents pairing a valid token issued
        // for user A with an attacker-supplied email belonging to user B.
        var requestingUserId = Guid.NewGuid();
        var tokenOwnerUserId = Guid.NewGuid(); // deliberately different

        _identityService
            .Setup(s => s.GetUserByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new AuthUserSnapshot(requestingUserId, "jane@example.com", "Jane"));

        _resetRepo
            .Setup(r => r.GetUserIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(ResultOfT<Guid?>.Ok(tokenOwnerUserId));

        var result = await _sut.Handle(Command(), CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(IdentityErrors.InvalidOrExpiredResetLink);
        _identityService.Verify(
            s => s.ResetPasswordAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenResetPasswordFails_ReturnsErrorAndDoesNotConsumeToken()
    {
        // The token must survive a failed reset attempt (e.g. weak
        // password) so the user can retry with the same link.
        var userId = Guid.NewGuid();
        _identityService
            .Setup(s => s.GetUserByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new AuthUserSnapshot(userId, "jane@example.com", "Jane"));

        _resetRepo
            .Setup(r => r.GetUserIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(ResultOfT<Guid?>.Ok(userId));

        _identityService
            .Setup(s => s.ResetPasswordAsync(userId, It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Failure(IdentityErrors.InvalidCredentials)); // adjust error name if different

        var result = await _sut.Handle(Command(), CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        _resetRepo.Verify(r => r.DeleteAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WithValidData_ResetsPasswordAndConsumesTokenAndReturnsSuccess()
    {
        var userId = Guid.NewGuid();
        _identityService
            .Setup(s => s.GetUserByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new AuthUserSnapshot(userId, "jane@example.com", "Jane"));

        _resetRepo
            .Setup(r => r.GetUserIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(ResultOfT<Guid?>.Ok(userId));

        _identityService
            .Setup(s => s.ResetPasswordAsync(userId, It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Ok());

        var result = await _sut.Handle(Command(), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.UserId.Should().Be(userId);
        _resetRepo.Verify(r => r.DeleteAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}
