using Microsoft.EntityFrameworkCore;

namespace MS_Infrastructure.DataAccess.DISTS.Contexts
{
    public class DistDbContext : BaseDbContext
    {
        public DistDbContext(DbContextOptions<DistDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("dist");

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(DistDbContext).Assembly);

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
}
