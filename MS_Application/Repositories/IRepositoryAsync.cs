using System.Linq.Expressions;

namespace MS_Application.Repositories.Interfaces;

public interface IRepositoryAsync<T>
    where T : class
{
    Task AddAsync(T entity);
    Task AddAsync(IEnumerable<T> entities);

    Task UpdateAsync(T entity);
    Task UpdateAsync(IEnumerable<T> entities);

    Task DeleteAsync(object id);
    Task DeleteAsync(T entity);
    Task DeleteAsync(IEnumerable<T> entities);

    Task<T?> FindByIdAsync(params object[] keyValues);

    IQueryable<T> QueryAll(bool asNoTracking = false);

    IQueryable<T> QueryCondition(Expression<Func<T, bool>> expression, bool asNoTracking = false);
}