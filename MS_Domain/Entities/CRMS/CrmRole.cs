using MS_Application.DataTransferObjects.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MS_Domain.Entities.CRMS
{
    [Table("CRM_ROLES")]
    public class CrmRole : BaseModel
    {
        [Column("role_code")]
        public string RoleCode { get; set; }
        [Column("role_name")]
        public string RoleName { get; set; }
    }
}
