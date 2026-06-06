using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MS_Application.DataTransferObjects.Lyrics
{
    public class PublishLyricsRequestDto
    {
        public string TrackName { get; set; } = string.Empty;
        public string ArtistName { get; set; } = string.Empty;
        public string AlbumName { get; set; } = string.Empty;
        public int Duration { get; set; }

        public string PlainLyrics { get; set; } = string.Empty;
        public string SyncedLyrics { get; set; } = string.Empty;
    }
}
