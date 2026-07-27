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
    public static IDisposable WithProductContext(Guid productId)
    {
        var context = LogContext.PushProperty("ProductId", productId);
           
        return context;
    }

    public static IDisposable WithBrandContext(Guid brandId)
    {
        var context = LogContext.PushProperty("BrandId", brandId);

        return context;
    }

    public static IDisposable WithTypeContext(Guid typeId)
    {
        var context = LogContext.PushProperty("TypeId", typeId);

        return context;
    }

    public static IDisposable WithInventoryContext(Guid? inventoryProductId)
    {
        var context = LogContext.PushProperty("InventoryProductId", inventoryProductId);
        
        return context;
    }

    public static IDisposable WithCartContext(Guid buyerId, Guid? guestBuyerId = null)
    {
        var buyer = LogContext.PushProperty("BuyerId", buyerId);

        if (guestBuyerId is Guid guest)
        {
            var guestContext = LogContext.PushProperty("GuestBuyerId", guest);
            return new DisposableCombiner(buyer, guestContext);
        }

        return buyer;
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
