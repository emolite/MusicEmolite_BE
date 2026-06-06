using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MS_Domain.Entities.DISTS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MS_Infrastructure.DataAccess.DISTS.Configurations
{
    public class DistSongAlbumsConfiguration : IEntityTypeConfiguration<DistSongAlbums>
    {
        public void Configure(EntityTypeBuilder<DistSongAlbums> builder)
        {
        }
    }
}
