using MS_Application.DataTransferObjects.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MS_Domain.Entities.DISTS
{
    [Table("song_albums", Schema = "dist")]
    public class DistSongAlbums : BaseModel
    {
        [Column("song_id")]
        public long SongId { get; set; }

        [Column("album_id")]
        public long AlbumId { get; set; }
    }
}
