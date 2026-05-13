using Microsoft.EntityFrameworkCore;

namespace MS_Infrastructure.DataAccess.DISTS.Contexts
{
    public class DistDbContext : DbContext
    {
        public DistDbContext(DbContextOptions<DistDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("dist");

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(DistDbContext).Assembly);

            base.OnModelCreating(modelBuilder);
        }
    }
}
