using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using EasyCraft.DataManagement.Abstraction.Abilities;

namespace EasyCraft.DataManagement.Abstraction;

public interface IRepository<TEntity> :
    ICountableRepository<TEntity>,
    IInsertableRepository<TEntity>,
    IReadableRepository<TEntity>,
    IRemovableRepository<TEntity>,
    IUpdateableRepository<TEntity>
    where TEntity : class, IEntity

{
}