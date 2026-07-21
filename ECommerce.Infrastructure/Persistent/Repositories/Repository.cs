using ECommerce.Domain.Abstractions.Repositories;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Specifications;
using ECommerce.Infrastructure.Persistent.Specifications;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Infrastructure.Persistent.Repositories;

public sealed class Repository<T>(ECommerceDbContext dbContext) : IRepository<T> where T : BaseEntity
{
    private readonly DbSet<T> _dbSet = dbContext.Set<T>();

    public async Task<T?> FirstOrDefaultAsync(ISpecification<T> specification, CancellationToken ct = default)
        => await SpecificationEvaluator.GetQuery(_dbSet, specification).FirstOrDefaultAsync(ct);

    public async Task<TResult?> FirstOrDefaultAsync<TResult>(
        ISpecification<T, TResult> specification,
        CancellationToken ct = default)
        => await SpecificationEvaluator.GetQuery(_dbSet, specification).FirstOrDefaultAsync(ct);

    public async Task<IReadOnlyList<T>> ListAsync(ISpecification<T> specification, CancellationToken ct = default)
    {
        return await SpecificationEvaluator.GetQuery(_dbSet, specification).ToListAsync(ct);
    }

    public async Task<IReadOnlyList<TResult>> ListAsync<TResult>(
        ISpecification<T, TResult> specification,
        CancellationToken ct = default)
    {
        return await SpecificationEvaluator.GetQuery(_dbSet, specification).ToListAsync(ct);
    }

    public async Task<int> CountAsync(ISpecification<T> specification, CancellationToken ct = default)
    {
        return await SpecificationEvaluator.GetCountQuery(_dbSet, specification).CountAsync(ct);
    }

    public async Task<bool> AnyAsync(ISpecification<T> specification, CancellationToken ct = default)
    {
        return await SpecificationEvaluator.GetCountQuery(_dbSet, specification).AnyAsync(ct);
    }

    public void Add(T entity)
        => _dbSet.Add(entity);

    public void Update(T entity)
        => _dbSet.Update(entity);

    public void SoftDelete(T entity)
        => entity.MarkDeleted();
}
