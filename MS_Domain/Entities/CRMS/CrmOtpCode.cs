using MS_Application.DataTransferObjects.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace MS_Domain.Entities.CRMS
{
    [Table("otp_codes", Schema = "crm")]
    public class CrmOtpCode : BaseModel
    {
        [Column("email")]
        public string Email { get; set; } = null!;

        [Column("code")]
        public string Code { get; set; } = null!;

        [Column("purpose")]
        public string Purpose { get; set; } = null!;

        [Column("expired_at")]
        public DateTime ExpiredAt { get; set; }

        [Column("is_used")]
        public bool IsUsed { get; set; } = false;

        [Column("attempt_count")]
        public int AttemptCount { get; set; } = 0;
    }
}
