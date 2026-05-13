using Microsoft.EntityFrameworkCore;
using MS_Application.Repositories.Interfaces;
using System.Linq.Expressions;

namespace MS_Infrastructure.Repositories;

public class RepositoryReadOnlyAsync<T>
    : BaseRepository<T>, IRepositoryReadOnlyAsync<T>
    where T : class
{
    public RepositoryReadOnlyAsync(DbContext context)
        : base(context) { }

    public IQueryable<T> QueryAll()
        => DbSet.AsNoTracking();

    public IQueryable<T> QueryCondition(Expression<Func<T, bool>> expression)
        => DbSet.Where(expression).AsNoTracking();

    public Task<bool> AnyAsync(Expression<Func<T, bool>> expression)
        => DbSet.AnyAsync(expression);

    public async Task<T?> FirstOrDefaultAsync(
        Expression<Func<T, bool>>? predicate = null,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
        Func<IQueryable<T>, IQueryable<T>>? include = null,
        bool disableTracking = true)
    {
        IQueryable<T> query = DbSet;

        if (disableTracking)
            query = query.AsNoTracking();

        if (include != null)
            query = include(query);

        if (predicate != null)
            query = query.Where(predicate);

        if (orderBy != null)
            return await orderBy(query).FirstOrDefaultAsync();

        return await query.FirstOrDefaultAsync();
    }
}