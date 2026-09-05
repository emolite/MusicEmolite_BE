using MS_Application.DataTransferObjects.ActivityLog;
using MS_Application.DataTransferObjects.Base;

namespace MS_Application.Services.Interfaces
{
    public interface IActivityLogService
    {
        Task<BaseTableResponse<ActivityLogResponseDto>> SearchAsync(BaseSearchDto<ActivityLogSearchRequest> dto);
    }
}
