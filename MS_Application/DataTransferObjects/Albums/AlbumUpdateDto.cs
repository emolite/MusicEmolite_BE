using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MS_Application.DataTransferObjects.Albums
{
    public class AlbumUpdateDto
    {
        public string Title { get; set; } = string.Empty;

        public DateTime ReleaseDate { get; set; }

        public long? ArtistId { get; set; }

        public bool IsActived { get; set; }
    }
}
