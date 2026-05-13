using MS_Application.DataTransferObjects.Base;
using MS_Domain.Entities.CRMS;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MS_Application.DataTransferObjects.Auth
{
    public class UserVerifyDto
    {
        public string RefCode { get; set; }
        public long UserId { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string RoleCode { get; set; }
        public CrmUserProfile Profile { get; set; }
    }
}
