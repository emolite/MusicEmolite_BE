namespace MS_Application.Repositories;

public interface IBaseRepository<T> where T : class
{
    Task<T?> GetByIdAsync(object id);
    Task<IReadOnlyList<T>> ListAsync();
    Task AddAsync(T entity);
    void Update(T entity);
    void Remove(T entity);
}