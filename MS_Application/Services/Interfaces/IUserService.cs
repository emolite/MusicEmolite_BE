using MS_Application.DataTransferObjects.Base;
using MS_Application.DataTransferObjects.User;

namespace MS_Application.Services.Interfaces
{
    public interface IUserService
    {
        Task<BaseTableResponse<CrmUserResponseDto>> GetUsers(BaseSearchDto<CrmUserRequestDto> dto);
        Task<BaseResponse<bool>> UpdateUser(long userId, UpdateUserRequestDto dto);
        Task<BaseResponse<CrmUserProfileResponseDto>> GetUserProfile(long userId);
    }
}
