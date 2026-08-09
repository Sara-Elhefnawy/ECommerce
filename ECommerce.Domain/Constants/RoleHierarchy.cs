namespace ECommerce.Domain.Constants;

public static class RoleHierarchy
{
    // Keeps role inheritance as data instead of scattering if/else logic
    // across IdentityService, seeders, or anywhere else roles get assigned.
    public static readonly IReadOnlyDictionary<string, string[]> Inherits =
        new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase)
        {
            [Roles.SuperAdmin] = [Roles.SuperAdmin, Roles.Admin, Roles.Manager, Roles.User],
            [Roles.Admin] = [Roles.Admin, Roles.Manager, Roles.User],
            [Roles.Manager] = [Roles.Manager, Roles.User],
            [Roles.User] = [Roles.User]
        };

    // Reverse of Inherits: for a given role, every role that DEPENDS on it
    // (i.e. would become an inconsistent, contradictory state if this role
    // were removed while they kept theirs). Computed once from Inherits so
    // the two can never drift out of sync with each other.
    public static readonly IReadOnlyDictionary<string, string[]> Dependents =
        Inherits.Keys.ToDictionary(
            role => role,
            role => Inherits.Where(kv => kv.Value.Contains(role, StringComparer.OrdinalIgnoreCase))
                             .Select(kv => kv.Key)
                             .ToArray(),
            StringComparer.OrdinalIgnoreCase);
}
