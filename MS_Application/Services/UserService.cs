using MS_Application.DataTransferObjects.Base;
using MS_Application.Constants;
using MS_Application.Helpers;
using MS_Application.Repositories.Interfaces;
using MS_Application.Services.Interfaces;
using MS_Domain.Entities.CRMS;
using MS_Application.DataTransferObjects.User;
using MS_Application.Services.Interfaces.External;

namespace MS_Application.Services
{
    public class UserService : IUserService
    {
        private readonly ICrmUnitOfWork _crmUnitOfWork;
        private readonly ICloudinaryService _cloudinaryService;

        public UserService(ICrmUnitOfWork crmUnitOfWork, ICloudinaryService cloudinaryService)
        {
            _crmUnitOfWork = crmUnitOfWork;
            _cloudinaryService = cloudinaryService;
        }


        public async Task<BaseTableResponse<CrmUserResponseDto>> GetUsers(BaseSearchDto<CrmUserRequestDto> dto)
        {
            var result = new BaseTableResponse<CrmUserResponseDto>();
            var repoUser = _crmUnitOfWork.GetRepositoryReadOnlyAsync<CrmUser>().QueryAll();
            var totalRecords = repoUser.Count();
            var data = repoUser
                .Skip(dto.Start)
                .Take(dto.PageSize)
                .Select(x => new CrmUserResponseDto
                {
                    Id = x.Id,
                    RefCode = x.RefCode,
                    Username = x.Username,
                    Email = x.Email,
                    RoleCode = x.RoleCode,
                    CreatedBy = x.CreatedBy,
                })
                .ToList();
            result.TotalRecords = totalRecords;
            result.Data = data;
            result.Code = ResponseStatusCode.Status200;
            return result.Success(string.Format(Messages.Action.GetSuccess, "users"));
        }

        public async Task<BaseResponse<CrmUserProfileResponseDto>> GetUserProfile(long userId)
        {
            var result = new BaseResponse<CrmUserProfileResponseDto>();

            var repoProfile = _crmUnitOfWork.GetRepositoryReadOnlyAsync<CrmUserProfile>().QueryAll();
            var profile = repoProfile.FirstOrDefault(x => x.UserId == userId);

            if (profile == null)
            {
                return result.Fail(string.Format(Messages.Validation.NotFound, "User info"));
            }

            result.Data = new CrmUserProfileResponseDto
            {
                Id = profile.Id,
                RefCode = profile.RefCode,
                UserId = profile.UserId,
                FullName = profile.FullName,
                Phone = profile.Phone,
                DateOfBirth = profile.DateOfBirth,
                Gender = profile.Gender,
                Bio = profile.Bio,
                Uri = string.IsNullOrWhiteSpace(profile.Uri) ? null : _cloudinaryService.BuildImageUrl(profile.Uri),
                IsActived = profile.IsActived,
                IsDeleted = profile.IsDeleted,
                CreatedAt = profile.CreatedAt,
                CreatedBy = profile.CreatedBy,
                UpdatedAt = profile.UpdatedAt,
                UpdatedBy = profile.UpdatedBy
            };

            return result.Success(string.Format(Messages.Action.GetSuccess, "data"));
        }

        public async Task<BaseResponse<bool>> UpdateUser(long userId, UpdateUserRequestDto dto)
        {
            var result = new BaseResponse<bool>();

            var repoUser = _crmUnitOfWork.GetRepositoryReadOnlyAsync<CrmUser>().QueryAll();
            var repoProfile = _crmUnitOfWork.GetRepositoryAsync<CrmUserProfile>();

            var user = repoUser.FirstOrDefault(x => x.Id == userId);

            if (user == null)
            {
                result.Code = ResponseStatusCode.Status404;
                return result.Fail(string.Format(Messages.Validation.NotFound, "user"));
            }

            var profile = repoProfile.QueryAll().FirstOrDefault(x => x.UserId == userId);

            if (profile == null)
            {
                result.Code = ResponseStatusCode.Status404;
                return result.Fail(string.Format(Messages.Validation.NotFound, "profile"));
            }

            profile.FullName = dto.FullName;
            profile.Phone = dto.Phone;
            profile.DateOfBirth = dto.DateOfBirth;
            profile.Gender = dto.Gender;
            profile.Bio = dto.Bio;
            profile.UpdatedAt = DateTime.Now;
            profile.UpdatedBy = userId;

            if (dto.Uri != null)
            {
                var uploadResult = await _cloudinaryService.UploadProfileImageAsync(dto.Uri);

                if (string.IsNullOrEmpty(uploadResult.Data))
                {
                    result.Code = ResponseStatusCode.Status500;
                    return result.Fail(string.Format(Messages.Action.UploadFail, "image profile"));
                }

                profile.Uri = uploadResult.Data;
            }

            await repoProfile.UpdateAsync(profile);
            await _crmUnitOfWork.SaveChangesAsync();

            result.Data = true;
            result.Code = ResponseStatusCode.Status200;

            return result.Success(string.Format(Messages.Action.UpdateSuccess, "data"));
        }
    }
}
