using Microsoft.EntityFrameworkCore;

namespace MS_Infrastructure.DataAccess
{
    public class BaseDbContext : DbContext
    {
        public BaseDbContext(DbContextOptions options)
            : base(options)
        {
        }

        public override int SaveChanges()
        {
            NormalizeDateTimes();
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(
            CancellationToken cancellationToken = default)
        {
            NormalizeDateTimes();
            return base.SaveChangesAsync(cancellationToken);
        }

        private void NormalizeDateTimes()
        {
            var entries = ChangeTracker.Entries()
                .Where(e =>
                    e.State == EntityState.Added ||
                    e.State == EntityState.Modified);

            foreach (var entry in entries)
            {
                foreach (var property in entry.Properties)
                {
                    var type = property.Metadata.ClrType;

                    if (type == typeof(DateTime))
                    {
                        var value = (DateTime?)property.CurrentValue;

                        if (value.HasValue)
                        {
                            property.CurrentValue =
                                DateTime.SpecifyKind(
                                    value.Value,
                                    DateTimeKind.Unspecified);
                        }
                    }

                    if (type == typeof(DateTime?))
                    {
                        var value = (DateTime?)property.CurrentValue;

                        if (value.HasValue)
                        {
                            property.CurrentValue =
                                DateTime.SpecifyKind(
                                    value.Value,
                                    DateTimeKind.Unspecified);
                        }
                    }
                }
            }
        }
    }
}