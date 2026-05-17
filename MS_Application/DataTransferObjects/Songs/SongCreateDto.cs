using Microsoft.AspNetCore.Http;
using MS_Application.DataTransferObjects.Lyrics;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MS_Application.DataTransferObjects.Songs
{
    public class SongCreateDto
    {
        public string Title { get; set; }
        public DateOnly ReleaseDate { get; set; }
        public long AlbumId { get; set; }
        public IFormFile FileUrl { get; set; }
        public IFormFile ImgUrl { get; set; }
        public long ArtistId { get; set; }
        public short Type { get; set; }
        public string? Lyrics { get; set; }
    }

    public class SongLyricsCreateDto
    {
        public string Lyrics { get; set; }

        //public string? SyncedLyrics { get; set; }
        [JsonPropertyName("syncedLyrics")]
        public List<LyricsLineDto> SyncedLyrics { get; set; }
    }
}
