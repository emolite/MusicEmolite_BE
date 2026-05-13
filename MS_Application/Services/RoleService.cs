using MS_Application.Constants;
using MS_Application.DataTransferObjects.Base;
using MS_Application.DataTransferObjects.Role;
using MS_Application.Helpers;
using MS_Application.Repositories.Interfaces;
using MS_Application.Services.Interfaces;
using MS_Domain.Entities.CRMS;

namespace MS_Application.Services
{
    public class RoleService : IRoleService
    {
        private readonly ICrmUnitOfWork _crmUnitOfWork;

        public RoleService(ICrmUnitOfWork crmUnitOfWork)
        {
            _crmUnitOfWork = crmUnitOfWork;
        }

        public async Task<BaseTableResponse<RoleResponseDto>> GetRoles(BaseSearchDto<RoleRequestDto> dto)
        {
            var result = new BaseTableResponse<RoleResponseDto>();
            var pageSize = dto.PageSize > 0 ? dto.PageSize : GlobalConstants.DefaultPageSize;
            var repoRole = _crmUnitOfWork.GetRepositoryReadOnlyAsync<CrmRole>().QueryAll().ToList();
            var totalRecords = repoRole.Count;
            var data = repoRole
                .Skip(dto.Start)
                .Take(pageSize)
                .Select(x => new RoleResponseDto
                {
                    Id = x.Id,
                    RoleCode = x.RoleCode,
                    RoleName = x.RoleName,
                    CreatedAt = x.CreatedAt,
                    IsActived = x.IsActived,
                    IsDeleted = x.IsDeleted,
                    CreatedBy = x.CreatedBy,
                })
                .ToList();
            result.TotalPages = (int)Math.Ceiling((double)totalRecords / pageSize);
            result.TotalRecords = totalRecords;
            result.Data = data;
            result.Code = ResponseStatusCode.Status200;
            return result.Success(string.Format(Messages.Action.GetSuccess, "roles"));
        }

        public async Task<BaseResponse<RoleResponseDto>> AddRoles(string roleName)
        {
            var result = new BaseResponse<RoleResponseDto>();
            var repo = _crmUnitOfWork.GetRepositoryAsync<CrmRole>();

            var allRoles = repo.QueryCondition(r => true).ToList();

            var roleCode = GenerateCode.GenerateRoleCode(roleName);
            var newRole = new CrmRole
            {
                RoleCode = roleCode,
                RoleName = roleName,
                CreatedAt = DateTime.Now
            };

            await repo.AddAsync(newRole);
            await _crmUnitOfWork.SaveChangesAsync();

            var data = new RoleResponseDto
            {
                RoleCode = newRole.RoleCode,
                RoleName = newRole.RoleName
            };
            result.Code = ResponseStatusCode.Status200;
            result.Data = data;
            return result.Success(string.Format(Messages.Action.CreateSuccess,"role"));
        }
    }
}
