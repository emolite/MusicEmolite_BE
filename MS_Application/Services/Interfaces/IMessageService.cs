using MS_Application.DataTransferObjects.Base;
using MS_Application.DataTransferObjects.Message;

namespace MS_Application.Services.Interfaces
{
    public interface IMessageService
    {
        Task<BaseResponse<MessageResponseDto>> SendMessageAsync(long senderId, SendMessageRequestDto dto);
        Task<BaseResponse<List<MessageResponseDto>>> GetConversationAsync(long userId, long otherUserId, int page, int pageSize);
        Task<BaseResponse<List<ConversationResponseDto>>> GetConversationsAsync(long userId);
        Task<BaseResponse<bool>> MarkAsReadAsync(long userId, long otherUserId);
    }
}
