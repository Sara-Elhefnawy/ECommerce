using ECommerce.APP.Cachings;
using ECommerce.APP.Cachings.Carts;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Results;

namespace ECommerce.Infrastructure.Cachings.Carts;

public class CartRepository(ICache<Cart> cache) : ICartRepository
{
    public Task<ResultOfT<Cart?>> GetAsync(Guid buyerId, CancellationToken ct = default) 
        => cache.GetAsync(BuildCacheKey(buyerId), ct);

    public async Task<ResultOfT<Cart>> GetOrCreateAsync(Guid buyerId, CancellationToken ct = default)
    {
        var existing = await cache.GetAsync(BuildCacheKey(buyerId), ct);

        if (existing.IsFailure)
            return existing.Error!;

        if (existing.Value is not null)
            return existing.Value;

        var created = Cart.CreateEmpty(buyerId);

        if (created.IsFailure)
            return created.Error!;

        var saveResult = await cache.SetAsync(BuildCacheKey(buyerId), created.Value, ct);

        if (saveResult.IsFailure)
            return saveResult.Error!;

        return created.Value;
    }

    public Task<Result> SaveAsync(Cart cart, CancellationToken ct = default)
        => cache.SetAsync(BuildCacheKey(cart.BuyerId), cart, ct);

    public Task<Result> DeleteAsync(Guid buyerId, CancellationToken ct = default)
        => cache.RemoveAsync(BuildCacheKey(buyerId), ct);

    private static string BuildCacheKey(Guid buyerId) => $"cart:{buyerId}";
}
