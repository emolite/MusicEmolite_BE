using MS_Application.DataTransferObjects.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MS_Domain.Entities.DISTS
{
    [Table("song_lyrics", Schema = "dist")]
    public class DistSongLyrics : BaseModel
    {
        [Column("song_id")]
        public long SongId { get; set; }

        [Column("content")]
        public string? Content { get; set; }

        [Column("synced_json", TypeName = "jsonb")]
        public string? SyncedJson { get; set; }

        [Column("is_official")]
        public bool IsOfficial { get; set; } = true;
    }
}
