using ECommerce.Domain.Entities;

namespace ECommerce.Domain.Abstractions.Repositories;

public interface IRepository<T> : IReadRepository<T> where T : BaseEntity
{
    void Add(T entity);

    void Update(T entity);

    void SoftDelete(T entity);

}
