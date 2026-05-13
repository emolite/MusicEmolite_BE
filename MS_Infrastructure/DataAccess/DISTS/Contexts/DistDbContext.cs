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
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(DistDbContext).Assembly);
        }
    }
}
