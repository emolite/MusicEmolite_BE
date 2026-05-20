using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MS_Application.DataTransferObjects.Lyrics
{
    public class LyricsResponseDto
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Title { get; set; } = string.Empty;

        public string Artist { get; set; } = string.Empty;

        public string Album { get; set; } = string.Empty;

        public decimal Duration { get; set; }

        public bool Instrumental { get; set; }

        public string Lyrics { get; set; } = string.Empty;

        public string RawSyncedLyrics { get; set; } = string.Empty;

        public List<LyricsLineDto> SyncedLyrics { get; set; } = [];
    }

    public class LyricsLineDto
    {
        [JsonPropertyName("time")]
        public double Time { get; set; }
        [JsonPropertyName("text")]
        public string Text { get; set; } = string.Empty;
    }
}
