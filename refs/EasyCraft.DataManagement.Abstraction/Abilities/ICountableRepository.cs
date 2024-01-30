using System;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace EasyCraft.DataManagement.Abstraction.Abilities;

public interface ICountableRepository<TEntity> where TEntity : class
{
    public Task<int> CountAsync(CancellationToken cancellationToken = default);
}

public interface ICountableManyRepository<TEntity> where TEntity : class
{
    public Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default);
}