using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MS_Application.DataTransferObjects.Albums
{
    public class AlbumCreateDto
    {
        public string Title { get; set; } = string.Empty;

        public DateTime ReleaseDate { get; set; }

        public long? ArtistId { get; set; }

        public byte AlbumType { get; set; }

        public IFormFile Image { get; set; }
    }
}
