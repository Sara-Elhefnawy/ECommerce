namespace ECommerce.APP.Token;

public sealed record AccessTokenResult(string AccessToken, DateTimeOffset ExpirationDate);
