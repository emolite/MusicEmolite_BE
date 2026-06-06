using MS_Application.DataTransferObjects.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MS_Application.DataTransferObjects.User
{
    public class CrmUserProfileResponseDto : BaseDto
    {
        public string? RefCode { get; set; }
        public long? UserId { get; set; }
        public string? FullName { get; set; }
        public string? Phone { get; set; }
        public DateOnly? DateOfBirth { get; set; }
        public string? Gender { get; set; }
        public string? Bio { get; set; }
        public string? Uri { get; set; }
    }
}
