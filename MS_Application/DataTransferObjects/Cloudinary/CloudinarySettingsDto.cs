using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MS_Application.DataTransferObjects.Cloudinary
{
    public class CloudinarySettingsDto
    {
        public string CloudName { get; set; } = string.Empty;
        public string ApiKey { get; set; } = string.Empty;
        public string ApiSecret { get; set; } = string.Empty;
        public string BaseAudioUrl { get; set; } = string.Empty;
        public string AudioTransform { get; set; } = string.Empty;
    }
}
