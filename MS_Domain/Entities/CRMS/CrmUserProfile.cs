using Microsoft.AspNetCore.Http;
using MS_Application.DataTransferObjects.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MS_Domain.Entities.CRMS
{
    [Table("CRM_USER_PROFILES")]
    public class CrmUserProfile : BaseModel
    {
        [Column("ref_code")]
        public string RefCode { get; set; }

        [Column("user_id")]
        public long UserId { get; set; }

        [Column("full_name")]
        public string FullName { get; set; }

        [Column("phone")]
        public string? Phone { get; set; }

        [Column("date_of_birth")]
        public DateTime? DateOfBirth { get; set; }

        [Column("gender")]
        public string? Gender { get; set; }

        [Column("bio")]
        public string? Bio { get; set; }

        [Column("uri")]
        public string? Uri { get; set; }

        [ForeignKey("UserId")]
        public CrmUser User { get; set; }
    }
}
