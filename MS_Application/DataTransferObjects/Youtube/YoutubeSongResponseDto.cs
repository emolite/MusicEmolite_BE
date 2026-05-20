using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MS_Application.DataTransferObjects.Youtube
{
    public class YoutubeSongResponseDto
    {
        public string Id { get; set; }

        public string Title { get; set; }

        public string Artist { get; set; }

        public string Thumbnail { get; set; }

        public double Duration { get; set; }
    }
}
