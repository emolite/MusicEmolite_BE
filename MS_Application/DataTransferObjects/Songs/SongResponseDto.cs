using Microsoft.AspNetCore.Http;
using MS_Application.DataTransferObjects.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MS_Application.DataTransferObjects.Songs
{
    public class SongResponseDto : BaseDto
    {
        public string Title { get; set; }
        public int Duration { get; set; }
        public string FileUrl { get; set; }
        public string? ImgUrl { get; set; }
        public DateOnly ReleaseDate { get; set; }
        public long AlbumId { get; set; }
        public string ArtistName { get; set; }
        public bool IsLiked { get; set; }
    }
}
