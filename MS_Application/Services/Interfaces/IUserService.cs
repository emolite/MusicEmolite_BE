using MS_Application.DataTransferObjects.Base;
using MS_Application.DataTransferObjects.User;
using MS_Application.DataTransferObjects.User.BankUser;

namespace MS_Application.Services.Interfaces
{
    public interface IUserService
    {
        Task<BaseTableResponse<CrmUserResponseDto>> GetUsers(BaseSearchDto<CrmUserRequestDto> dto);
        Task<BaseResponse<bool>> UpdateUser(long userId, UpdateUserRequestDto dto);
        Task<BaseResponse<CrmUserProfileResponseDto>> GetUserProfile(long userId);

        Task<BaseResponse<BankUserResponseDto>> GetBankUser(long userId);

        Task<BaseResponse<BankUserResponseDto>> CreateBankUser(long userId, string refCode, BankUserRequestDto dto);

        Task<BaseResponse<BankUserResponseDto>> UpdateBankUser(long userId, long id, BankUserRequestDto dto);

        Task<BaseResponse<bool>> DeleteBankUser(long userId, long id);
    }
}
