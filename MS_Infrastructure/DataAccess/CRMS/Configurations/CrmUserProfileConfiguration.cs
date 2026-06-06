using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MS_Domain.Entities.CRMS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MS_Infrastructure.DataAccess.CRMS.Configurations
{
    public class CrmUserProfileConfiguration : IEntityTypeConfiguration<CrmUserProfile>
    {
        public void Configure(EntityTypeBuilder<CrmUserProfile> builder)
        {
        }
    }
}
