using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MS_Application.DataTransferObjects.User.BankUser
{
    public class BankUserRequestDto
    {
        public string BankCode { get; set; } = null!;

        public string BankName { get; set; } = null!;

        public string AccountNo { get; set; } = null!;

        public string AccountName { get; set; } = null!;

        public string? VietQrUrl { get; set; }

        public string? QrImageUrl { get; set; }
    }
}
