using ECommerce.APP.Cachings;
using ECommerce.Domain.Entities;

namespace ECommerce.Infrastructure.Cachings;

public class CartRepository(ICache<Cart> cache) : ICartRepository
{
    public Task<Cart?> GetAsync(Guid buyerId, CancellationToken ct = default) 
        => cache.GetAsync(BuildCacheKey(buyerId), ct);

    public Task<Cart> GetOrCreateAsync(Guid buyerId, CancellationToken ct = default)
        => cache.GetOrCreateAsync(
            BuildCacheKey(buyerId),
            async cancellationToken =>     // if buyerId not found in cart then create it in cart?? what does it even mean?
            {          
                // if cart was saved in DB then he would call the DB here but it only save in cache 
                var cartCreatedResult = Cart.CreateEmpty(buyerId);

                if (cartCreatedResult.IsFailure)
                    throw new InvalidOperationException(cartCreatedResult?.Error?.Message);

                return cartCreatedResult.Value;
            },
            ct);

    public Task SaveAsync(Cart cart, CancellationToken ct = default)
        => cache.SetAsync(BuildCacheKey(cart.BuyerId), cart, ct);

    public Task DeleteAsync(Guid buyerId, CancellationToken ct = default)
        => cache.RemoveAsync(BuildCacheKey(buyerId), ct);

    private static string BuildCacheKey(Guid buyerId) => $"cart:{buyerId}";
}
