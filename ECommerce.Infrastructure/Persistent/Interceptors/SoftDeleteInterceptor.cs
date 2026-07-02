using ECommerce.Domain.Abstractions.Interceptors;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;

namespace ECommerce.Infrastructure.Persistent.Interceptors;

public class SoftDeleteInterceptor : SaveChangesInterceptor, ISoftDeleteInterceptor
{
    private readonly ILogger<SoftDeleteInterceptor>? _logger;

    public SoftDeleteInterceptor(ILogger<SoftDeleteInterceptor>? logger = null)
    {
        _logger = logger;
    }

    public void ApplySoftDelete(DbContext context)
    {
        var deletedEntries = context.ChangeTracker.Entries()
            .Where(e => e.State == EntityState.Deleted)
            .ToList();

        // Only log in development (Debug level)
        _logger?.LogDebug("Applying soft delete to {Count} entities", deletedEntries.Count);

        foreach (var entry in deletedEntries)
        {
            var entityType = entry.Entity.GetType();
            var isDeletedProp = entityType.GetProperty("IsDeleted");

            if (isDeletedProp is null)
            {
                // Owned/dependent entity (e.g. Address) with no soft-delete concept of its own.
                // It was cascaded into Deleted state along with its owner — reset it so EF
                // doesn't try to null out its table-split columns.
                entry.State = EntityState.Unchanged;
                continue;
            }

            entry.State = EntityState.Modified;

            isDeletedProp.SetValue(entry.Entity, true);

            var deletedAtProp = entityType.GetProperty("DeletedAt");
            if (deletedAtProp != null)
            {
                deletedAtProp.SetValue(entry.Entity, DateTime.UtcNow);
            }

            foreach (var property in entry.Properties)
            {
                if (property.Metadata.GetAfterSaveBehavior() != Microsoft.EntityFrameworkCore.Metadata.PropertySaveBehavior.Save)
                    continue;

                property.IsModified = true;
            }
        }
    }

    public override InterceptionResult<int> SavingChanges(
        DbContextEventData eventData,
        InterceptionResult<int> result)
    {
        ApplySoftDelete(eventData.Context!);
        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        ApplySoftDelete(eventData.Context!);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }
}
