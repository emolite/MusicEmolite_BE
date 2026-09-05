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

    public class FoodOrderSearchRequest
    {
        public string? Keyword { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }

        /// <summary>Lọc theo 1 cửa hàng cụ thể bên FoodEmolite. Null = tất cả cửa hàng.</summary>
        public string? StoreRefCode { get; set; }
        public string? OrderStatus { get; set; }
        public string? PaymentStatus { get; set; }
    }

    public class FoodActivityLogSearchRequest
    {
        public string? Keyword { get; set; }
        public string? Action { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
    }
}
