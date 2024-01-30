using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace EasyCraft.DataManagement.Abstraction.Abilities;

public interface IUpdateableRepository<TEntity> where TEntity : class
{
    public Task<TEntity> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default);
}

public interface IUpdateableManyRepository<TEntity> where TEntity : class
{
    public Task<IEnumerable<TEntity>> UpdateAsync(IEnumerable<TEntity> entity, CancellationToken cancellationToken = default);
}