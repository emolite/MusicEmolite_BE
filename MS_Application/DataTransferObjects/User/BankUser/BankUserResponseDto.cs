using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MS_Application.DataTransferObjects.User.BankUser
{
    public class BankUserResponseDto
    {
        public long Id { get; set; }

        public long UserProfileId { get; set; }

        public string? BankCode { get; set; }

        public string? BankName { get; set; }

        public string? AccountNo { get; set; }

        public string? AccountName { get; set; }

        public string? VietQrUrl { get; set; }

        public string? QrImageUrl { get; set; }

        public bool IsActive { get; set; }

        public DateTime? CreatedAt { get; set; }
    }
}
