using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MS_Application.DataTransferObjects.Albums
{
    public class AlbumRequestDto
    {
        public string? Keyword { get; set; }

        public int? AlbumType { get; set; }

        public bool? IsActived { get; set; }

        public string? SortBy { get; set; }
    }
}
