using EasyCraft.DataManagement.Abstraction.Abilities;

namespace EasyCraft.DataManagement.Abstraction;

public interface IKeyedRepository<TEntity, TKey> : IRepository<TEntity>,
    IKeyedReadableRepository<TEntity, TKey>,
    IKeyedRemovableRepository<TEntity, TKey>
    where TEntity : class, IEntity
{
}