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
        public bool IsDeleted { get; set; }

        public long? ReplyToMessageId { get; set; }
        public string? ReplyToContent { get; set; }
        public bool ReplyToHasImage { get; set; }
        public long? ReplyToSenderId { get; set; }
        public bool ReplyToIsDeleted { get; set; }

        public long? ForwardedFromMessageId { get; set; }
    }
}
