using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace EasyCraft.DataManagement.Abstraction.Abilities;


public interface IQueryableRepository<TEntity> where TEntity : class
{
    public Task<IQueryable<TEntity>> GetQueryable(CancellationToken cancellationToken = default);
}