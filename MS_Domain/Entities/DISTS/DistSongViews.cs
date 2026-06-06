using MS_Application.DataTransferObjects.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace MS_Domain.Entities.DISTS
{
    [Table("song_views", Schema = "dist")]
    public class DistSongViews : BaseModel
    {
        [Column("user_id")]
        public long? UserId { get; set; }

        [Column("song_id")]
        public long SongId { get; set; }

        [Column("ip_address")]
        public string? IpAddress { get; set; }

        [Column("viewed_at")]
        public DateTime ViewedAt { get; set; }

        [Column("duration_seconds")]
        public int? DurationSeconds { get; set; }

        public DistSongs Song { get; set; } = null!;
    }
}