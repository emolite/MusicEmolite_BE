using MS_Application.Constants;
using MS_Application.DataTransferObjects.Base;
using MS_Application.DataTransferObjects.Friend;
using MS_Application.Helpers;
using MS_Application.Repositories.Interfaces;
using MS_Application.Services.Interfaces;
using MS_Application.Services.Interfaces.External;
using MS_Domain.Entities.CRMS;

namespace MS_Application.Services
{
    public class FriendService : IFriendService
    {
        private readonly ICrmUnitOfWork _crmUnitOfWork;
        private readonly ICloudinaryService _cloudinaryService;
        private readonly IRealtimeNotifier _realtimeNotifier;

        public FriendService(
            ICrmUnitOfWork crmUnitOfWork,
            ICloudinaryService cloudinaryService,
            IRealtimeNotifier realtimeNotifier)
        {
            _crmUnitOfWork = crmUnitOfWork;
            _cloudinaryService = cloudinaryService;
            _realtimeNotifier = realtimeNotifier;
        }

        public async Task<BaseResponse<FriendUserDto>> SendRequestAsync(long requesterId, SendFriendRequestDto dto)
        {
            var result = new BaseResponse<FriendUserDto>();

            if (dto.AddresseeId == requesterId)
            {
                result.Code = ResponseStatusCode.Status400;
                return result.Fail("Không thể tự kết bạn với chính mình");
            }

            var repoUserRead = _crmUnitOfWork.GetRepositoryReadOnlyAsync<CrmUser>().QueryAll();
            var addressee = repoUserRead.FirstOrDefault(u => u.Id == dto.AddresseeId && !u.IsDeleted);

            if (addressee == null)
            {
                result.Code = ResponseStatusCode.Status404;
                return result.Fail("Không tìm thấy người dùng");
            }

            var repoFriendRead = _crmUnitOfWork.GetRepositoryReadOnlyAsync<CrmFriendship>().QueryAll();
            var repoFriendWrite = _crmUnitOfWork.GetRepositoryAsync<CrmFriendship>();

            var existing = repoFriendRead.FirstOrDefault(x =>
                !x.IsDeleted &&
                ((x.RequesterId == requesterId && x.AddresseeId == dto.AddresseeId) ||
                 (x.RequesterId == dto.AddresseeId && x.AddresseeId == requesterId)));

            var now = DateTime.Now;

            if (existing != null)
            {
                if (existing.Status == "ACCEPTED")
                {
                    result.Code = ResponseStatusCode.Status409;
                    return result.Fail("Hai người đã là bạn bè");
                }

                if (existing.Status == "PENDING")
                {
                    result.Code = ResponseStatusCode.Status409;
                    return result.Fail(existing.RequesterId == requesterId
                        ? "Bạn đã gửi lời mời trước đó"
                        : "Người này đã gửi lời mời kết bạn cho bạn, hãy vào mục lời mời để chấp nhận");
                }
            }

            long friendshipId;

            if (existing != null)
            {
                existing.RequesterId = requesterId;
                existing.AddresseeId = dto.AddresseeId;
                existing.Status = "PENDING";
                existing.RespondedAt = null;
                existing.UpdatedAt = now;
                existing.UpdatedBy = requesterId;

                await repoFriendWrite.UpdateAsync(existing);
                friendshipId = existing.Id;
            }
            else
            {
                var friendship = new CrmFriendship
                {
                    RequesterId = requesterId,
                    AddresseeId = dto.AddresseeId,
                    Status = "PENDING",
                    CreatedAt = now,
                    CreatedBy = requesterId
                };

                await repoFriendWrite.AddAsync(friendship);
                friendshipId = friendship.Id;
            }

            await _crmUnitOfWork.SaveChangesAsync();

            var repoProfileRead = _crmUnitOfWork.GetRepositoryReadOnlyAsync<CrmUserProfile>().QueryAll();
            var requesterUser = repoUserRead.FirstOrDefault(u => u.Id == requesterId);
            var requesterProfile = repoProfileRead.FirstOrDefault(p => p.UserId == requesterId);

            var data = new FriendUserDto
            {
                FriendshipId = friendshipId,
                UserId = requesterId,
                Username = requesterUser?.Username ?? string.Empty,
                FullName = requesterProfile?.FullName,
                AvatarUrl = string.IsNullOrWhiteSpace(requesterProfile?.Uri) ? null : _cloudinaryService.BuildImageUrl(requesterProfile!.Uri!),
                Status = "PENDING",
                CreatedAt = now
            };

            await _realtimeNotifier.NotifyUserAsync(dto.AddresseeId, "FriendRequestReceived", data);

            result.Code = ResponseStatusCode.Status200;
            return result.Success(data, "Đã gửi lời mời kết bạn");
        }

        public async Task<BaseResponse<bool>> AcceptRequestAsync(long userId, long friendshipId)
        {
            var result = new BaseResponse<bool>();

            var repoFriendRead = _crmUnitOfWork.GetRepositoryReadOnlyAsync<CrmFriendship>().QueryAll();
            var repoFriendWrite = _crmUnitOfWork.GetRepositoryAsync<CrmFriendship>();

            var friendship = repoFriendRead.FirstOrDefault(x =>
                x.Id == friendshipId &&
                x.AddresseeId == userId &&
                x.Status == "PENDING" &&
                !x.IsDeleted);

            if (friendship == null)
            {
                result.Code = ResponseStatusCode.Status404;
                return result.Fail(false, "Không tìm thấy lời mời kết bạn");
            }

            var now = DateTime.Now;

            friendship.Status = "ACCEPTED";
            friendship.RespondedAt = now;
            friendship.UpdatedAt = now;
            friendship.UpdatedBy = userId;

            await repoFriendWrite.UpdateAsync(friendship);
            await _crmUnitOfWork.SaveChangesAsync();

            var repoUserRead = _crmUnitOfWork.GetRepositoryReadOnlyAsync<CrmUser>().QueryAll();
            var repoProfileRead = _crmUnitOfWork.GetRepositoryReadOnlyAsync<CrmUserProfile>().QueryAll();

            var addresseeUser = repoUserRead.FirstOrDefault(u => u.Id == userId);
            var addresseeProfile = repoProfileRead.FirstOrDefault(p => p.UserId == userId);

            var data = new FriendUserDto
            {
                FriendshipId = friendship.Id,
                UserId = userId,
                Username = addresseeUser?.Username ?? string.Empty,
                FullName = addresseeProfile?.FullName,
                AvatarUrl = string.IsNullOrWhiteSpace(addresseeProfile?.Uri) ? null : _cloudinaryService.BuildImageUrl(addresseeProfile!.Uri!),
                Status = "ACCEPTED",
                CreatedAt = now
            };

            await _realtimeNotifier.NotifyUserAsync(friendship.RequesterId, "FriendRequestAccepted", data);

            result.Data = true;
            result.Code = ResponseStatusCode.Status200;
            return result.Success(true, "Đã chấp nhận lời mời kết bạn");
        }

        public async Task<BaseResponse<bool>> RejectRequestAsync(long userId, long friendshipId)
        {
            var result = new BaseResponse<bool>();

            var repoFriendRead = _crmUnitOfWork.GetRepositoryReadOnlyAsync<CrmFriendship>().QueryAll();
            var repoFriendWrite = _crmUnitOfWork.GetRepositoryAsync<CrmFriendship>();

            var friendship = repoFriendRead.FirstOrDefault(x =>
                x.Id == friendshipId &&
                x.AddresseeId == userId &&
                x.Status == "PENDING" &&
                !x.IsDeleted);

            if (friendship == null)
            {
                result.Code = ResponseStatusCode.Status404;
                return result.Fail(false, "Không tìm thấy lời mời kết bạn");
            }

            friendship.Status = "REJECTED";
            friendship.RespondedAt = DateTime.Now;
            friendship.IsDeleted = true;
            friendship.UpdatedAt = DateTime.Now;
            friendship.UpdatedBy = userId;

            await repoFriendWrite.UpdateAsync(friendship);
            await _crmUnitOfWork.SaveChangesAsync();

            result.Data = true;
            result.Code = ResponseStatusCode.Status200;
            return result.Success(true, "Đã từ chối lời mời kết bạn");
        }

        public async Task<BaseResponse<bool>> RemoveFriendAsync(long userId, long friendUserId)
        {
            var result = new BaseResponse<bool>();

            var repoFriendRead = _crmUnitOfWork.GetRepositoryReadOnlyAsync<CrmFriendship>().QueryAll();
            var repoFriendWrite = _crmUnitOfWork.GetRepositoryAsync<CrmFriendship>();

            var friendship = repoFriendRead.FirstOrDefault(x =>
                !x.IsDeleted &&
                x.Status == "ACCEPTED" &&
                ((x.RequesterId == userId && x.AddresseeId == friendUserId) ||
                 (x.RequesterId == friendUserId && x.AddresseeId == userId)));

            if (friendship == null)
            {
                result.Code = ResponseStatusCode.Status404;
                return result.Fail(false, "Không tìm thấy quan hệ bạn bè");
            }

            friendship.IsDeleted = true;
            friendship.UpdatedAt = DateTime.Now;
            friendship.UpdatedBy = userId;

            await repoFriendWrite.UpdateAsync(friendship);
            await _crmUnitOfWork.SaveChangesAsync();

            result.Data = true;
            result.Code = ResponseStatusCode.Status200;
            return result.Success(true, "Đã hủy kết bạn");
        }

        public async Task<BaseResponse<List<FriendUserDto>>> GetFriendsAsync(long userId)
        {
            var result = new BaseResponse<List<FriendUserDto>>();

            var friendships = _crmUnitOfWork.GetRepositoryReadOnlyAsync<CrmFriendship>().QueryAll()
                .Where(x => !x.IsDeleted && x.Status == "ACCEPTED" && (x.RequesterId == userId || x.AddresseeId == userId))
                .ToList();

            var friendUserIds = friendships
                .Select(x => x.RequesterId == userId ? x.AddresseeId : x.RequesterId)
                .ToList();

            var pinnedIds = _crmUnitOfWork.GetRepositoryReadOnlyAsync<CrmFriendPin>().QueryAll()
                .Where(x => !x.IsDeleted && x.UserId == userId && friendUserIds.Contains(x.FriendUserId))
                .Select(x => x.FriendUserId)
                .ToHashSet();

            var data = MapFriendUsers(friendships, friendUserIds, userId, "ACCEPTED", pinnedIds)
                .OrderByDescending(x => x.IsPinned)
                .ToList();

            result.Data = data;
            result.Code = ResponseStatusCode.Status200;
            return result.Success(data, string.Format(Messages.Action.GetSuccess, "danh sách bạn bè"));
        }

        public async Task<BaseResponse<bool>> TogglePinAsync(long userId, long friendUserId)
        {
            var result = new BaseResponse<bool>();

            var isFriend = _crmUnitOfWork.GetRepositoryReadOnlyAsync<CrmFriendship>().QueryAll()
                .Any(x =>
                    !x.IsDeleted &&
                    x.Status == "ACCEPTED" &&
                    ((x.RequesterId == userId && x.AddresseeId == friendUserId) ||
                     (x.RequesterId == friendUserId && x.AddresseeId == userId)));

            if (!isFriend)
            {
                result.Code = ResponseStatusCode.Status404;
                return result.Fail(false, "Không tìm thấy quan hệ bạn bè");
            }

            var repoPinRead = _crmUnitOfWork.GetRepositoryReadOnlyAsync<CrmFriendPin>().QueryAll();
            var repoPinWrite = _crmUnitOfWork.GetRepositoryAsync<CrmFriendPin>();

            var existing = repoPinRead.FirstOrDefault(x =>
                !x.IsDeleted && x.UserId == userId && x.FriendUserId == friendUserId);

            var now = DateTime.Now;
            bool isPinned;

            if (existing != null)
            {
                existing.IsDeleted = true;
                existing.UpdatedAt = now;
                existing.UpdatedBy = userId;

                await repoPinWrite.UpdateAsync(existing);
                isPinned = false;
            }
            else
            {
                var pin = new CrmFriendPin
                {
                    UserId = userId,
                    FriendUserId = friendUserId,
                    CreatedAt = now,
                    CreatedBy = userId
                };

                await repoPinWrite.AddAsync(pin);
                isPinned = true;
            }

            await _crmUnitOfWork.SaveChangesAsync();

            result.Data = isPinned;
            result.Code = ResponseStatusCode.Status200;
            return result.Success(isPinned, isPinned ? "Đã ghim bạn bè" : "Đã bỏ ghim bạn bè");
        }

        public async Task<BaseResponse<List<FriendUserDto>>> GetPendingRequestsAsync(long userId)
        {
            var result = new BaseResponse<List<FriendUserDto>>();

            var friendships = _crmUnitOfWork.GetRepositoryReadOnlyAsync<CrmFriendship>().QueryAll()
                .Where(x => !x.IsDeleted && x.Status == "PENDING" && x.AddresseeId == userId)
                .ToList();

            var requesterIds = friendships.Select(x => x.RequesterId).ToList();

            var data = MapFriendUsers(friendships, requesterIds, userId, "PENDING");

            result.Data = data;
            result.Code = ResponseStatusCode.Status200;
            return result.Success(data, string.Format(Messages.Action.GetSuccess, "lời mời kết bạn"));
        }

        public async Task<BaseResponse<List<FriendUserDto>>> GetSentRequestsAsync(long userId)
        {
            var result = new BaseResponse<List<FriendUserDto>>();

            var friendships = _crmUnitOfWork.GetRepositoryReadOnlyAsync<CrmFriendship>().QueryAll()
                .Where(x => !x.IsDeleted && x.Status == "PENDING" && x.RequesterId == userId)
                .ToList();

            var addresseeIds = friendships.Select(x => x.AddresseeId).ToList();

            var data = MapFriendUsers(friendships, addresseeIds, userId, "PENDING");

            result.Data = data;
            result.Code = ResponseStatusCode.Status200;
            return result.Success(data, string.Format(Messages.Action.GetSuccess, "lời mời đã gửi"));
        }

        public async Task<BaseResponse<List<FriendSearchResultDto>>> SearchUsersAsync(long userId, string keyword)
        {
            var result = new BaseResponse<List<FriendSearchResultDto>>();

            var repoUserRead = _crmUnitOfWork.GetRepositoryReadOnlyAsync<CrmUser>().QueryAll();
            var repoProfileRead = _crmUnitOfWork.GetRepositoryReadOnlyAsync<CrmUserProfile>().QueryAll();
            var repoFriendRead = _crmUnitOfWork.GetRepositoryReadOnlyAsync<CrmFriendship>().QueryAll();

            var kw = (keyword ?? string.Empty).Trim().ToLower();

            var users = repoUserRead
                .Where(u => !u.IsDeleted && u.Id != userId)
                .ToList();

            var profiles = repoProfileRead.ToList();

            if (!string.IsNullOrWhiteSpace(kw))
            {
                var matchingUserIds = profiles
                    .Where(p => p.FullName != null && p.FullName.ToLower().Contains(kw))
                    .Select(p => p.UserId)
                    .ToHashSet();

                users = users
                    .Where(u => u.Username.ToLower().Contains(kw) || matchingUserIds.Contains(u.Id))
                    .ToList();
            }

            users = users.Take(20).ToList();

            var relevantIds = users.Select(u => u.Id).ToHashSet();

            var friendships = repoFriendRead
                .Where(x => !x.IsDeleted &&
                    ((x.RequesterId == userId && relevantIds.Contains(x.AddresseeId)) ||
                     (x.AddresseeId == userId && relevantIds.Contains(x.RequesterId))))
                .ToList();

            var data = users.Select(u =>
            {
                var profile = profiles.FirstOrDefault(p => p.UserId == u.Id);
                var friendship = friendships.FirstOrDefault(f => f.RequesterId == u.Id || f.AddresseeId == u.Id);

                var status = "NONE";
                if (friendship != null)
                {
                    status = friendship.Status == "ACCEPTED"
                        ? "FRIENDS"
                        : friendship.RequesterId == userId
                            ? "PENDING_SENT"
                            : "PENDING_RECEIVED";
                }

                return new FriendSearchResultDto
                {
                    UserId = u.Id,
                    Username = u.Username,
                    FullName = profile?.FullName,
                    AvatarUrl = string.IsNullOrWhiteSpace(profile?.Uri) ? null : _cloudinaryService.BuildImageUrl(profile!.Uri!),
                    FriendStatus = status,
                    FriendshipId = friendship?.Id
                };
            }).ToList();

            result.Data = data;
            result.Code = ResponseStatusCode.Status200;
            return result.Success(data, string.Format(Messages.Action.GetSuccess, "kết quả tìm kiếm"));
        }

        private List<FriendUserDto> MapFriendUsers(List<CrmFriendship> friendships, List<long> otherUserIds, long userId, string status, HashSet<long>? pinnedIds = null)
        {
            var repoUserRead = _crmUnitOfWork.GetRepositoryReadOnlyAsync<CrmUser>().QueryAll();
            var repoProfileRead = _crmUnitOfWork.GetRepositoryReadOnlyAsync<CrmUserProfile>().QueryAll();

            var users = repoUserRead.Where(u => otherUserIds.Contains(u.Id)).ToList();
            var profiles = repoProfileRead.Where(p => otherUserIds.Contains(p.UserId)).ToList();

            return friendships.Select(f =>
            {
                var otherUserId = f.RequesterId == userId ? f.AddresseeId : f.RequesterId;
                var user = users.FirstOrDefault(u => u.Id == otherUserId);
                var profile = profiles.FirstOrDefault(p => p.UserId == otherUserId);

                return new FriendUserDto
                {
                    FriendshipId = f.Id,
                    UserId = otherUserId,
                    Username = user?.Username ?? string.Empty,
                    FullName = profile?.FullName,
                    AvatarUrl = string.IsNullOrWhiteSpace(profile?.Uri) ? null : _cloudinaryService.BuildImageUrl(profile!.Uri!),
                    Status = status,
                    CreatedAt = f.CreatedAt,
                    IsPinned = pinnedIds?.Contains(otherUserId) ?? false
                };
            }).ToList();
        }
    }
}
