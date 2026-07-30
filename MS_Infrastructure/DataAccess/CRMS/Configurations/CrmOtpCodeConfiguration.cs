using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MS_Domain.Entities.CRMS;

namespace MS_Infrastructure.DataAccess.CRMS.Configurations
{
    public class CrmOtpCodeConfiguration : IEntityTypeConfiguration<CrmOtpCode>
    {
        public void Configure(EntityTypeBuilder<CrmOtpCode> builder)
        {
            builder.HasIndex(x => new { x.Email, x.Purpose, x.CreatedAt });
        }
    }
}
