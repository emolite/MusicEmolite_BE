using Microsoft.EntityFrameworkCore;

public abstract class BaseRepository<T>
    where T : class
{
    protected BaseRepository(DbContext context)
    {
        DbContext = context;
        DbSet = DbContext.Set<T>();
    }

    protected DbContext DbContext { get; }
    protected DbSet<T> DbSet { get; }
}