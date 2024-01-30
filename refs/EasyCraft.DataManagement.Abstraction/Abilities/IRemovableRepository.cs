using System;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace EasyCraft.DataManagement.Abstraction.Abilities;

public interface IRemovableRepository<TEntity> where TEntity : class
{
    public Task<bool> RemoveAsync(TEntity entity, CancellationToken cancellationToken = default);
}

public interface IKeyedRemovableRepository<TEntity, TKey> where TEntity : class
{
    public Task<bool> RemoveAsync(TKey key, CancellationToken cancellationToken = default);
}

public interface IRemovableManyRepository<TEntity>
{
    public Task<int> RemoveAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);
}