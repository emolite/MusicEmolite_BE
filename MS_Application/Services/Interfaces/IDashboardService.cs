using MS_Application.DataTransferObjects.Base;
using MS_Application.DataTransferObjects.Dashboard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MS_Application.Services.Interfaces
{
    public interface IDashboardService
    {
        Task<BaseResponse<DashboardSummaryResponseDto>> GetSummary();

        Task<BaseResponse<List<DashboardTrendResponseDto>>> GetTrend();
    }
}
