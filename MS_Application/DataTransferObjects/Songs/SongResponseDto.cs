using Microsoft.AspNetCore.Http;
using MS_Application.DataTransferObjects.Base;
using MS_Application.DataTransferObjects.Lyrics;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MS_Application.DataTransferObjects.Songs
{
    public class SongResponseDto : BaseDto
    {
        public string Title { get; set; }
        public int Duration { get; set; }
        public string? FileUrl { get; set; }
        public string? ImgUrl { get; set; }
        public DateOnly ReleaseDate { get; set; }
        public long? AlbumId { get; set; }
        public string ArtistName { get; set; }
        public bool IsLiked { get; set; }
        public string TypeSong { get; set; }
        public string? Lyrics { get; set; }
        public long? Views { get; set; }
        public long? Likes {  get; set; }
        public long PlayCount { get; set; } = 0;
        public string? YoutubeVideoId { get; set; }
        public short? SourceType { get; set; }
        public List<long> AlbumIds { get; set; } = new();
        public List<LyricsLineDto>? SyncedLyrics { get; set; }
    }
}
