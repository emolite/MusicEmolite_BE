namespace MS_Application.DataTransferObjects.FoodEmolite
{
    public class FoodProductRevenueSearchRequest
    {
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string? Keyword { get; set; }

        /// <summary>Lọc theo 1 cửa hàng cụ thể bên FoodEmolite. Null = tất cả cửa hàng.</summary>
        public string? StoreRefCode { get; set; }
    }

    public class FoodCustomerSearchRequest
    {
        public string? Keyword { get; set; }

        /// <summary>Lọc theo 1 cửa hàng cụ thể bên FoodEmolite. Null = tất cả cửa hàng.</summary>
        public string? StoreRefCode { get; set; }
    }
}
