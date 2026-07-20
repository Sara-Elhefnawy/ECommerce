using ECommerce.Domain.Entities;

namespace ECommerce.APP.Cachings;

public interface ICartRepository
{
    Task<Cart?> GetAsync(Guid buyerId, CancellationToken ct = default);

    Task<Cart> GetOrCreateAsync(Guid buyerId, CancellationToken ct = default);

    Task SaveAsync(Cart cart, CancellationToken ct = default);

    Task DeleteAsync(Guid buyerId, CancellationToken ct = default);
}
