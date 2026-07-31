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

            if (dto.ReplyToMessageId.HasValue && dto.ForwardFromMessageId.HasValue)
            {
                result.Code = ResponseStatusCode.Status400;
                return result.Fail("Không thể vừa trả lời vừa chuyển tiếp cùng lúc");
            }

            var repoMessageRead = _crmUnitOfWork.GetRepositoryReadOnlyAsync<CrmMessage>().QueryAll();

            var content = dto.Content;
            var imagePublicId = dto.ImagePublicId;
            long? forwardedFromMessageId = null;

            if (dto.ForwardFromMessageId.HasValue)
            {
                var source = repoMessageRead.FirstOrDefault(m => m.Id == dto.ForwardFromMessageId.Value);

                if (source == null || source.IsDeleted ||
                    (source.SenderId != senderId && source.ReceiverId != senderId))
                {
                    result.Code = ResponseStatusCode.Status403;
                    return result.Fail("Không thể chuyển tiếp tin nhắn này");
                }

                content = source.Content;
                imagePublicId = source.ImagePublicId;
                forwardedFromMessageId = source.Id;
            }

            if (string.IsNullOrWhiteSpace(content) && string.IsNullOrWhiteSpace(imagePublicId))
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

            long? replyToMessageId = null;

            if (dto.ReplyToMessageId.HasValue)
            {
                var replySource = repoMessageRead.FirstOrDefault(m => m.Id == dto.ReplyToMessageId.Value);

                var belongsToConversation = replySource != null && !replySource.IsDeleted &&
                    ((replySource.SenderId == senderId && replySource.ReceiverId == dto.ReceiverId) ||
                     (replySource.SenderId == dto.ReceiverId && replySource.ReceiverId == senderId));

                if (!belongsToConversation)
                {
                    result.Code = ResponseStatusCode.Status400;
                    return result.Fail("Không tìm thấy tin nhắn để trả lời");
                }

                replyToMessageId = replySource!.Id;
            }

            var now = DateTime.Now;

            var message = new CrmMessage
            {
                SenderId = senderId,
                ReceiverId = dto.ReceiverId,
                Content = content,
                ImagePublicId = imagePublicId,
                IsRead = false,
                CreatedAt = now,
                CreatedBy = senderId,
                ReplyToMessageId = replyToMessageId,
                ForwardedFromMessageId = forwardedFromMessageId
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
                .Where(m =>
                    (m.SenderId == userId && m.ReceiverId == otherUserId) ||
                    (m.SenderId == otherUserId && m.ReceiverId == userId))
                .OrderByDescending(m => m.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            messages.Reverse();

            var data = MapMessages(messages);

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
                .Where(m =>
                    (m.SenderId == userId && friendIds.Contains(m.ReceiverId)) ||
                    (m.ReceiverId == userId && friendIds.Contains(m.SenderId)))
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
                var lastMessageDeleted = lastMessage?.IsDeleted ?? false;

                var unreadCount = conversationMessages.Count(m =>
                    m.ReceiverId == userId && m.SenderId == friendId && !m.IsRead && !m.IsDeleted);

                return new ConversationResponseDto
                {
                    FriendUserId = friendId,
                    Username = user?.Username ?? string.Empty,
                    FullName = profile?.FullName,
                    AvatarUrl = string.IsNullOrWhiteSpace(profile?.Uri) ? null : _cloudinaryService.BuildImageUrl(profile!.Uri!),
                    LastMessage = lastMessageDeleted ? null : lastMessage?.Content,
                    LastMessageHasImage = !lastMessageDeleted && !string.IsNullOrWhiteSpace(lastMessage?.ImagePublicId),
                    IsLastMessageDeleted = lastMessageDeleted,
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

        public async Task<BaseResponse<bool>> DeleteMessageAsync(long userId, long messageId)
        {
            var result = new BaseResponse<bool>();

            var repoWrite = _crmUnitOfWork.GetRepositoryAsync<CrmMessage>();

            var message = _crmUnitOfWork.GetRepositoryReadOnlyAsync<CrmMessage>().QueryAll()
                .FirstOrDefault(m => m.Id == messageId && !m.IsDeleted);

            if (message == null)
            {
                result.Code = ResponseStatusCode.Status404;
                return result.Fail("Không tìm thấy tin nhắn");
            }

            if (message.SenderId != userId)
            {
                result.Code = ResponseStatusCode.Status403;
                return result.Fail("Bạn không thể thu hồi tin nhắn này");
            }

            message.IsDeleted = true;
            message.UpdatedAt = DateTime.Now;
            message.UpdatedBy = userId;

            await repoWrite.UpdateAsync(message);
            await _crmUnitOfWork.SaveChangesAsync();

            await _realtimeNotifier.NotifyUserAsync(message.ReceiverId, "MessageDeleted", new { messageId = message.Id });

            result.Data = true;
            result.Code = ResponseStatusCode.Status200;
            return result.Success(true, "Đã thu hồi tin nhắn");
        }

        private MessageResponseDto MapMessage(CrmMessage message)
        {
            CrmMessage? replyTo = null;

            if (message.ReplyToMessageId.HasValue)
            {
                replyTo = _crmUnitOfWork.GetRepositoryReadOnlyAsync<CrmMessage>().QueryAll()
                    .FirstOrDefault(m => m.Id == message.ReplyToMessageId.Value);
            }

            return MapMessage(message, replyTo);
        }

        private List<MessageResponseDto> MapMessages(List<CrmMessage> messages)
        {
            var replyToIds = messages
                .Where(m => m.ReplyToMessageId.HasValue)
                .Select(m => m.ReplyToMessageId!.Value)
                .Distinct()
                .ToList();

            var replyToMessages = replyToIds.Count == 0
                ? new List<CrmMessage>()
                : _crmUnitOfWork.GetRepositoryReadOnlyAsync<CrmMessage>().QueryAll()
                    .Where(m => replyToIds.Contains(m.Id))
                    .ToList();

            return messages
                .Select(m => MapMessage(
                    m,
                    m.ReplyToMessageId.HasValue
                        ? replyToMessages.FirstOrDefault(r => r.Id == m.ReplyToMessageId.Value)
                        : null))
                .ToList();
        }

        private MessageResponseDto MapMessage(CrmMessage message, CrmMessage? replyToMessage)
        {
            return new MessageResponseDto
            {
                Id = message.Id,
                SenderId = message.SenderId,
                ReceiverId = message.ReceiverId,
                Content = message.IsDeleted ? null : message.Content,
                ImageUrl = message.IsDeleted || string.IsNullOrWhiteSpace(message.ImagePublicId)
                    ? null
                    : _cloudinaryService.BuildImageUrl(message.ImagePublicId),
                IsRead = message.IsRead,
                CreatedAt = message.CreatedAt,
                IsDeleted = message.IsDeleted,

                ReplyToMessageId = message.ReplyToMessageId,
                ReplyToContent = replyToMessage != null && !replyToMessage.IsDeleted ? replyToMessage.Content : null,
                ReplyToHasImage = replyToMessage != null && !replyToMessage.IsDeleted && !string.IsNullOrWhiteSpace(replyToMessage.ImagePublicId),
                ReplyToSenderId = replyToMessage?.SenderId,
                ReplyToIsDeleted = replyToMessage?.IsDeleted ?? false,

                ForwardedFromMessageId = message.ForwardedFromMessageId
            };
        }
    }
}
