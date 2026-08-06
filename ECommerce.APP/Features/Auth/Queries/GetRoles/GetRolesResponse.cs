namespace ECommerce.APP.Features.Auth.Queries.GetRoles;

public sealed record GetRolesResponse(
    Guid UserId,
    string Email,
    string? UserDisplayName,
    IEnumerable<string> Roles
    );
