using MS_Application.DataTransferObjects.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace MS_Domain.Entities.CRMS
{
    [Table("messages", Schema = "crm")]
    public class CrmMessage : BaseModel
    {
        [Column("sender_id")]
        public long SenderId { get; set; }

        [Column("receiver_id")]
        public long ReceiverId { get; set; }

        [Column("content")]
        public string? Content { get; set; }

        [Column("image_public_id")]
        public string? ImagePublicId { get; set; }

        [Column("is_read")]
        public bool IsRead { get; set; } = false;

        [Column("read_at")]
        public DateTime? ReadAt { get; set; }

        [Column("reply_to_message_id")]
        public long? ReplyToMessageId { get; set; }

        [Column("forwarded_from_message_id")]
        public long? ForwardedFromMessageId { get; set; }
    }
}
