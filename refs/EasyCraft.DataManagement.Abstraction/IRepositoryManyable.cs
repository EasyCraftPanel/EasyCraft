using EasyCraft.DataManagement.Abstraction.Abilities;

namespace EasyCraft.DataManagement.Abstraction;

public interface IRepositoryManyable<TEntity> : IRepository<TEntity>,
    ICountableManyRepository<TEntity>,
    IInsertableManyRepository<TEntity>,
    IReadableManyRepository<TEntity>,
    IRemovableManyRepository<TEntity>,
    IUpdateableManyRepository<TEntity>
    where TEntity : class, IEntity
{
    
}