using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MS_Domain.Entities.DISTS;

namespace MS_Infrastructure.DataAccess.DISTS.Configurations
{
    public class DistUserLikesConfiguration : IEntityTypeConfiguration<DistUserLikes>
    {
        public void Configure(EntityTypeBuilder<DistUserLikes> builder)
        {
        }
    }
}
