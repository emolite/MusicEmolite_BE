using Microsoft.EntityFrameworkCore;

namespace MS_Infrastructure.DataAccess;

public class CrmDbContext : BaseDbContext
{
    public CrmDbContext(DbContextOptions<CrmDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("crm");

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CrmDbContext).Assembly);

        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            foreach (var property in entityType.GetProperties())
            {
                if (property.ClrType == typeof(DateTime) ||
                    property.ClrType == typeof(DateTime?))
                {
                    property.SetColumnType(
                        "timestamp without time zone");
                }
            }
        }

        base.OnModelCreating(modelBuilder);
    }
}
