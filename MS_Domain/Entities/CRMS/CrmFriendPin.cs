using MS_Application.DataTransferObjects.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace MS_Domain.Entities.CRMS
{
    [Table("friend_pins", Schema = "crm")]
    public class CrmFriendPin : BaseModel
    {
        [Column("user_id")]
        public long UserId { get; set; }

        [Column("friend_user_id")]
        public long FriendUserId { get; set; }
    }
}
