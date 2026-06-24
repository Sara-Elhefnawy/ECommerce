using ECommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace ECommerce.Infrastructure.Interceptors;

public class AuditInterceptor : SaveChangesInterceptor
{
    public AuditInterceptor() { }

    private void ApplyAudit(DbContext context)
    {
        if (context == null) return;

        var now = DateTimeOffset.UtcNow;

        // Use the generic constraint to pull only BaseEntity entries
        foreach (var entry in context.ChangeTracker.Entries<BaseEntity>())
        {

            switch (entry.State)
            {
                case EntityState.Added:
                    // Bypasses the private/protected modifier completely
                    var createdAtProp = entry.Property(e => e.CreatedAt);
                    break;

                case EntityState.Modified:
                    entry.Property(e => e.UpdatedAt).CurrentValue = now;
                    break;

                case EntityState.Deleted:
                    // Intercept physical deletion and turn it into a soft deletion
                    entry.State = EntityState.Modified;
                    entry.Property(e => e.IsDeleted).CurrentValue = true;
                    entry.Property(e => e.UpdatedAt).CurrentValue = now;
                    break;
            }
        }
    }

    public override InterceptionResult<int> SavingChanges(
        DbContextEventData eventData,
        InterceptionResult<int> result)
    {
        ApplyAudit(eventData.Context!);
        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken ct = default)
    {
        ApplyAudit(eventData.Context!);
        return base.SavingChangesAsync(eventData, result, ct);
    }
}
