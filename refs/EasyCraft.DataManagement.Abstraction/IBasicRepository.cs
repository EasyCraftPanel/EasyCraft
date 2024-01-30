using EasyCraft.DataManagement.Abstraction.Abilities;

namespace EasyCraft.DataManagement.Abstraction;

public interface IBasicRepository<TEntity> : 
    IRepositoryManyable<TEntity>,
    IPageableRepository<TEntity>
    where TEntity : class, IEntity
{
}