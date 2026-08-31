using ECommerce.APP.Cachings.ResetPassword;
using ECommerce.APP.Email;
using ECommerce.APP.Features.Users.Commands.ResetPassword;
using ECommerce.APP.Identity;
using ECommerce.APP.Settings;
using ECommerce.APP.Token;
using ECommerce.Domain.Entities.Errors;
using ECommerce.Domain.Results;
using FluentAssertions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace ECommerce.Tests.Unit.Application.Users;

public class ResetPasswordHandlerTests
{
    private readonly Mock<IIdentityService> _identityService = new();
    private readonly Mock<IResetPasswordRepository> _resetRepo = new();
    private readonly Mock<IEmailSender> _emailSender = new();
    private readonly Mock<IHostEnvironment> _env = new();
    private readonly Mock<ILogger<ResetPasswordHandler>> _logger = new();
    private readonly ResetPasswordHandler _sut;

    public ResetPasswordHandlerTests()
    {
        // Assumes ResetPasswordSettings has a FrontendResetPasswordUrl string property —
        // that's the only member the handler reads.
        var settings = Options.Create(new ResetPasswordSettings { FrontendResetPasswordUrl = "https://app.test/reset" });

        _env.Setup(e => e.EnvironmentName).Returns(Environments.Production); // default: not Development

        _sut = new ResetPasswordHandler(
            _identityService.Object, _resetRepo.Object, _emailSender.Object,
            settings, _env.Object, _logger.Object);
    }

    // Assumes ResetPasswordCommand(string Email) — adjust if the property name differs.
    private static ResetPasswordCommand Command(string email = "jane@example.com") => new(email);

    [Fact]
    public async Task Handle_WhenEmailDoesNotExist_StillReturnsSuccessMessage_WithoutSendingEmail()
    {
        // Deliberate security behavior: never reveal whether an email is
        // registered, so a lookup failure still returns the same generic
        // success message as a real reset — just with no email actually sent.
        _identityService
            .Setup(s => s.GetUserByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(ResultOfT<AuthUserSnapshot>.Failure(IdentityErrors.UserNotFound));

        var result = await _sut.Handle(Command(), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        _emailSender.Verify(
            e => e.SendAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_WhenEmailExists_SavesTokenAndSendsEmail()
    {
        var userId = Guid.NewGuid();
        _identityService
            .Setup(s => s.GetUserByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new AuthUserSnapshot(userId, "jane@example.com", "Jane"));

        _resetRepo
            .Setup(r => r.SaveAsync(It.IsAny<string>(), userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Ok());

        _emailSender
            .Setup(e => e.SendAsync("jane@example.com", It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Ok());

        var result = await _sut.Handle(Command("jane@example.com"), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        _resetRepo.Verify(r => r.SaveAsync(It.IsAny<string>(), userId, It.IsAny<CancellationToken>()), Times.Once);
        _emailSender.Verify(
            e => e.SendAsync("jane@example.com", It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_WhenSavingTokenFails_ReturnsErrorWithoutSendingEmail()
    {
        _identityService
            .Setup(s => s.GetUserByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new AuthUserSnapshot(Guid.NewGuid(), "jane@example.com", "Jane"));

        _resetRepo
            .Setup(r => r.SaveAsync(It.IsAny<string>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Failure(CacheErrors.OperationFailed));

        var result = await _sut.Handle(Command(), CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(CacheErrors.OperationFailed);
        _emailSender.Verify(
            e => e.SendAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_WhenEmailSendFails_ReturnsError()
    {
        _identityService
            .Setup(s => s.GetUserByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new AuthUserSnapshot(Guid.NewGuid(), "jane@example.com", "Jane"));

        _resetRepo
            .Setup(r => r.SaveAsync(It.IsAny<string>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Ok());

        _emailSender
            .Setup(e => e.SendAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Failure(IdentityErrors.EmailSendFailed));

        var result = await _sut.Handle(Command(), CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(IdentityErrors.EmailSendFailed);
    }
}
