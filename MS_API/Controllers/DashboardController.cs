using Microsoft.AspNetCore.Mvc;
using MS_Application.Services.Interfaces;

namespace MS_API.Controllers
{
    [ApiController]
    [Route("api/dashboard")]
    public class DashboardController : BaseController
    {
        private readonly IDashboardService _dashboardService;

        public DashboardController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        [HttpGet("summary")]
        public async Task<IActionResult> GetSummary()
        {
            var result = await _dashboardService.GetSummary();
            return Ok(result);
        }

        [HttpGet("trend")]
        public async Task<IActionResult> GetTrend()
        {
            var result = await _dashboardService.GetTrend();
            return Ok(result);
        }
    }
}
