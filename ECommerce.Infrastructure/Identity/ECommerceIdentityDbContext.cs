using ECommerce.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Infrastructure.Identity;

public sealed class ECommerceIdentityDbContext(DbContextOptions<ECommerceIdentityDbContext> options)
    : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>(options)
{
    public DbSet<RefreshToken> RefreshTokens { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        // Very Important as it's responsible for creating the Base Identity Tables containing their configuratioins
        base.OnModelCreating(builder);

        builder.ApplyConfigurationsFromAssembly(
            typeof(ECommerceIdentityDbContext).Assembly,
            type => type.Namespace == "ECommerce.Infrastructure.Identity");
    }
}
