using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MS_Application.DataTransferObjects.Base;
using MS_Application.DataTransferObjects.Role;
using MS_Application.Services.Interfaces;

namespace MS_API.Controllers
{
    [Route("api/role")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private readonly IRoleService _roleService;

        public RoleController(IRoleService roleService)
        {
            _roleService = roleService;
        }

        [HttpPost("search-role")]
        public async Task<IActionResult> GetRoles([FromBody] BaseSearchDto<RoleRequestDto> dto)
        {
            var result = await _roleService.GetRoles(dto);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> AddRole(string roleName)
        {
            var result = await _roleService.AddRoles(roleName);
            return Ok(result);
        }
    }
}
