using MS_Application.DataTransferObjects.Base;
using MS_Application.DataTransferObjects.Role;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MS_Application.Services.Interfaces
{
    public interface IRoleService
    {
        Task<BaseTableResponse<RoleResponseDto>> GetRoles(BaseSearchDto<RoleRequestDto> dto);
        Task<BaseResponse<RoleResponseDto>> AddRoles(string roleName);
    }
}
