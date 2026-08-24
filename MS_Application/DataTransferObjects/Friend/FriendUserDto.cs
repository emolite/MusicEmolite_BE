namespace MS_Application.DataTransferObjects.Friend
{
    public class FriendUserDto
    {
        public long FriendshipId { get; set; }
        public long UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string? FullName { get; set; }
        public string? AvatarUrl { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime? CreatedAt { get; set; }
        public bool IsPinned { get; set; }
    }
}
