using MS_Application.DataTransferObjects.Base;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MS_Application.Services.Interfaces;
using MS_Application.DataTransferObjects.User;

namespace MS_API.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UsersController : BaseController
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost]
        public async Task<IActionResult> GetUsers([FromBody] BaseSearchDto<CrmUserRequestDto> dto)
        {
            var result = await _userService.GetUsers(dto);
            return Ok(result);
        }

        [HttpPost("profile")]
        public async Task<IActionResult> GetUserProfile()
        {
            var result = await _userService.GetUserProfile(UserId);
            return Ok(result);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateUser([FromForm] UpdateUserRequestDto dto)
        {
            var result = await _userService.UpdateUser(UserId, dto);
            return Ok(result);
        }
    }
}
