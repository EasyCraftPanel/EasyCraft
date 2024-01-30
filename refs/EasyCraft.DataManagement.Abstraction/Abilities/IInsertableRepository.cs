using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace EasyCraft.DataManagement.Abstraction.Abilities;

public interface IInsertableRepository<TEntity> where TEntity : class
{
    public Task<TEntity> InsertAsync(TEntity entity, CancellationToken cancellationToken = default);
}

public interface IInsertableManyRepository<TEntity> where TEntity : class
{
    public Task<IEnumerable<TEntity>> InsertAsync(IEnumerable<TEntity> entity, CancellationToken cancellationToken = default);
}