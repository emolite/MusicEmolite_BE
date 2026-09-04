using Microsoft.AspNetCore.Http;

namespace MS_Application.DataTransferObjects.FoodEmolite
{
    public class CreateFoodStoreRequestDto
    {
        public string StoreName { get; set; } = string.Empty;
        public long OwnerAccountId { get; set; }
        public IFormFile? ThumbnailFile { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
        public string? Description { get; set; }
    }

    public class UpdateFoodStoreRequestDto
    {
        public string StoreName { get; set; } = string.Empty;
        public IFormFile? ThumbnailFile { get; set; }
        public string? ThumbnailFileRefCode { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
        public string? Description { get; set; }
        public bool IsActive { get; set; }
    }
}
