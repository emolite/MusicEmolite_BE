using MS_Application.DataTransferObjects.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace MS_Domain.Entities.CRMS
{
    [Table("friendships", Schema = "crm")]
    public class CrmFriendship : BaseModel
    {
        [Column("requester_id")]
        public long RequesterId { get; set; }

        [Column("addressee_id")]
        public long AddresseeId { get; set; }

        [Column("status")]
        public string Status { get; set; } = "PENDING";

        [Column("responded_at")]
        public DateTime? RespondedAt { get; set; }
    }
}
