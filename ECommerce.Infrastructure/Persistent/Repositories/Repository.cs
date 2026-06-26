using ECommerce.Domain.Abstractions.Repositories;
using ECommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Infrastructure.Persistent.Repositories;

public sealed class Repository<T>(ECommerceDbContext dbContext) : IRepository<T> where T : BaseEntity
{
    private readonly DbSet<T> _dbSet = dbContext.Set<T>();

    public async Task<IReadOnlyList<T>?> GetAllAsync(CancellationToken ct = default)
        => await _dbSet
        .AsNoTracking()
        .ToListAsync(ct);

    public async Task<T?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await _dbSet.FirstOrDefaultAsync(e => e.Id == id, ct);

    public void Add(T entity)
        => _dbSet.Add(entity);

    public void Update(T entity)
        => _dbSet.Update(entity);

    public void SoftDelete(T entity)
        => entity.MarkDeleted();
}
