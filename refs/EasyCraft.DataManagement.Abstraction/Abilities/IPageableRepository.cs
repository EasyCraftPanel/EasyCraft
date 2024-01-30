using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace EasyCraft.DataManagement.Abstraction.Abilities;

public interface IPageableRepository<TEntity> where TEntity : class
{
    public Task<List<TEntity>> GetPageAsync(int skipCount, int pageSize, Expression<Func<TEntity, int>>? sorting, CancellationToken cancellationToken = default);
}