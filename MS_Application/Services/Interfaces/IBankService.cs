using MS_Application.DataTransferObjects.Bank;
using MS_Application.DataTransferObjects.Base;
using MS_Application.Services.Interfaces.External;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace MS_Application.Services.Interfaces
{
    public interface IBankService
    {
        Task<BaseTableResponse<BankResponseDto>> GetBanks();
    }
}
