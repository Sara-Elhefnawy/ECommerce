using Microsoft.Extensions.Caching.Hybrid;

namespace ECommerce.Infrastructure.Caching;

// HybridCache has no GetAsync() like Redis — only GetOrCreateAsync()
// i created this extension method to for GetAsync() to get from cache
//      without updating that key's LastAccessedUtc
public static class HybridCacheExtensions
{
    private static readonly HybridCacheEntryOptions ReadOnlyOptions = new()
    {
        // i don't want to write to the cache when reading
        Flags = HybridCacheEntryFlags.DisableLocalCacheWrite | HybridCacheEntryFlags.DisableDistributedCacheWrite
    };
    

    public static async ValueTask<T?> TryGetAsync<T>(
        this HybridCache cache,
        string key,
        CancellationToken ct = default) where T : class
    {
        var found = true;

        // i don't send factory as if i sent it then he will write, instead i discard the factory 
        // if key found in cache then he won't go to the discard '_'
        // if key not found in cache then he will go to the discard '_'
        var value = await cache.GetOrCreateAsync(
            key,                         // if found key in cache then freturn it
            _ =>                         
            {
                found = false;
                return ValueTask.FromResult<T?>(default);  // it returns null
            },
            ReadOnlyOptions,
            cancellationToken: ct);

        return found ? value : default;
    }
}
