using MS_Application.DataTransferObjects.Auth;
using MS_Application.DataTransferObjects.Base;
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
        Task<BaseResponse<UserVerifyDto>> VerifyTokenAsync(string token);
    }
}
