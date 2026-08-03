namespace ECommerce.API.Endpoints.V1.Test;

public sealed record GenerateTestJwtRequest(
    Guid UserId, 
    string Email, 
    string? UserDisplayName, 
    string[] Roles);
