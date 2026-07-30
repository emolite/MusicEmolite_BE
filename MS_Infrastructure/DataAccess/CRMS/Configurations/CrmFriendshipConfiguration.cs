using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MS_Domain.Entities.CRMS;

namespace MS_Infrastructure.DataAccess.CRMS.Configurations
{
    public class CrmFriendshipConfiguration : IEntityTypeConfiguration<CrmFriendship>
    {
        public void Configure(EntityTypeBuilder<CrmFriendship> builder)
        {
            builder.HasIndex(x => new { x.RequesterId, x.AddresseeId });
            builder.HasIndex(x => new { x.AddresseeId, x.Status });
        }
    }
}
