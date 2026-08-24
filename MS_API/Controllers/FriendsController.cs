using Microsoft.AspNetCore.Mvc;
using MS_Application.DataTransferObjects.Base;
using MS_Application.DataTransferObjects.Friend;
using MS_Application.Services.Interfaces;

namespace MS_API.Controllers
{
    [Route("api/friends")]
    [ApiController]
    public class FriendsController : BaseController
    {
        private readonly IFriendService _friendService;

        public FriendsController(IFriendService friendService)
        {
            _friendService = friendService;
        }

        [HttpGet]
        public async Task<BaseResponse<List<FriendUserDto>>> GetFriends()
        {
            return await _friendService.GetFriendsAsync(UserId);
        }

        [HttpGet("requests")]
        public async Task<BaseResponse<List<FriendUserDto>>> GetPendingRequests()
        {
            return await _friendService.GetPendingRequestsAsync(UserId);
        }

        [HttpGet("requests/sent")]
        public async Task<BaseResponse<List<FriendUserDto>>> GetSentRequests()
        {
            return await _friendService.GetSentRequestsAsync(UserId);
        }

        [HttpGet("search")]
        public async Task<BaseResponse<List<FriendSearchResultDto>>> Search([FromQuery] string keyword)
        {
            return await _friendService.SearchUsersAsync(UserId, keyword ?? string.Empty);
        }

        [HttpPost("requests")]
        public async Task<BaseResponse<FriendUserDto>> SendRequest([FromBody] SendFriendRequestDto dto)
        {
            return await _friendService.SendRequestAsync(UserId, dto);
        }

        [HttpPut("requests/{friendshipId}/accept")]
        public async Task<BaseResponse<bool>> Accept(long friendshipId)
        {
            return await _friendService.AcceptRequestAsync(UserId, friendshipId);
        }

        [HttpPut("requests/{friendshipId}/reject")]
        public async Task<BaseResponse<bool>> Reject(long friendshipId)
        {
            return await _friendService.RejectRequestAsync(UserId, friendshipId);
        }

        [HttpDelete("{friendUserId}")]
        public async Task<BaseResponse<bool>> RemoveFriend(long friendUserId)
        {
            return await _friendService.RemoveFriendAsync(UserId, friendUserId);
        }

        [HttpPut("{friendUserId}/pin")]
        public async Task<BaseResponse<bool>> TogglePin(long friendUserId)
        {
            return await _friendService.TogglePinAsync(UserId, friendUserId);
        }
    }
}
