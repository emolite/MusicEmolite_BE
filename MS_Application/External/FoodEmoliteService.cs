using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using MS_Application.DataTransferObjects.Base;
using MS_Application.DataTransferObjects.FoodEmolite;
using MS_Application.Services.Interfaces.External;

namespace MS_Application.External;

public class FoodEmoliteService : IFoodEmoliteService
{
    private readonly HttpClient _httpClient;
    private readonly string _baseUrl;

    public FoodEmoliteService(IConfiguration configuration, IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient();
        _baseUrl = (configuration["FoodEmolite_URL"] ?? "").TrimEnd('/');
    }

    // ===================== Stores =====================

    public async Task<JsonElement> CreateStoreAsync(CreateFoodStoreRequestDto request)
    {
        using var content = new MultipartFormDataContent
        {
            { new StringContent(request.StoreName ?? ""), "StoreName" },
            { new StringContent(request.OwnerAccountId.ToString()), "OwnerAccountId" }
        };

        AddOptionalField(content, "PhoneNumber", request.PhoneNumber);
        AddOptionalField(content, "Address", request.Address);
        AddOptionalField(content, "Description", request.Description);
        AddFile(content, "ThumbnailFile", request.ThumbnailFile);

        var response = await _httpClient.PostAsync($"{_baseUrl}/admin/stores", content);
        return await ParseResponseAsync(response);
    }

    public async Task<JsonElement> UpdateStoreAsync(long id, UpdateFoodStoreRequestDto request)
    {
        using var content = new MultipartFormDataContent
        {
            { new StringContent(request.StoreName ?? ""), "StoreName" },
            { new StringContent(request.IsActive.ToString()), "IsActive" }
        };

        AddOptionalField(content, "ThumbnailFileRefCode", request.ThumbnailFileRefCode);
        AddOptionalField(content, "PhoneNumber", request.PhoneNumber);
        AddOptionalField(content, "Address", request.Address);
        AddOptionalField(content, "Description", request.Description);
        AddFile(content, "ThumbnailFile", request.ThumbnailFile);

        using var httpRequest = new HttpRequestMessage(HttpMethod.Put, $"{_baseUrl}/admin/stores/{id}")
        {
            Content = content
        };

        var response = await _httpClient.SendAsync(httpRequest);
        return await ParseResponseAsync(response);
    }

    public async Task<JsonElement> DeleteStoreAsync(long id)
    {
        var response = await _httpClient.DeleteAsync($"{_baseUrl}/admin/stores/{id}");
        return await ParseResponseAsync(response);
    }

    public async Task<JsonElement> GetAllStoresAsync(int page = 1, int pageSize = 10)
    {
        var response = await _httpClient.GetAsync($"{_baseUrl}/admin/stores?page={page}&pageSize={pageSize}");
        return await ParseResponseAsync(response);
    }

    public async Task<JsonElement> GetStoreDetailAsync(long id)
    {
        var response = await _httpClient.GetAsync($"{_baseUrl}/admin/stores/{id}");
        return await ParseResponseAsync(response);
    }

    public async Task<JsonElement> GetStoresByOwnerAsync(string ownerRefCode, int page = 1, int pageSize = 10)
    {
        var response = await _httpClient.GetAsync(
            $"{_baseUrl}/admin/stores/owner/{Uri.EscapeDataString(ownerRefCode)}?page={page}&pageSize={pageSize}");

        return await ParseResponseAsync(response);
    }

    // ===================== Accounts =====================

    public async Task<JsonElement> GetAllUsersAsync(int page = 1, int pageSize = 10)
    {
        var response = await _httpClient.GetAsync($"{_baseUrl}/admin/users?page={page}&pageSize={pageSize}");
        return await ParseResponseAsync(response);
    }

    public async Task<JsonElement> GetAllAgentsAsync(int page = 1, int pageSize = 10)
    {
        var response = await _httpClient.GetAsync($"{_baseUrl}/admin/agents?page={page}&pageSize={pageSize}");
        return await ParseResponseAsync(response);
    }

    public async Task<JsonElement> CreateAgentAsync(CreateFoodAgentRequestDto request)
    {
        var response = await _httpClient.PostAsync($"{_baseUrl}/admin/agents", ToJsonContent(request));
        return await ParseResponseAsync(response);
    }

    // ===================== Revenue =====================

    public async Task<JsonElement> GetRevenueAsync(DateTime? fromDate, DateTime? toDate, string groupBy = "day")
    {
        var query = BuildRevenueQuery(fromDate, toDate, groupBy);
        var response = await _httpClient.GetAsync($"{_baseUrl}/admin/revenue{query}");
        return await ParseResponseAsync(response);
    }

    public async Task<JsonElement> GetTopProductsAsync(DateTime? fromDate, DateTime? toDate, string groupBy, string? storeRefCode, int top = 10)
    {
        var query = BuildRevenueQuery(fromDate, toDate, groupBy);
        query += (query.Length == 0 ? "?" : "&") + $"top={top}";

        if (!string.IsNullOrWhiteSpace(storeRefCode))
            query += $"&storeRefCode={Uri.EscapeDataString(storeRefCode)}";

        var response = await _httpClient.GetAsync($"{_baseUrl}/admin/revenue/top-products{query}");
        return await ParseResponseAsync(response);
    }

    public async Task<JsonElement> SearchProductRevenueAsync(BaseSearchDto<FoodProductRevenueSearchRequest> request)
    {
        var response = await _httpClient.PostAsync($"{_baseUrl}/admin/revenue/products/search", ToJsonContent(request));
        return await ParseResponseAsync(response);
    }

    // ===================== Customers =====================

    public async Task<JsonElement> SearchCustomersAsync(BaseSearchDto<FoodCustomerSearchRequest> request)
    {
        var response = await _httpClient.PostAsync($"{_baseUrl}/admin/customers/search", ToJsonContent(request));
        return await ParseResponseAsync(response);
    }

    // ===================== Store foods =====================

    public async Task<JsonElement> GetAllStoreFoodsAsync(int page = 1, int pageSize = 10)
    {
        var response = await _httpClient.GetAsync($"{_baseUrl}/admin/store-foods?page={page}&pageSize={pageSize}");
        return await ParseResponseAsync(response);
    }

    // ===================== Helpers =====================

    private static string BuildRevenueQuery(DateTime? fromDate, DateTime? toDate, string groupBy)
    {
        var parts = new List<string>();

        if (fromDate.HasValue)
            parts.Add($"FromDate={Uri.EscapeDataString(fromDate.Value.ToString("O"))}");

        if (toDate.HasValue)
            parts.Add($"ToDate={Uri.EscapeDataString(toDate.Value.ToString("O"))}");

        if (!string.IsNullOrWhiteSpace(groupBy))
            parts.Add($"GroupBy={Uri.EscapeDataString(groupBy)}");

        return parts.Count == 0 ? "" : "?" + string.Join("&", parts);
    }

    private static void AddOptionalField(MultipartFormDataContent content, string name, string? value)
    {
        if (!string.IsNullOrEmpty(value))
            content.Add(new StringContent(value), name);
    }

    private static void AddFile(MultipartFormDataContent content, string name, Microsoft.AspNetCore.Http.IFormFile? file)
    {
        if (file == null) return;

        var streamContent = new StreamContent(file.OpenReadStream());
        streamContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);
        content.Add(streamContent, name, file.FileName);
    }

    private static StringContent ToJsonContent<T>(T body)
    {
        return new StringContent(JsonSerializer.Serialize(body), Encoding.UTF8, "application/json");
    }

    /// <summary>
    /// Parses the response body as JSON regardless of status code, since
    /// FoodEmolite encodes success/failure inside the JSON body itself
    /// (same BaseResponse-style convention this app uses) rather than only
    /// through the HTTP status. Cloned so it stays valid after the backing
    /// JsonDocument is disposed.
    /// </summary>
    private static async Task<JsonElement> ParseResponseAsync(HttpResponseMessage response)
    {
        var json = await response.Content.ReadAsStringAsync();

        if (string.IsNullOrWhiteSpace(json))
        {
            using var empty = JsonDocument.Parse("{}");
            return empty.RootElement.Clone();
        }

        using var doc = JsonDocument.Parse(json);
        return doc.RootElement.Clone();
    }
}
