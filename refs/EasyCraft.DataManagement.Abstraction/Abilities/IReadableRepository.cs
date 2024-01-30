using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace EasyCraft.DataManagement.Abstraction.Abilities;

public interface IReadableRepository<TEntity> where TEntity : class
{
    public Task<TEntity?> FindAsync(Expression<Func<TEntity, bool>>? predicate, CancellationToken cancellationToken = default);
}

public interface IKeyedReadableRepository<TEntity, TKey> where TEntity : class
{
    public Task<TEntity?> FindAsync(TKey key, CancellationToken cancellationToken = default);
}

public interface IReadableManyRepository<TEntity>
{
    public Task<IAsyncEnumerable<TEntity>> GetAllAsync(CancellationToken cancellationToken = default);
}