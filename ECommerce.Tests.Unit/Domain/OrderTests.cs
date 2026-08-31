using ECommerce.Domain.Entities;
using ECommerce.Domain.Entities.Enums;
using ECommerce.Domain.Entities.Errors;
using FluentAssertions;
using Xunit;

namespace ECommerce.Tests.Unit.Domain;

public class OrderTests
{
    // Builds an Order directly in a given starting status via the public
    // constructor, bypassing Order.Create(). We don't need a real
    // DeliveryMethod/ShippingAddress here — none of the state-transition
    // methods under test (Cancel, AttachPaymentIntent, MarkAsPaid,
    // MarkAsPaymentFailed) read those fields, so `null!` for
    // ShippingAddress is a deliberate shortcut, not an oversight.
    private static Order BuildOrder(OrderStatus status) =>
        new(
            userId: Guid.NewGuid(),
            status: status,
            deliveryMethodId: Guid.NewGuid(),
            deliveryMethodName: "Standard",
            deliveryMethodPrice: 5m,
            deliveryMethodEstimatedTime: "3-5 days",
            shippingAddress: null!,
            shippingCost: 5m);

    // ---------- Cancel ----------
    [Fact]
    public void Cancel_FromPending_Succeeds()
    {
        var order = BuildOrder(OrderStatus.Pending);

        var result = order.Cancel();

        result.IsSuccess.Should().BeTrue();
        order.Status.Should().Be(OrderStatus.Cancelled);
    }

    [Fact]
    public void Cancel_FromPaymentFailed_Succeeds()
    {
        // Regression test: a customer who declines a card and doesn't
        // retry must be able to cancel and get their reserved stock
        // back (see CancelOrderHandler). Before today's fix this was
        // blocked, leaving stock held forever.
        var order = BuildOrder(OrderStatus.PaymentFailed);

        var result = order.Cancel();

        result.IsSuccess.Should().BeTrue();
        order.Status.Should().Be(OrderStatus.Cancelled);
    }

    [Theory]
    [InlineData(OrderStatus.Processing)]
    [InlineData(OrderStatus.Shipped)]
    [InlineData(OrderStatus.Delivered)]
    [InlineData(OrderStatus.Cancelled)]
    public void Cancel_FromNonCancellableStatus_ReturnsFailure(OrderStatus status)
    {
        var order = BuildOrder(status);

        var result = order.Cancel();

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(OrderErrors.CannotCancel);
        order.Status.Should().Be(status); // unchanged
    }

    // ---------- AttachPaymentIntent ----------
    [Fact]
    public void AttachPaymentIntent_FromPending_SetsPaymentIntentId()
    {
        var order = BuildOrder(OrderStatus.Pending);

        var result = order.AttachPaymentIntent("pi_123");

        result.IsSuccess.Should().BeTrue();
        order.PaymentIntentId.Should().Be("pi_123");
        order.Status.Should().Be(OrderStatus.Pending); // attaching doesn't change status
    }

    [Theory]
    [InlineData(OrderStatus.Processing)]
    [InlineData(OrderStatus.PaymentFailed)]
    [InlineData(OrderStatus.Cancelled)]
    public void AttachPaymentIntent_FromNonPending_ReturnsFailure(OrderStatus status)
    {
        var order = BuildOrder(status);

        var result = order.AttachPaymentIntent("pi_123");

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(OrderErrors.InvalidPaymentState);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void AttachPaymentIntent_WithEmptyId_ReturnsFailure(string paymentIntentId)
    {
        var order = BuildOrder(OrderStatus.Pending);

        var result = order.AttachPaymentIntent(paymentIntentId);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(OrderErrors.InvalidPaymentIntent);
    }

    // ---------- MarkAsPaid ----------
    [Fact]
    public void MarkAsPaid_FromPending_SetsProcessingAndPaidAtUtc()
    {
        var order = BuildOrder(OrderStatus.Pending);

        var result = order.MarkAsPaid("pi_123");

        result.IsSuccess.Should().BeTrue();
        order.Status.Should().Be(OrderStatus.Processing);
        order.PaymentIntentId.Should().Be("pi_123");
        order.PaidAtUtc.Should().NotBeNull();
    }

    [Fact]
    public void MarkAsPaid_FromPaymentFailed_Succeeds()
    {
        // THE bug fix from today: a declined card followed by a
        // successful retry on the same Stripe PaymentIntent must mark
        // the order paid. Before the fix, this returned
        // InvalidPaymentState and silently dropped a real payment.
        var order = BuildOrder(OrderStatus.PaymentFailed);

        var result = order.MarkAsPaid("pi_123");

        result.IsSuccess.Should().BeTrue();
        order.Status.Should().Be(OrderStatus.Processing);
        order.PaidAtUtc.Should().NotBeNull();
    }

    [Fact]
    public void MarkAsPaid_FromCancelled_ReturnsCannotPayCancelledError()
    {
        var order = BuildOrder(OrderStatus.Cancelled);

        var result = order.MarkAsPaid("pi_123");

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(OrderErrors.CannotPayCancelled);
    }

    [Theory]
    [InlineData(OrderStatus.Shipped)]
    [InlineData(OrderStatus.Delivered)]
    public void MarkAsPaid_FromLaterFulfillmentStatus_ReturnsInvalidPaymentState(OrderStatus status)
    {
        var order = BuildOrder(status);

        var result = order.MarkAsPaid("pi_123");

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(OrderErrors.InvalidPaymentState);
    }

    [Fact]
    public void MarkAsPaid_CalledTwiceWithSameIntent_IsIdempotent()
    {
        // Stripe's webhook delivery guarantee is "at least once," not
        // "exactly once" — a redelivered event must be a safe no-op,
        // not an error.
        var order = BuildOrder(OrderStatus.Pending);
        order.MarkAsPaid("pi_123");

        var secondResult = order.MarkAsPaid("pi_123");

        secondResult.IsSuccess.Should().BeTrue();
        order.Status.Should().Be(OrderStatus.Processing);
    }

    [Fact]
    public void MarkAsPaid_WithDifferentIntentThanAlreadyAttached_ReturnsMismatchError()
    {
        var order = BuildOrder(OrderStatus.Pending);
        order.AttachPaymentIntent("pi_first");

        var result = order.MarkAsPaid("pi_different");

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(OrderErrors.PaymentIntentMismatch);
        order.Status.Should().Be(OrderStatus.Pending); // unchanged
    }

    // ---------- MarkAsPaymentFailed ----------
    [Fact]
    public void MarkAsPaymentFailed_FromPending_SetsPaymentFailed()
    {
        var order = BuildOrder(OrderStatus.Pending);

        var result = order.MarkAsPaymentFailed("pi_123");

        result.IsSuccess.Should().BeTrue();
        order.Status.Should().Be(OrderStatus.PaymentFailed);
    }

    [Fact]
    public void MarkAsPaymentFailed_FromAlreadyProcessing_ReturnsFailureAndDoesNotOverwrite()
    {
        // Guards the out-of-order-webhook case: if the success event
        // is processed before a late failure event for the same
        // attempt arrives, the late failure must no-op instead of
        // knocking an already-paid order back to failed.
        var order = BuildOrder(OrderStatus.Pending);
        order.MarkAsPaid("pi_123");

        var result = order.MarkAsPaymentFailed("pi_123");

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(OrderErrors.InvalidPaymentState);
        order.Status.Should().Be(OrderStatus.Processing); // unchanged
    }

    [Fact]
    public void MarkAsPaymentFailed_WithMismatchedIntentId_ReturnsMismatchError()
    {
        var order = BuildOrder(OrderStatus.Pending);
        order.AttachPaymentIntent("pi_first");

        var result = order.MarkAsPaymentFailed("pi_different");

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(OrderErrors.PaymentIntentMismatch);
    }
}
