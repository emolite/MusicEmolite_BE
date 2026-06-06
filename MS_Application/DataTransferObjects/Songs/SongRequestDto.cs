using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MS_Application.DataTransferObjects.Songs
{
    public class SongRequestDto
    {
        public string? Keyword { get; set; }
        public long? AlbumId { get; set; }
        public int? Type { get; set; }

        public bool? IsActived { get; set; }

        public string? SortBy { get; set; }
    }
}
