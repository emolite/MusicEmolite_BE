

namespace MS_Application.DataTransferObjects.Artist
{
    public class ArtistRequestDto
    {
        public string? Keyword { get; set; }
        public string? Country { get; set; }
        public bool? IsActived { get; set; }
        public string? SortBy { get; set; }
    }

}
