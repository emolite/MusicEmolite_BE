using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MS_Domain.Entities.CRMS;

namespace MS_Infrastructure.DataAccess.CRMS.Configurations
{
    public class CrmUserConfiguration : IEntityTypeConfiguration<CrmUser>
    {
        public void Configure(EntityTypeBuilder<CrmUser> builder)
        {
        }
    }
}
