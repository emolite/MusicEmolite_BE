using MS_Application.Constants;
using MS_Application.DataTransferObjects.Base;
using MS_Application.DataTransferObjects.Message;
using MS_Application.Helpers;
using MS_Application.Repositories.Interfaces;
using MS_Application.Services.Interfaces;
using MS_Application.Services.Interfaces.External;
using MS_Domain.Entities.CRMS;

namespace MS_Application.Services
{
    public class MessageService : IMessageService
    {
        private readonly ICrmUnitOfWork _crmUnitOfWork;
        private readonly ICloudinaryService _cloudinaryService;
        private readonly IRealtimeNotifier _realtimeNotifier;

        public MessageService(
            ICrmUnitOfWork crmUnitOfWork,
            ICloudinaryService cloudinaryService,
            IRealtimeNotifier realtimeNotifier)
        {
            _crmUnitOfWork = crmUnitOfWork;
            _cloudinaryService = cloudinaryService;
            _realtimeNotifier = realtimeNotifier;
        }

        public async Task<BaseResponse<MessageResponseDto>> SendMessageAsync(long senderId, SendMessageRequestDto dto)
        {
            var result = new BaseResponse<MessageResponseDto>();

            if (string.IsNullOrWhiteSpace(dto.Content) && string.IsNullOrWhiteSpace(dto.ImagePublicId))
            {
                result.Code = ResponseStatusCode.Status400;
                return result.Fail("Tin nhắn không được để trống");
            }

            var isFriend = _crmUnitOfWork.GetRepositoryReadOnlyAsync<CrmFriendship>().QueryAll()
                .Any(x => !x.IsDeleted && x.Status == "ACCEPTED" &&
                    ((x.RequesterId == senderId && x.AddresseeId == dto.ReceiverId) ||
                     (x.RequesterId == dto.ReceiverId && x.AddresseeId == senderId)));

            if (!isFriend)
            {
                result.Code = ResponseStatusCode.Status403;
                return result.Fail("Chỉ có thể nhắn tin với bạn bè");
            }

            var now = DateTime.Now;

            var message = new CrmMessage
            {
                SenderId = senderId,
                ReceiverId = dto.ReceiverId,
                Content = dto.Content,
                ImagePublicId = dto.ImagePublicId,
                IsRead = false,
                CreatedAt = now,
                CreatedBy = senderId
            };

            var repoWrite = _crmUnitOfWork.GetRepositoryAsync<CrmMessage>();

            await repoWrite.AddAsync(message);
            await _crmUnitOfWork.SaveChangesAsync();

            var data = MapMessage(message);

            await _realtimeNotifier.NotifyUserAsync(dto.ReceiverId, "ReceiveMessage", data);

            result.Data = data;
            result.Code = ResponseStatusCode.Status200;
            return result.Success(data, "Đã gửi tin nhắn");
        }

        public async Task<BaseResponse<List<MessageResponseDto>>> GetConversationAsync(long userId, long otherUserId, int page, int pageSize)
        {
            var result = new BaseResponse<List<MessageResponseDto>>();

            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 30;

            var messages = _crmUnitOfWork.GetRepositoryReadOnlyAsync<CrmMessage>().QueryAll()
                .Where(m => !m.IsDeleted &&
                    ((m.SenderId == userId && m.ReceiverId == otherUserId) ||
                     (m.SenderId == otherUserId && m.ReceiverId == userId)))
                .OrderByDescending(m => m.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            messages.Reverse();

            var data = messages.Select(MapMessage).ToList();

            result.Data = data;
            result.Code = ResponseStatusCode.Status200;
            return result.Success(data, string.Format(Messages.Action.GetSuccess, "tin nhắn"));
        }

        public async Task<BaseResponse<List<ConversationResponseDto>>> GetConversationsAsync(long userId)
        {
            var result = new BaseResponse<List<ConversationResponseDto>>();

            var friendships = _crmUnitOfWork.GetRepositoryReadOnlyAsync<CrmFriendship>().QueryAll()
                .Where(x => !x.IsDeleted && x.Status == "ACCEPTED" && (x.RequesterId == userId || x.AddresseeId == userId))
                .ToList();

            var friendIds = friendships
                .Select(x => x.RequesterId == userId ? x.AddresseeId : x.RequesterId)
                .ToList();

            if (friendIds.Count == 0)
            {
                result.Data = new List<ConversationResponseDto>();
                result.Code = ResponseStatusCode.Status200;
                return result.Success(result.Data, string.Format(Messages.Action.GetSuccess, "cuộc trò chuyện"));
            }

            var repoUserRead = _crmUnitOfWork.GetRepositoryReadOnlyAsync<CrmUser>().QueryAll();
            var repoProfileRead = _crmUnitOfWork.GetRepositoryReadOnlyAsync<CrmUserProfile>().QueryAll();
            var repoMessageRead = _crmUnitOfWork.GetRepositoryReadOnlyAsync<CrmMessage>().QueryAll();

            var users = repoUserRead.Where(u => friendIds.Contains(u.Id)).ToList();
            var profiles = repoProfileRead.Where(p => friendIds.Contains(p.UserId)).ToList();

            var allMessages = repoMessageRead
                .Where(m => !m.IsDeleted &&
                    ((m.SenderId == userId && friendIds.Contains(m.ReceiverId)) ||
                     (m.ReceiverId == userId && friendIds.Contains(m.SenderId))))
                .ToList();

            var data = friendIds.Select(friendId =>
            {
                var user = users.FirstOrDefault(u => u.Id == friendId);
                var profile = profiles.FirstOrDefault(p => p.UserId == friendId);

                var conversationMessages = allMessages
                    .Where(m => m.SenderId == friendId || m.ReceiverId == friendId)
                    .OrderByDescending(m => m.CreatedAt)
                    .ToList();

                var lastMessage = conversationMessages.FirstOrDefault();

                var unreadCount = conversationMessages.Count(m => m.ReceiverId == userId && m.SenderId == friendId && !m.IsRead);

                return new ConversationResponseDto
                {
                    FriendUserId = friendId,
                    Username = user?.Username ?? string.Empty,
                    FullName = profile?.FullName,
                    AvatarUrl = string.IsNullOrWhiteSpace(profile?.Uri) ? null : _cloudinaryService.BuildImageUrl(profile!.Uri!),
                    LastMessage = lastMessage?.Content,
                    LastMessageHasImage = !string.IsNullOrWhiteSpace(lastMessage?.ImagePublicId),
                    IsLastMessageMine = lastMessage != null && lastMessage.SenderId == userId,
                    LastMessageAt = lastMessage?.CreatedAt,
                    UnreadCount = unreadCount
                };
            })
            .OrderByDescending(c => c.LastMessageAt ?? DateTime.MinValue)
            .ToList();

            result.Data = data;
            result.Code = ResponseStatusCode.Status200;
            return result.Success(data, string.Format(Messages.Action.GetSuccess, "cuộc trò chuyện"));
        }

        public async Task<BaseResponse<bool>> MarkAsReadAsync(long userId, long otherUserId)
        {
            var result = new BaseResponse<bool>();

            var repoRead = _crmUnitOfWork.GetRepositoryReadOnlyAsync<CrmMessage>().QueryAll();
            var repoWrite = _crmUnitOfWork.GetRepositoryAsync<CrmMessage>();

            var unread = repoRead
                .Where(m => !m.IsDeleted && m.ReceiverId == userId && m.SenderId == otherUserId && !m.IsRead)
                .ToList();

            if (unread.Count > 0)
            {
                var now = DateTime.Now;

                foreach (var message in unread)
                {
                    message.IsRead = true;
                    message.ReadAt = now;
                }

                await repoWrite.UpdateAsync(unread);
                await _crmUnitOfWork.SaveChangesAsync();
            }

            result.Data = true;
            result.Code = ResponseStatusCode.Status200;
            return result.Success(true, "Đã đánh dấu đã đọc");
        }

        private MessageResponseDto MapMessage(CrmMessage message)
        {
            return new MessageResponseDto
            {
                Id = message.Id,
                SenderId = message.SenderId,
                ReceiverId = message.ReceiverId,
                Content = message.Content,
                ImageUrl = string.IsNullOrWhiteSpace(message.ImagePublicId) ? null : _cloudinaryService.BuildImageUrl(message.ImagePublicId),
                IsRead = message.IsRead,
                CreatedAt = message.CreatedAt
            };
        }
    }
}
