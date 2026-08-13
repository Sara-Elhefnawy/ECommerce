using ECommerce.Domain.Entities.Errors;
using ECommerce.Domain.Results;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace ECommerce.API.Extensions;

public static class BuyerIdExtensions
{
    public const string HeaderName = "X-Buyer-Id";

    // Who is the current cart owner?
    public static ResultOfT<Guid> GetBuyerId(this HttpContext context)
    {
        // Authenticated shopper → buyer id comes from the JWT (NameIdentifier / sub).
        if (context.User.Identity?.IsAuthenticated == true)
        {
            var userIdValue = context.User.FindFirstValue(JwtRegisteredClaimNames.Sub);

            if (!Guid.TryParse(userIdValue, out var userId) || userId == Guid.Empty)
                return ResultOfT<Guid>.Failure(CartErrors.AuthenticatedBuyerIdMissing);

            return userId;
        }

        // Guest shopper → client-generated GUID via X-Buyer-Id header.
        if (!context.Request.Headers.TryGetValue(HeaderName, out var headerValue))
            return ResultOfT<Guid>.Failure(CartErrors.GuestBuyerIdRequired);

        if (!Guid.TryParse(headerValue, out var buyerId) || buyerId == Guid.Empty)
            return ResultOfT<Guid>.Failure(CartErrors.InvalidBuyerId);

        return buyerId;
    }

    // What guest cart should I merge?
    public static ResultOfT<Guid> GetBuyerIdHeader(this HttpContext context)
    {
        if (!context.Request.Headers.TryGetValue(HeaderName, out var headerValue))
            return ResultOfT<Guid>.Failure(CartErrors.GuestBuyerIdRequired);

        if (!Guid.TryParse(headerValue, out var buyerId) || buyerId == Guid.Empty)
            return ResultOfT<Guid>.Failure(CartErrors.InvalidBuyerId);

        return buyerId;
    }
}
