using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MS_Application.DataTransferObjects.Auth;
using MS_Application.DataTransferObjects.Base;
using MS_Application.Helpers;
using MS_Application.Services.Interfaces;
using System;
using System.Threading.Tasks;

namespace MS_API.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : BaseController
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<BaseResponse<LoginResponseDto>> Login (LoginRequestDto dto)
        {
            var result = await _authService.LoginAsync(dto);
            return result;
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<BaseResponse<RegisterResponseDto>> Register (RegisterRequestDto dto)
        {
            var result = await _authService.RegisterAsync(dto);
            return result;
        }

        [HttpGet("current-user")]
        public async Task<BaseResponse<UserVerifyDto>> VerifyToken()
        {
            var authHeader = HttpContext.Request.Headers["Authorization"].FirstOrDefault();
            if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
                return new BaseResponse<UserVerifyDto>().Fail("Token is missing or invalid.");

            var token = authHeader.Substring("Bearer ".Length).Trim();
            return await _authService.VerifyTokenAsync(token);
        }
    }
}
