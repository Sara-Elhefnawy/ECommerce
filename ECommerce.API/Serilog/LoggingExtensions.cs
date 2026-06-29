using Serilog.Context;

namespace ECommerce.API.Serilog;

// Helper methods that wrap LogContext.PushProperty() calls.
// Use these in endpoints or services to attach context to a group of log entries.
// They return IDisposable — always use with "using" so the context is cleaned up:
//
//   using (LoggingExtensions.WithProductContext(productId))
//   {
//       logger.LogInformation("doing something"); // ← ProductId will be on this log
//   } // ← ProductId is removed from context here
public static class LoggingExtensions
{
    public static IDisposable WithProductContext(Guid productId, string? productName = null)
    {
        var context = LogContext.PushProperty("ProductId", productId);
        if (!string.IsNullOrEmpty(productName))
        {
            var nameContext = LogContext.PushProperty("ProductName", productName);
            return new DisposableCombiner(context, nameContext);
        }
        return context;
    }

    public static IDisposable WithUserContext(Guid userId, string? userName = null)
    {
        var context = LogContext.PushProperty("UserId", userId);
        if (!string.IsNullOrEmpty(userName))
        {
            var nameContext = LogContext.PushProperty("UserName", userName);
            return new DisposableCombiner(context, nameContext);
        }
        return context;
    }

    public static IDisposable WithOrderContext(Guid orderId, string? orderNumber = null)
    {
        var context = LogContext.PushProperty("OrderId", orderId);
        if (!string.IsNullOrEmpty(orderNumber))
        {
            var numberContext = LogContext.PushProperty("OrderNumber", orderNumber);
            return new DisposableCombiner(context, numberContext);
        }
        return context;
    }

    public static IDisposable WithCorrelationId(string correlationId)
        => LogContext.PushProperty("CorrelationId", correlationId);

    // Combines two IDisposable objects so both get disposed with one "using" block.
    private class DisposableCombiner(IDisposable first, IDisposable second) : IDisposable
    {
        public void Dispose()
        {
            first.Dispose();
            second.Dispose();
        }
    }
}
