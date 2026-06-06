using MS_Application.DataTransferObjects.Auth;
using MS_Application.DataTransferObjects.Base;
using MS_Application.DataTransferObjects.GoogleLogin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MS_Application.Services.Interfaces
{
    public interface IAuthService
    {
        Task<BaseResponse<LoginResponseDto>> LoginAsync(LoginRequestDto dto);
        Task<BaseResponse<RegisterResponseDto>> RegisterAsync(RegisterRequestDto dto);
        Task<BaseResponse<bool>> CheckEmailExistsAsync(string email);
        Task<BaseResponse<bool>> CheckIpAddressExistsAsync(string ipAddress);
        Task<BaseResponse<UserVerifyDto>> VerifyTokenAsync(string token);
        Task<BaseResponse<LoginResponseDto>> LoginWithGoogleAsync(GoogleLoginRequestDto dto);
        Task<BaseResponse<bool>> CompleteProfileAsync(CompleteProfileRequestDto dto);
    }
}
