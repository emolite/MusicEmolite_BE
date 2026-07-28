using System.Collections.Generic;

namespace MS_Application.DataTransferObjects.Songs
{
    // Danh sách "kết hợp" kiểu Mix của Youtube/Spotify: gói các bài hát (vd: hay nghe nhất)
    // thành 1 playlist ảo có cover + title, dùng chung được cho cả web và mobile.
    public class MixPlaylistResponseDto
    {
        public string Key { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public List<string> CoverImages { get; set; } = new();
        public int TotalSongs { get; set; }
        public List<SongResponseDto> Songs { get; set; } = new();
    }
}
