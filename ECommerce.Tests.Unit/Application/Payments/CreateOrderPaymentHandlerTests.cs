using ECommerce.APP.Features.Payments.Commands.Create;
using ECommerce.APP.Identity;
using ECommerce.APP.Payments;
using ECommerce.APP.Settings;
using ECommerce.Domain.Abstractions.Repositories;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Entities.Enums;
using ECommerce.Domain.Entities.Errors;
using ECommerce.Domain.Results;
using ECommerce.Domain.Specifications;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace ECommerce.Tests.Unit.Application.Payments;

public class CreateOrderPaymentHandlerTests
{
    private readonly Mock<ICurrentUserService> _currentUser = new();
    private readonly Mock<IRepository<Order>> _orderRepository = new();
    private readonly Mock<IUnitOfWork> _uow = new();
    private readonly Mock<IPaymentService> _paymentService = new();
    private readonly CreateOrderPaymentHandler _sut;

    private readonly Guid _userId = Guid.NewGuid();
    private readonly Guid _orderId = Guid.NewGuid();

    public CreateOrderPaymentHandlerTests()
    {
        var stripeOptions = Options.Create(new StripeSettings
        {
            Currency = "usd",
            PublishableKey = "pk_test_x",
            SecretKey = "sk_test_x",
            WebhookSecret = "whsec_x"
        });

        _sut = new CreateOrderPaymentHandler(
            _currentUser.Object, _orderRepository.Object, _uow.Object, _paymentService.Object, stripeOptions);

        _currentUser.Setup(c => c.UserId).Returns(_userId);
    }

    // Builds a real Order via the domain factory so .Total is genuinely
    // computed, not faked — matches how CreateOrderPaymentHandler actually
    // uses order.Total to compute the Stripe amount.
    private Order ValidOrder()
    {
        var deliveryMethod = DeliveryMethod.Create("Standard", 5m, "3-5 days").Value!;
        var address = UserAddress.Create(
            _userId, "Jane", "Doe", "0100000000", "Egypt", "Cairo", "1 Main St", "12345").Value!;

        var items = new List<(Guid ProductId, string ProductName, string PictureUrl, decimal UnitPrice, int Quantity)>
        {
            (Guid.NewGuid(), "Product", "url", 10m, 2) // Total = 20 + 5 shipping = 25
        };

        return Order.Create(_userId, deliveryMethod, address, items).Value!;
    }

    // Same shortcut as OrderTests: builds an Order directly in a given
    // status via the public constructor, to test the AttachPaymentIntent
    // failure branch (only reachable from a non-Pending order).
    private static Order BuildOrderInStatus(OrderStatus status) => new(
        userId: Guid.NewGuid(), status: status, deliveryMethodId: Guid.NewGuid(),
        deliveryMethodName: "Standard", deliveryMethodPrice: 5m, deliveryMethodEstimatedTime: "3-5 days",
        shippingAddress: null!, shippingCost: 5m);

    private static CreateOrderPaymentCommand Command(Guid orderId) => new(OrderId: orderId);

    private PaymentIntentResult SuccessfulIntent(string id = "pi_123") =>
        new(id, "secret_abc", "requires_payment_method", "pk_test_x");

    [Fact]
    public async Task Handle_WhenUserNotAuthenticated_ReturnsUnauthorized()
    {
        _currentUser.Setup(c => c.UserId).Returns((Guid?)null);

        var result = await _sut.Handle(Command(_orderId), CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(OrderErrors.Unauthorized);
    }

    [Fact]
    public async Task Handle_WhenOrderNotFound_ReturnsNotFound()
    {
        _orderRepository
            .Setup(r => r.FirstOrDefaultAsync(It.IsAny<ISpecification<Order>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Order?)null);

        var result = await _sut.Handle(Command(_orderId), CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(OrderErrors.NotFound);
    }

    [Fact]
    public async Task Handle_WhenOrderHasNoExistingPaymentIntent_CallsCreateNotUpdate()
    {
        var order = ValidOrder();
        _orderRepository
            .Setup(r => r.FirstOrDefaultAsync(It.IsAny<ISpecification<Order>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(order);

        _paymentService
            .Setup(p => p.CreatePaymentIntentAsync(2500, "usd", order.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(ResultOfT<PaymentIntentResult>.Ok(SuccessfulIntent()));

        var result = await _sut.Handle(Command(order.Id), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        _paymentService.Verify(
            p => p.CreatePaymentIntentAsync(2500, "usd", order.Id, It.IsAny<CancellationToken>()), Times.Once);
        _paymentService.Verify(
            p => p.UpdatePaymentIntentAsync(It.IsAny<string>(), It.IsAny<long>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()),
            Times.Never);
        order.PaymentIntentId.Should().Be("pi_123");
        _uow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenOrderAlreadyHasPaymentIntent_CallsUpdateNotCreate()
    {
        var order = ValidOrder();
        order.AttachPaymentIntent("pi_existing");

        _orderRepository
            .Setup(r => r.FirstOrDefaultAsync(It.IsAny<ISpecification<Order>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(order);

        _paymentService
            .Setup(p => p.UpdatePaymentIntentAsync("pi_existing", 2500, order.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(ResultOfT<PaymentIntentResult>.Ok(SuccessfulIntent("pi_existing")));

        var result = await _sut.Handle(Command(order.Id), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        _paymentService.Verify(
            p => p.CreatePaymentIntentAsync(It.IsAny<long>(), It.IsAny<string>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()),
            Times.Never);
        _paymentService.Verify(
            p => p.UpdatePaymentIntentAsync("pi_existing", 2500, order.Id, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenStripeCallFails_ReturnsErrorWithoutSaving()
    {
        var order = ValidOrder();
        _orderRepository
            .Setup(r => r.FirstOrDefaultAsync(It.IsAny<ISpecification<Order>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(order);

        _paymentService
            .Setup(p => p.CreatePaymentIntentAsync(It.IsAny<long>(), It.IsAny<string>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(ResultOfT<PaymentIntentResult>.Failure(OrderErrors.PaymentFailed));

        var result = await _sut.Handle(Command(order.Id), CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        _uow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenOrderIsNotPending_ReturnsAttachFailureWithoutSaving()
    {
        // Regression-adjacent: AttachPaymentIntent only accepts Pending
        // orders. A non-Pending order reaching this handler (e.g. already
        // Processing) must fail cleanly here rather than silently
        // overwrite a paid order's PaymentIntentId.
        var order = BuildOrderInStatus(OrderStatus.Processing);

        _orderRepository
            .Setup(r => r.FirstOrDefaultAsync(It.IsAny<ISpecification<Order>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(order);

        _paymentService
            .Setup(p => p.CreatePaymentIntentAsync(It.IsAny<long>(), It.IsAny<string>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(ResultOfT<PaymentIntentResult>.Ok(SuccessfulIntent()));

        var result = await _sut.Handle(Command(order.Id), CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(OrderErrors.InvalidPaymentState);
        _uow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}
