using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
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
    }
}
