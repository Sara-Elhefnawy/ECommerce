using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace ECommerce.API.Extensions;

public static class ClaimsPrincipalExtensions
{
    /// Raw lookup — null if unauthenticated, or if the sub claim is
    /// missing/malformed. Callers decide how to treat that.
    public static Guid? GetUserIdOrNull(this ClaimsPrincipal user)
    {
        if (user.Identity?.IsAuthenticated != true)
            return null;

        var value = user.FindFirstValue(JwtRegisteredClaimNames.Sub);

        return Guid.TryParse(value, out var id) ? id : null;
    }

    /// Display-only string for logging — collapses every "no usable id" case
    /// into "anonymous". Only use where the distinction doesn't matter.
    public static string GetUserIdOrAnonymous(this ClaimsPrincipal user) 
        => user.GetUserIdOrNull()?.ToString() ?? "anonymous";
}
