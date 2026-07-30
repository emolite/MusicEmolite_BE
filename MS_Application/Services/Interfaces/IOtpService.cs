using MS_Application.DataTransferObjects.Auth;
using MS_Application.DataTransferObjects.Base;
using System.Threading.Tasks;

namespace MS_Application.Services.Interfaces
{
    public interface IOtpService
    {
        Task<BaseResponse<bool>> SendOtpAsync(SendOtpRequestDto dto);
        Task<BaseResponse<bool>> VerifyOtpAsync(VerifyOtpRequestDto dto);
    }
}
