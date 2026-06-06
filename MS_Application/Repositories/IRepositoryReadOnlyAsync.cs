using System.Linq.Expressions;

namespace MS_Application.Repositories.Interfaces;

public interface IRepositoryReadOnlyAsync<T>
    where T : class
{
    Task<T?> FirstOrDefaultAsync(
        Expression<Func<T, bool>>? predicate = null,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
        Func<IQueryable<T>, IQueryable<T>>? include = null,
        bool disableTracking = true);

    IQueryable<T> QueryAll();

    IQueryable<T> QueryCondition(Expression<Func<T, bool>> expression);

    Task<bool> AnyAsync(Expression<Func<T, bool>> expression);
}