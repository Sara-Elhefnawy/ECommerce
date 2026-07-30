using ECommerce.APP.Token;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ECommerce.Infrastructure.Token;

public sealed class JwtTokenGenerator(IOptions<JwtSettings> settings) : IJwtTokenGenerator
{
    // Captured once from IOptions<T>.Value —
    // avoids re-resolving the options object on every call to GenerateToken.
    private readonly JwtSettings _settings = settings.Value;

    public AccessTokenResult GenerateToken(Guid userId, string email, string? displayName, IEnumerable<string> roles)
    {
        var expiresAt = DateTimeOffset.UtcNow.AddMinutes(_settings.AccessTokenExpirationMinutes);

        // A "claim" is just a key/value statement about the user that gets embedded
        //      INSIDE the token itself (in the JWT payload).
        // Once the token is issued, anyone who validates it can read these claims
        //      WITHOUT hitting the database
        var claims = new List<Claim>
        {
            // "sub" = subject = who this token is about (the user id).
            new(JwtRegisteredClaimNames.Sub, userId.ToString()),
            new(JwtRegisteredClaimNames.Email, email)
        };

        if (!string.IsNullOrEmpty(displayName))
            claims.Add(new("display_name", displayName));

        // One Claim object per role, all sharing the type "role".
        // A user with 3 roles ends up with 3 separate claims in the token, e.g.
        // role=Admin, role=Manager, role=User — not one claim like "Admin,Manager,User".
        // ASP.NET Core's [Authorize(Roles = "Admin")] expects claims in this shape.
        claims.AddRange(roles.Select(role => new Claim ("role", role)));

        // "Credentials" here doesn't mean username/password — in JWT terms it means
        // "the key + algorithm used to sign the token",
        //      i.e. proof the token was issued by us and hasn't been tampered with.
        // SymmetricSecurityKey = the SAME secret string is used to both sign this token
        // now, and verify it later when a request comes in
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.Secret));

        // SigningCredentials bundles that key together with the algorithm to use (HMAC-SHA256).
        // This is what actually gets used to compute the token's signature — the third part
        // of the "header.payload.signature" string that makes up a JWT.
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        // JwtSecurityToken is just an in-memory representation/model of the token —
        // it doesn't produce the final string yet.
        var token = new JwtSecurityToken(
            issuer: _settings.Issuer,
            audience: _settings.Audience,
            claims: claims,
            expires: expiresAt.UtcDateTime,
            signingCredentials: credentials
            );


        // WriteToken serializes that in-memory model into the actual compact JWT string
        // you've seen before, e.g. "eyJhbGciOiJIUzI1NiIs...". This is the string that
        // actually gets sent to the client and later attached as "Authorization: Bearer <this>".
        var written = new JwtSecurityTokenHandler().WriteToken(token);

        return new AccessTokenResult(written, expiresAt);
    }
}
