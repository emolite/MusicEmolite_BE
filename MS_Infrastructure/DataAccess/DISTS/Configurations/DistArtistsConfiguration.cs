using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MS_Domain.Entities.DISTS;
namespace MS_Infrastructure.DataAccess.DISTS.Configurations
{
    public class DistArtistsConfiguration : IEntityTypeConfiguration<DistArtists>
    {
        public void Configure(EntityTypeBuilder<DistArtists> builder)
        {
        }
    }
}
