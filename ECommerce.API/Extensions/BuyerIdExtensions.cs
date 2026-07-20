using ECommerce.Domain.Common;
using ECommerce.Domain.Common.Errors;

namespace ECommerce.API.Extensions;

public static class BuyerIdExtensions
{
    public const string HeaderName = "X-Buyer-Id";

    public static ResultOfT<Guid> GetBuyerId(this HttpContext context)
        => GetBuyerId(context.Request.Headers);

    private static ResultOfT<Guid> GetBuyerId(IHeaderDictionary headers)
    {
        if (!headers.TryGetValue(HeaderName, out var headerValue))
            return ResultOfT<Guid>.Failure(CartErrors.GuestBuyerIdRequired);

        if (!Guid.TryParse(headerValue, out var buyerId) || buyerId == Guid.Empty)
            return ResultOfT<Guid>.Failure(CartErrors.InvalidBuyerId);

        // read from jwt

        return buyerId;
    }
}
