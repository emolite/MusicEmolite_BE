using MS_Application.DataTransferObjects.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MS_Domain.Entities.DISTS
{
    [Table("albums", Schema = "dist")]
    public class DistAlbums : BaseModel
    {
        [Column("title")]
        public string Title { get; set; }
        [Column("release_date")]
        public DateTime ReleaseDate { get; set; }
        [Column("artist_id")]
        public long? ArtistId { get; set; }
        [Column("album_type")]
        public byte AlbumType { get; set; }

        [Column("uri")]
        public string? Uri { get; set; }

        public DistArtists Artist { get; set; }
    }
}
