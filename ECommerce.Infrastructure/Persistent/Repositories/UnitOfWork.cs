using ECommerce.Domain.Abstractions.Repositories;
using ECommerce.Domain.Entities;
using System.Collections.Concurrent;

namespace ECommerce.Infrastructure.Persistent.Repositories;

public class UnitOfWork(ECommerceDbContext dbContext) : IUnitOfWork
{
    // key                 value
    // typeof(Product)    IRepo<Product>
    private readonly ConcurrentDictionary<Type, Object> _repos = new();

    public IRepository<T> Repository<T>() where T : BaseEntity
    {
        var type = typeof(T);

        if(_repos.TryGetValue(type, out var repo))
            return (IRepository<T>)repo;

        var newRepos = new Repository<T>(dbContext);

        _repos.TryAdd(type, newRepos);

        return newRepos; 
    }

    // What if i want productrepo? or want a specific repo?
    // problem is this method gets generic only
    // solution is create another method with another casting!!
    // if specific repos are alot then this way is not good implementation
    // when is this code good example and when is it bad example?

    public Task<int> SaveChangesAsync(CancellationToken ct = default)
        => dbContext.SaveChangesAsync(ct);
}
