using MS_Application.DataTransferObjects.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MS_Application.DataTransferObjects.Auth
{
    public class RegisterResponseDto : BaseDto
    {
        public string RefCode { get; set; }
        public string UserName { get; set; }
        public string RoleCode { get; set; }
    }
}
