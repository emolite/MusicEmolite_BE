using MS_Application.DataTransferObjects.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace MS_Domain.Entities.DISTS
{
    [Table("song_histories", Schema = "dist")]
    public class DistSongHistories : BaseModel
    {
        [Column("user_id")]
        public long UserId { get; set; }

        [Column("song_id")]
        public long SongId { get; set; }

        [Column("played_at")]
        public DateTime PlayedAt { get; set; }

        public DistSongs Song { get; set; } = null!;
    }
}