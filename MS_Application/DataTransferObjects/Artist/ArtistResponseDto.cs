using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MS_Application.DataTransferObjects.Artist
{
    public class ArtistResponseDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string StageName { get; set; }
        public string Country { get; set; }
        public bool IsActived { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}
