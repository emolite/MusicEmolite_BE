using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MS_Domain.Entities.CRMS;

namespace MS_Infrastructure.DataAccess.CRMS.Configurations
{
    public class CrmMessageConfiguration : IEntityTypeConfiguration<CrmMessage>
    {
        public void Configure(EntityTypeBuilder<CrmMessage> builder)
        {
            builder.HasIndex(x => new { x.SenderId, x.ReceiverId, x.CreatedAt });
            builder.HasIndex(x => new { x.ReceiverId, x.IsRead });
        }
    }
}
