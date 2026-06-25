using MS_Application.DataTransferObjects.Base;

namespace MS_Application.DataTransferObjects.Albums
{
    public class AlbumResponseDto : BaseDto
    {
        public string Title { get; set; } = string.Empty;

        public DateTime? ReleaseDate { get; set; }

        public long? ArtistId { get; set; }

        public string AlbumTypeName { get; set; }

        public string? Uri { get; set; }
    }
}
