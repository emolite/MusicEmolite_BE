using Microsoft.EntityFrameworkCore;
using MS_Application.Repositories.Interfaces;
using System.Linq.Expressions;

namespace MS_Infrastructure.Repositories;

public class RepositoryAsync<T>
    : BaseRepository<T>, IRepositoryAsync<T>
    where T : class
{
    public RepositoryAsync(DbContext context)
        : base(context) { }

    public Task AddAsync(T entity)
        => DbSet.AddAsync(entity).AsTask();

    public Task AddAsync(IEnumerable<T> entities)
        => DbSet.AddRangeAsync(entities);

    public Task UpdateAsync(T entity)
    {
        DbSet.Update(entity);
        return Task.CompletedTask;
    }

    public Task UpdateAsync(IEnumerable<T> entities)
    {
        DbSet.UpdateRange(entities);
        return Task.CompletedTask;
    }

    public async Task DeleteAsync(object id)
    {
        var entity = await DbSet.FindAsync(id);
        if (entity != null)
            DbSet.Remove(entity);
    }

    public Task DeleteAsync(T entity)
    {
        DbSet.Remove(entity);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(IEnumerable<T> entities)
    {
        DbSet.RemoveRange(entities);
        return Task.CompletedTask;
    }

    public Task<T?> FindByIdAsync(params object[] keyValues)
        => DbSet.FindAsync(keyValues).AsTask();

    public IQueryable<T> QueryCondition(Expression<Func<T, bool>> expression, bool asNoTracking = false)
        => asNoTracking ? DbSet.Where(expression).AsNoTracking() : DbSet.Where(expression);

    public IQueryable<T> QueryAll(bool asNoTracking = false)
    {
        return asNoTracking
            ? DbSet.AsNoTracking()
            : DbSet;
    }
}