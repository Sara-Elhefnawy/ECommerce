using Microsoft.AspNetCore.Identity;

namespace ECommerce.Infrastructure.Identity;

public sealed class ApplicationUser : IdentityUser<Guid>
{
    public ApplicationUser() => Id = Guid.NewGuid();

    public string? UserDisplayName { get; set; }

    // there is no ICollection<UserAddress> cuz i can't include it while joining
    // as UserAddress and ApplicationUser are in different DbContexts
}
