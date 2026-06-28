using ECommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;

namespace ECommerce.Infrastructure.Interceptors;

public sealed class AuditInterceptor : SaveChangesInterceptor
{
    private readonly ILogger<AuditInterceptor>? _logger;

    public AuditInterceptor(ILogger<AuditInterceptor>? logger = null)
    {
        _logger = logger;
    }

    public void ApplyAudit(DbContext context)
    {
        if (context is null) return;

        var now = DateTimeOffset.UtcNow;
        var entries = context.ChangeTracker.Entries<BaseEntity>().ToList();

        // Only log in development (Debug level)
        _logger?.LogDebug("Applying audit to {Count} entities", entries.Count);

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
                    _logger?.LogDebug("Set CreatedAt for {Type} to {Now}",
                        entry.Entity.GetType().Name, now);
                    break;

                case EntityState.Modified:
                    entry.Property(nameof(BaseEntity.UpdatedAt)).CurrentValue = now;
                    _logger?.LogDebug("Set UpdatedAt for {Type} to {Now}",
                        entry.Entity.GetType().Name, now);
                    break;

                case EntityState.Deleted:
                    entry.State = EntityState.Modified;
                    entry.Property(nameof(BaseEntity.IsDeleted)).CurrentValue = true;
                    entry.Property(nameof(BaseEntity.UpdatedAt)).CurrentValue = now;
                    _logger?.LogDebug("Soft-deleted {Type} at {Now}",
                        entry.Entity.GetType().Name, now);
                    break;
            }
        }
    }

    public override InterceptionResult<int> SavingChanges(
        DbContextEventData eventData,
        InterceptionResult<int> result)
    {
        if (eventData.Context is not null)
            ApplyAudit(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        if (eventData.Context is not null)
            ApplyAudit(eventData.Context);
        return await base.SavingChangesAsync(eventData, result, cancellationToken);
    }
}
