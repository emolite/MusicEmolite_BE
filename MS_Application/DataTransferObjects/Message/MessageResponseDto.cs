namespace MS_Application.DataTransferObjects.Message
{
    public class MessageResponseDto
    {
        public long Id { get; set; }
        public long SenderId { get; set; }
        public long ReceiverId { get; set; }
        public string? Content { get; set; }
        public string? ImageUrl { get; set; }
        public bool IsRead { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}
