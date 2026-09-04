using Microsoft.AspNetCore.Mvc;
using MS_Application.DataTransferObjects.Base;
using MS_Application.DataTransferObjects.FoodEmolite;
using MS_Application.Services.Interfaces.External;

namespace MS_API.Controllers
{
    [ApiController]
    [Route("api/foodemolite")]
    public class FoodEmoliteController : BaseController
    {
        private readonly IFoodEmoliteService _foodEmoliteService;

        public FoodEmoliteController(IFoodEmoliteService foodEmoliteService)
        {
            _foodEmoliteService = foodEmoliteService;
        }

        [HttpPost("stores")]
        public async Task<IActionResult> CreateStore([FromForm] CreateFoodStoreRequestDto request)
        {
            var result = await _foodEmoliteService.CreateStoreAsync(request);
            return Ok(result);
        }

        [HttpPut("stores/{id}")]
        public async Task<IActionResult> UpdateStore(long id, [FromForm] UpdateFoodStoreRequestDto request)
        {
            var result = await _foodEmoliteService.UpdateStoreAsync(id, request);
            return Ok(result);
        }

        [HttpDelete("stores/{id}")]
        public async Task<IActionResult> DeleteStore(long id)
        {
            var result = await _foodEmoliteService.DeleteStoreAsync(id);
            return Ok(result);
        }

        [HttpGet("stores")]
        public async Task<IActionResult> GetAllStores([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _foodEmoliteService.GetAllStoresAsync(page, pageSize);
            return Ok(result);
        }

        [HttpGet("stores/{id}")]
        public async Task<IActionResult> GetStoreDetail(long id)
        {
            var result = await _foodEmoliteService.GetStoreDetailAsync(id);
            return Ok(result);
        }

        [HttpGet("stores/owner/{ownerRefCode}")]
        public async Task<IActionResult> GetStoresByOwner(
            string ownerRefCode,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            var result = await _foodEmoliteService.GetStoresByOwnerAsync(ownerRefCode, page, pageSize);
            return Ok(result);
        }

        [HttpGet("users")]
        public async Task<IActionResult> GetAllUsers([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _foodEmoliteService.GetAllUsersAsync(page, pageSize);
            return Ok(result);
        }

        [HttpGet("agents")]
        public async Task<IActionResult> GetAllAgents([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _foodEmoliteService.GetAllAgentsAsync(page, pageSize);
            return Ok(result);
        }

        [HttpPost("agents")]
        public async Task<IActionResult> CreateAgent([FromBody] CreateFoodAgentRequestDto request)
        {
            var result = await _foodEmoliteService.CreateAgentAsync(request);
            return Ok(result);
        }

        [HttpGet("revenue")]
        public async Task<IActionResult> GetRevenue(
            [FromQuery] DateTime? fromDate,
            [FromQuery] DateTime? toDate,
            [FromQuery] string groupBy = "day")
        {
            var result = await _foodEmoliteService.GetRevenueAsync(fromDate, toDate, groupBy);
            return Ok(result);
        }

        [HttpGet("revenue/top-products")]
        public async Task<IActionResult> GetTopProducts(
            [FromQuery] DateTime? fromDate,
            [FromQuery] DateTime? toDate,
            [FromQuery] string groupBy = "day",
            [FromQuery] string? storeRefCode = null,
            [FromQuery] int top = 10)
        {
            var result = await _foodEmoliteService.GetTopProductsAsync(fromDate, toDate, groupBy, storeRefCode, top);
            return Ok(result);
        }

        [HttpPost("revenue/products/search")]
        public async Task<IActionResult> SearchProductRevenue([FromBody] BaseSearchDto<FoodProductRevenueSearchRequest> request)
        {
            var result = await _foodEmoliteService.SearchProductRevenueAsync(request);
            return Ok(result);
        }

        [HttpPost("customers/search")]
        public async Task<IActionResult> SearchCustomers([FromBody] BaseSearchDto<FoodCustomerSearchRequest> request)
        {
            var result = await _foodEmoliteService.SearchCustomersAsync(request);
            return Ok(result);
        }

        [HttpGet("store-foods")]
        public async Task<IActionResult> GetAllStoreFoods([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _foodEmoliteService.GetAllStoreFoodsAsync(page, pageSize);
            return Ok(result);
        }
    }
}
