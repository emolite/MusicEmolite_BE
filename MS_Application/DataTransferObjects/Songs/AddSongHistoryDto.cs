using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MS_Application.DataTransferObjects.Songs
{
    public class AddSongHistoryDto
    {
        public string VideoId { get; set; } = "";
        public string Title { get; set; } = "";
        public string Channel { get; set; } = "";
        public string ThumbnailHigh { get; set; } = "";
        public int Duration { get; set; }
    }
}
