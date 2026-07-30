namespace MS_Application.DataTransferObjects.Friend
{
    public class FriendSearchResultDto
    {
        public long UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string? FullName { get; set; }
        public string? AvatarUrl { get; set; }

        /// <summary>NONE, FRIENDS, PENDING_SENT, PENDING_RECEIVED</summary>
        public string FriendStatus { get; set; } = "NONE";
        public long? FriendshipId { get; set; }
    }
}
