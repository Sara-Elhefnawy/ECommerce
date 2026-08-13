using ECommerce.Domain.Entities;
using ECommerce.Domain.Results;

namespace ECommerce.APP.Cachings.Carts;

public interface ICartRepository
{
    Task<ResultOfT<Cart?>> GetAsync(Guid buyerId, CancellationToken ct = default);

    Task<ResultOfT<Cart>> GetOrCreateAsync(Guid buyerId, CancellationToken ct = default);

    Task<Result> SaveAsync(Cart cart, CancellationToken ct = default);

    Task<Result> DeleteAsync(Guid buyerId, CancellationToken ct = default);
}
