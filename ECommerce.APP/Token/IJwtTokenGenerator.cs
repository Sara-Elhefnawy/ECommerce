namespace ECommerce.APP.Token;

public interface IJwtTokenGenerator
{
    AccessTokenResult GenerateToken(Guid userId, string email, string? displayName, IEnumerable<string> roles);
}
