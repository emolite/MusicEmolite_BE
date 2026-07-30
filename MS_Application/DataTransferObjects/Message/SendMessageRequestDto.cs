namespace MS_Application.DataTransferObjects.Message
{
    public class SendMessageRequestDto
    {
        public long ReceiverId { get; set; }
        public string? Content { get; set; }
        public string? ImagePublicId { get; set; }
    }
}
