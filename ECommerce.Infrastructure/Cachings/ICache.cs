namespace ECommerce.Infrastructure.Cachings;

// is a marker interface for caching implementations (e.g., Redis, MemoryCache, etc.)
// any caching implementation should implement this interface to be recognized as a cache provider in the application.
public interface ICache<T> where T : class 
{
    // check if key exists in cache without updating that key's LastAccessedUtc
    // this method is cuz hybridcache only have GetOrCreateAsync, but not a method to check if key exists without updating LastAccessedUtc
    Task<T?> GetAsync(string cacheKey, CancellationToken ct = default);

    Task<T> GetOrCreateAsync(
        string cacheKey,             // if the cacheKey exists, return the cached value
        Func<CancellationToken, Task<T>> factory,  // of not in cache then get from DB using CancellationToken and store in cache
        CancellationToken ct = default);

    Task SetAsync(string cacheKey, T value, CancellationToken ct = default);

    Task RemoveAsync(string cacheKey, CancellationToken ct = default);
}
