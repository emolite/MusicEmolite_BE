namespace MS_Application.DataTransferObjects.Message
{
    public class ConversationResponseDto
    {
        public long FriendUserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string? FullName { get; set; }
        public string? AvatarUrl { get; set; }
        public string? LastMessage { get; set; }
        public bool LastMessageHasImage { get; set; }
        public bool IsLastMessageMine { get; set; }
        public DateTime? LastMessageAt { get; set; }
        public int UnreadCount { get; set; }
    }
}
