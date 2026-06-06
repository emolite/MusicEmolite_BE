using AMFC_Domain.Entities.Crms;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MS_Infrastructure.DataAccess.CRMS.Configurations
{
    public class CrmBankUserInfoConfiguration : IEntityTypeConfiguration<CrmBankUserInfo>
    {
        public void Configure(EntityTypeBuilder<CrmBankUserInfo> builder)
        {
        }
    }
}
