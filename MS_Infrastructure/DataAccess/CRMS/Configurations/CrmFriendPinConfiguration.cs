using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MS_Domain.Entities.CRMS;

namespace MS_Infrastructure.DataAccess.CRMS.Configurations
{
    public class CrmFriendPinConfiguration : IEntityTypeConfiguration<CrmFriendPin>
    {
        public void Configure(EntityTypeBuilder<CrmFriendPin> builder)
        {
            builder.HasIndex(x => new { x.UserId, x.FriendUserId });
        }
    }
}
