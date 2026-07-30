using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MS_Application.DataTransferObjects.Auth;
using MS_Application.DataTransferObjects.Base;
using MS_Application.Services.Interfaces;
using System.Threading.Tasks;

namespace MS_API.Controllers
{
    [Route("api/otp")]
    [ApiController]
    public class OtpController : BaseController
    {
        private readonly IOtpService _otpService;

        public OtpController(IOtpService otpService)
        {
            _otpService = otpService;
        }

        [AllowAnonymous]
        [HttpPost("send")]
        public async Task<BaseResponse<bool>> Send([FromBody] SendOtpRequestDto dto)
        {
            return await _otpService.SendOtpAsync(dto);
        }

        [AllowAnonymous]
        [HttpPost("verify")]
        public async Task<BaseResponse<bool>> Verify([FromBody] VerifyOtpRequestDto dto)
        {
            return await _otpService.VerifyOtpAsync(dto);
        }
    }
}
