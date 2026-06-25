using ECommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace ECommerce.Infrastructure.Interceptors;

public sealed class AuditInterceptor : SaveChangesInterceptor
{
    public void ApplyAudit(DbContext context)
    {
        if (context is null) return;

        var now = DateTimeOffset.UtcNow;

        // Use the generic constraint to pull only BaseEntity entries
        foreach (var entry in context.ChangeTracker.Entries<BaseEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    if (entry.Entity.Id == Guid.Empty)
                        // Bypasses the private/protected modifier completely
                        //      using:
                        //          entry.Property(nameof(BaseEntity.Id)).CurrentValue = Guid.NewGuid();
                        //      instead of accessing the entity object directly
                        //          entry.Entity.Id = Guid.NewGuid())
                        entry.Property(nameof(BaseEntity.Id)).CurrentValue = Guid.NewGuid();
                    
                    entry.Property(nameof(BaseEntity.CreatedAt)).CurrentValue = now;
                    break;

                case EntityState.Modified:
                    entry.Property(nameof(BaseEntity.UpdatedAt)).CurrentValue = now;
                    break;

                case EntityState.Deleted:
                    entry.State = EntityState.Modified;
                    entry.Property(nameof(BaseEntity.IsDeleted)).CurrentValue = true;
                    entry.Property(nameof(BaseEntity.UpdatedAt)).CurrentValue = now;
                    break;
            }
        }
    }
}
