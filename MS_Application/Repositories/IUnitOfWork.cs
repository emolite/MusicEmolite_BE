namespace MS_Application.Repositories.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IRepositoryAsync<TEntity> GetRepositoryAsync<TEntity>()
        where TEntity : class;

    IRepositoryReadOnlyAsync<TEntity> GetRepositoryReadOnlyAsync<TEntity>()
        where TEntity : class;

    Task<int> SaveChangesAsync();


}

public interface ICrmUnitOfWork : IUnitOfWork { }
public interface IDistUnitOfWork : IUnitOfWork { }