using MS_Application.DataTransferObjects.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace AMFC_Domain.Entities.Crms
{
    [Table("bank_user_info", Schema = "crm")]
    public class CrmBankUserInfo : BaseModel
    {
        [Column("ref_code")]
        public string RefCode { get; set; }

        [Column("user_profile_id")]
        public long UserProfileId { get; set; }

        [Column("bank_code")]
        public string BankCode { get; set; } = null!;

        [Column("bank_name")]
        public string BankName { get; set; } = null!;

        [Column("account_no")]
        public string AccountNo { get; set; } = null!;

        [Column("account_name")]
        public string AccountName { get; set; } = null!;

        [Column("vietqr_url")]
        public string? VietQrUrl { get; set; }

        [Column("qr_image_url")]
        public string? QrImageUrl { get; set; }

        [Column("trans_date")]
        public DateTime? TransDate { get; set; }
    }
}