using ECommerce.APP.Identity;
using Microsoft.AspNetCore.Http;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace ECommerce.Infrastructure.Identity;

// IHttpContextAccessor is used to help infratsructure know the current user
public sealed class CurrentUserService(IHttpContextAccessor httpContext) : ICurrentUserService
{
    // will get true if user is logged in and JWT is correct
    public bool IsAuthenticated => httpContext.HttpContext?.User?.Identity?.IsAuthenticated ?? false;

    public Guid? UserId
    {
        get
        {
            // JwtRegisteredClaimNames.Sub should match in JwtTokenGenerator
            var user = httpContext.HttpContext.User.FindFirstValue(JwtRegisteredClaimNames.Sub);

            return Guid.TryParse(user, out var userId) ? userId : null;
        }
    }

    public string? Email => httpContext.HttpContext?.User?.FindFirstValue(JwtRegisteredClaimNames.Email)
        ?? httpContext.HttpContext?.User?.FindFirstValue("email");
}
