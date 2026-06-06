using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MS_Application.DataTransferObjects.Bank
{
    public class BankResponseDto
    {
        public string Code { get; set; } = null!;

        public string Bin { get; set; } = null!;

        public string Name { get; set; } = null!;

        public string? ShortName { get; set; }

        public string? Logo { get; set; }
    }
}
