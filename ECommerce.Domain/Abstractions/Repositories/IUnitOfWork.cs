using ECommerce.Domain.Entities;

namespace ECommerce.Domain.Abstractions.Repositories;

public interface IUnitOfWork
{
    IRepository<T> Repository<T>() where T : BaseEntity;

    Task<int> SaveChangesAsync(CancellationToken ct = default);
}
