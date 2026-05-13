using MS_Application.DataTransferObjects.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MS_Application.DataTransferObjects.Role
{
    public class RoleResponseDto : BaseDto
    {
        public string RoleCode { get; set; }
        public string RoleName { get; set; }

    }
}
