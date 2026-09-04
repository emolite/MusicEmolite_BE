using System.Text.Json;
using MS_Application.DataTransferObjects.Base;
using MS_Application.DataTransferObjects.FoodEmolite;

namespace MS_Application.Services.Interfaces.External
{
    /// <summary>
    /// Calls FoodEmolite's own AdminController (a separate BE/repo, base URL from
    /// config "FoodEmolite_URL") over HTTP. That controller is AllowAnonymous on
    /// their side (meant for direct calls from another internal tool, not their own
    /// FE), so no auth token is sent here either. Every call returns the raw parsed
    /// JSON body (JsonElement) rather than a locally-typed DTO, since the response
    /// shapes belong to FoodEmolite's own models, not ours.
    /// </summary>
    public interface IFoodEmoliteService
    {
        // Stores
        Task<JsonElement> CreateStoreAsync(CreateFoodStoreRequestDto request);
        Task<JsonElement> UpdateStoreAsync(long id, UpdateFoodStoreRequestDto request);
        Task<JsonElement> DeleteStoreAsync(long id);
        Task<JsonElement> GetAllStoresAsync(int page = 1, int pageSize = 10);
        Task<JsonElement> GetStoreDetailAsync(long id);
        Task<JsonElement> GetStoresByOwnerAsync(string ownerRefCode, int page = 1, int pageSize = 10);

        // Accounts
        Task<JsonElement> GetAllUsersAsync(int page = 1, int pageSize = 10);
        Task<JsonElement> GetAllAgentsAsync(int page = 1, int pageSize = 10);
        Task<JsonElement> CreateAgentAsync(CreateFoodAgentRequestDto request);

        // Revenue
        Task<JsonElement> GetRevenueAsync(DateTime? fromDate, DateTime? toDate, string groupBy = "day");
        Task<JsonElement> GetTopProductsAsync(DateTime? fromDate, DateTime? toDate, string groupBy, string? storeRefCode, int top = 10);
        Task<JsonElement> SearchProductRevenueAsync(BaseSearchDto<FoodProductRevenueSearchRequest> request);

        // Customers
        Task<JsonElement> SearchCustomersAsync(BaseSearchDto<FoodCustomerSearchRequest> request);

        // Store foods
        Task<JsonElement> GetAllStoreFoodsAsync(int page = 1, int pageSize = 10);
    }
}
