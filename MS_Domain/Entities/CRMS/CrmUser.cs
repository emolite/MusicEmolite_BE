

using MS_Application.DataTransferObjects.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace MS_Domain.Entities.CRMS
{
    [Table("users", Schema = "crm")]
    public class CrmUser : BaseModel
    {
        [Column("ref_code")]
        public string RefCode { get; set; }
        [Column("user_name")]
        public string Username { get; set; }
        [Column("email")]
        public string Email { get; set; }
        [Column("password_hash")]
        public string PasswordHash { get; set; }
        [Column("role_code")]
        public string RoleCode { get; set; }
        [Column("user_type")]
        public short UserType { get; set; }
        public CrmUserProfile Profile { get; set; }
    }
}
