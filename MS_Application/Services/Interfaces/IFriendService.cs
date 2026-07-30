using MS_Application.DataTransferObjects.Base;
using MS_Application.DataTransferObjects.Friend;

namespace MS_Application.Services.Interfaces
{
    public interface IFriendService
    {
        Task<BaseResponse<FriendUserDto>> SendRequestAsync(long requesterId, SendFriendRequestDto dto);
        Task<BaseResponse<bool>> AcceptRequestAsync(long userId, long friendshipId);
        Task<BaseResponse<bool>> RejectRequestAsync(long userId, long friendshipId);
        Task<BaseResponse<bool>> RemoveFriendAsync(long userId, long friendUserId);
        Task<BaseResponse<List<FriendUserDto>>> GetFriendsAsync(long userId);
        Task<BaseResponse<List<FriendUserDto>>> GetPendingRequestsAsync(long userId);
        Task<BaseResponse<List<FriendUserDto>>> GetSentRequestsAsync(long userId);
        Task<BaseResponse<List<FriendSearchResultDto>>> SearchUsersAsync(long userId, string keyword);
    }
}
