using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MS_Application.DataTransferObjects.GoogleLogin
{
    public class CompleteProfileRequestDto
    {
        public bool UseGoogleInfo { get; set; }
        public string? FullName { get; set; }
        public string? Uri { get; set; }
        public string? PhoneNumber { get; set; }
        public DateOnly? DateOfBirth { get; set; }
        public string? GoogleName { get; set; }
        public string? GooglePicture { get; set; }
    }
}
