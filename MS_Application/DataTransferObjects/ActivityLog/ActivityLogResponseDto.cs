namespace MS_Application.DataTransferObjects.ActivityLog
{
    public class ActivityLogResponseDto
    {
        public long UserId { get; set; }
        public string UserName { get; set; } = string.Empty;

        /// <summary>"PLAY" hoặc "LIKE".</summary>
        public string ActionType { get; set; } = string.Empty;

        public long SongId { get; set; }
        public string SongTitle { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }
    }

    public class ActivityLogSearchRequest
    {
        public string? Keyword { get; set; }

        /// <summary>"PLAY" hoặc "LIKE". Null = tất cả.</summary>
        public string? ActionType { get; set; }

        public string? SortBy { get; set; }

        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
    }
}
