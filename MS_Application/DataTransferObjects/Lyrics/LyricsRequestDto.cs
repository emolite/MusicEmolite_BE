
namespace MS_Application.DataTransferObjects.Lyrics
{
    public class LyricsRequestDto
    {
        public string? Title { get; set; }
        public string? Artist { get; set; }
        public string? AlbumName { get; set; }
        public double Duration { get; set; }
    }
}
