using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using EasyCraft.DataManagement.Abstraction;
using Microsoft.EntityFrameworkCore;

namespace EasyCraft.DataManagement;

public class EfCoreRepository<TEntity> : IBasicRepository<TEntity> where TEntity : class, IEntity
{

    private readonly DbSet<TEntity> _dbSet;
    private readonly DbContext _dbContext;
    public EfCoreRepository(DbContext dbContext)
    {
        _dbContext = dbContext;
        _dbSet = dbContext.Set<TEntity>();
    }
    
    public async Task<int> CountAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet.CountAsync(cancellationToken: cancellationToken);
    }

    public async Task<TEntity> InsertAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        var tracking = await _dbSet.AddAsync(entity, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return tracking.Entity;
    }

    public async Task<TEntity?> FindAsync(Expression<Func<TEntity, bool>>? predicate, CancellationToken cancellationToken = default)
    {
        if (predicate is not null)
            return await _dbSet.FirstAsync(predicate, cancellationToken);
        return await _dbSet.FirstAsync(cancellationToken);
    }


    public async Task<bool> RemoveAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        var tracking = _dbSet.Remove(entity);
        var affectedRows = await _dbContext.SaveChangesAsync(cancellationToken);
        return tracking.State == EntityState.Deleted && affectedRows > 0;
    }

    public async Task<TEntity> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        var tracking = _dbSet.Update(entity);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return tracking.Entity;
    }

    public async Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await _dbSet.CountAsync(predicate, cancellationToken);
    }

    public async Task<IEnumerable<TEntity>> InsertAsync(IEnumerable<TEntity> entity, CancellationToken cancellationToken = default)
    {
        var entities = entity.ToList();
        await _dbSet.AddRangeAsync(entities, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return entities;
    }

    public Task<IAsyncEnumerable<TEntity>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_dbSet.AsAsyncEnumerable());
    }

    public async Task<int> RemoveAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
    {
        var entities = await _dbSet.Where(predicate).ToListAsync(cancellationToken);
        _dbSet.RemoveRange(entities);
        return await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<IEnumerable<TEntity>> UpdateAsync(IEnumerable<TEntity> entity, CancellationToken cancellationToken = default)
    {
        var entities = entity.ToList();
        _dbSet.UpdateRange(entities);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return entities;
    }

    public async Task<List<TEntity>> GetPageAsync(int skipCount, int pageSize, Expression<Func<TEntity, int>>? sorting, CancellationToken cancellationToken)
    {
        if (sorting is not null)
            return await _dbSet.OrderBy(sorting).Skip(skipCount).Take(pageSize).ToListAsync(cancellationToken: cancellationToken);
        return await _dbSet.Skip(skipCount).Take(pageSize).ToListAsync(cancellationToken: cancellationToken);
    }
}