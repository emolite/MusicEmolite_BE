using MS_Application.DataTransferObjects.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace MS_Domain.Entities.CRMS
{
    [Table("user_sessions", Schema = "crm")]
    public class CrmUserSession : BaseModel
    {
        [Column("user_id")]
        public long UserId { get; set; }

        [Column("refresh_token")]
        public string RefreshToken { get; set; } = null!;

        [Column("ip_address")]
        public string? IpAddress { get; set; }

        [Column("user_agent")]
        public string? UserAgent { get; set; }

        [Column("device_name")]
        public string? DeviceName { get; set; }

        [Column("is_verified")]
        public bool IsVerified { get; set; }

        [Column("last_access_at")]
        public DateTime? LastAccessAt { get; set; }

        [Column("expired_at")]
        public DateTime? ExpiredAt { get; set; }

        [ForeignKey(nameof(UserId))]
        public CrmUser User { get; set; } = null!;
    }
}