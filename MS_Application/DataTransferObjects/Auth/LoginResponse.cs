using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MS_Application.DataTransferObjects.Auth
{
    public class LoginResponseDto
    {
        public string AccessToken { get; set; }
        public string RefCode { get; set; }
        public string RoleCode { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
