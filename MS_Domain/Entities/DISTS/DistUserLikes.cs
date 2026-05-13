using MS_Application.DataTransferObjects.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace MS_Domain.Entities.DISTS
{
    [Table("DIST_USER_LIKES")]
    public class DistUserLikes : BaseModel
    {
        [Column("user_id")]
        public long UserId { get; set; }

        [Column("song_id")]
        public long SongId { get; set; }

        public DistSongs Song { get; set; } = null!;
    }
}