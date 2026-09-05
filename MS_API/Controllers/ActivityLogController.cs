using Microsoft.AspNetCore.Mvc;
using MS_Application.DataTransferObjects.ActivityLog;
using MS_Application.DataTransferObjects.Base;
using MS_Application.Services.Interfaces;

namespace MS_API.Controllers
{
    /// <summary>
    /// Lịch sử hoạt động của người dùng MusicEmolite đã đăng nhập (nghe/thích bài hát).
    /// Dữ liệu lấy từ song_histories/user_likes đã có sẵn - không cần bảng log riêng.
    /// </summary>
    [ApiController]
    [Route("api/activity-logs")]
    public class ActivityLogController : BaseController
    {
        private readonly IActivityLogService _activityLogService;

        public ActivityLogController(IActivityLogService activityLogService)
        {
            _activityLogService = activityLogService;
        }

        [HttpPost("search")]
        public async Task<IActionResult> Search([FromBody] BaseSearchDto<ActivityLogSearchRequest> dto)
        {
            var result = await _activityLogService.SearchAsync(dto);
            return Ok(result);
        }
    }
}
