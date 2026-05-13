using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MS_Application.DataTransferObjects.User
{
    public class CrmUserRequestDto
    {
        public string? RefCode { get; set; }
        public string? Username { get; set; }
        public string? Email { get; set; }
        public string? Role { get; set; }
    }
}
