using MS_Application.Constants;
using MS_Application.DataTransferObjects.Auth;
using MS_Application.DataTransferObjects.Base;
using MS_Application.Helpers;
using MS_Application.Repositories.Interfaces;
using MS_Application.Services.Interfaces;
using MS_Application.Services.Interfaces.External;
using MS_Domain.Entities.CRMS;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace MS_Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly ICrmUnitOfWork _crmUnitOfWork;
        private readonly JwtHelper _jwtHelper;
        private readonly ICloudinaryService _cloudinaryService;

        public AuthService(ICrmUnitOfWork crmUnitOfWork, JwtHelper jwtHelper, ICloudinaryService cloudinaryService)
        {
            _crmUnitOfWork = crmUnitOfWork;
            _jwtHelper=jwtHelper;
            _cloudinaryService=cloudinaryService;
        }

        public async Task<BaseResponse<LoginResponseDto>> LoginAsync(LoginRequestDto dto)
        {
            var result = new BaseResponse<LoginResponseDto>();
            var repoUser = _crmUnitOfWork.GetRepositoryReadOnlyAsync<CrmUser>().QueryAll();
            var user = repoUser.FirstOrDefault(u => u.Username == dto.UserName);
            if (user == null)
            {
                result.Code = ResponseStatusCode.Status404;
                return result.Fail(Messages.Validation.NotFound);
            }
            var token = _jwtHelper.GenerateToken(user.Id, user.RefCode, user.Username, user.RoleCode, user.Email, user.UserType, expireMinutes: 120);

            var data = new LoginResponseDto
            {
                AccessToken = token,
                RefCode = user.RefCode,
                RoleCode = user.RoleCode,
                Username = user.Username,
                Email = user.Email
            };
            result.Data = data;
            result.Code = ResponseStatusCode.Status200;
            return result.Success(Messages.Login.LoginSuccess);
        }

        public async Task<BaseResponse<RegisterResponseDto>> RegisterAsync(RegisterRequestDto dto)
        {
            var result = new BaseResponse<RegisterResponseDto>();

            var repoUser = _crmUnitOfWork.GetRepositoryReadOnlyAsync<CrmUser>().QueryAll();
            var repoRole = _crmUnitOfWork.GetRepositoryReadOnlyAsync<CrmRole>().QueryAll();

            var repoUserWrite = _crmUnitOfWork.GetRepositoryAsync<CrmUser>();
            var repoProfileWrite = _crmUnitOfWork.GetRepositoryAsync<CrmUserProfile>();

            var existingUser = repoUser.FirstOrDefault(u => u.Username == dto.UserName);
            if (existingUser != null)
            {
                result.Code = ResponseStatusCode.Status400;
                return result.Fail(string.Format(Messages.Validation.Exists, "user"));
            }

            var roleCode = repoRole.FirstOrDefault(x => x.RoleName == "user")?.RoleCode;

            HashHelper.CreatePasswordHash(dto.Password, out string passwordHash, out string passwordSalt);

            var newUser = new CrmUser
            {
                RefCode = GenerateCode.GenerateRefCode(),
                Username = dto.UserName,
                Email = dto.Email,
                PasswordHash = passwordHash,
                RoleCode = roleCode,
                CreatedAt = DateTime.Now
            };

            await repoUserWrite.AddAsync(newUser);
            await _crmUnitOfWork.SaveChangesAsync();

            var userProfile = new CrmUserProfile
            {
                UserId = newUser.Id,
                RefCode = newUser.RefCode,
                FullName = dto.FullName,
                CreatedAt = DateTime.Now
            };

            await repoProfileWrite.AddAsync(userProfile);
            await _crmUnitOfWork.SaveChangesAsync();

            var data = new RegisterResponseDto
            {
                RefCode = newUser.RefCode,
                UserName = newUser.Username,
                RoleCode = newUser.RoleCode
            };

            result.Code = ResponseStatusCode.Status200;
            result.Data = data;

            return result.Success(Messages.Register.RegisterSuccess);
        }

        public async Task<BaseResponse<UserVerifyDto>> VerifyTokenAsync(string token)
        {
            var result = new BaseResponse<UserVerifyDto>();
            var repoProfile = _crmUnitOfWork.GetRepositoryReadOnlyAsync<CrmUserProfile>().QueryAll();
            var repoUser = _crmUnitOfWork.GetRepositoryReadOnlyAsync<CrmUser>().QueryAll();

            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);

            var userIdStr = jwtToken.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;
            long userId = 0;
            if (!string.IsNullOrEmpty(userIdStr))
            {
                long.TryParse(userIdStr, out userId);
            }
            var refCode = jwtToken.Claims.FirstOrDefault(c => c.Type == "RefCode")?.Value;
            var roleCode = jwtToken.Claims.FirstOrDefault(c => c.Type == "RoleCode")?.Value;
            var username = jwtToken.Claims.FirstOrDefault(c => c.Type == "unique_name")?.Value;
            var email = jwtToken.Claims.FirstOrDefault(c => c.Type == "email")?.Value;
            var userTypeStr = jwtToken.Claims.FirstOrDefault(c => c.Type == "user_type")?.Value;

            short userTypeValue = 0;

            if (!string.IsNullOrEmpty(userTypeStr))
            {
                short.TryParse(userTypeStr, out userTypeValue);
            }

            CrmUserProfile? profile = null;
            profile = repoProfile.FirstOrDefault(p => p.RefCode == refCode);
            if (profile != null)
            {
                profile.Uri = string.IsNullOrWhiteSpace(profile.Uri) ? null : _cloudinaryService.BuildImageUrl(profile.Uri);
            }
            var data = new UserVerifyDto
            {
                UserId = userId,
                RefCode = refCode,
                RoleCode = roleCode,
                Username = username,
                Email = email,
                Profile = profile,
                UserType = EnumHelper.GetDisplayName((MS_Domain.Enums.UserType)userTypeValue)
            };
            result.Data = data;
            result.Code = ResponseStatusCode.Status200;
            return result.Success(string.Format(Messages.Action.GetSuccess, "data"));
        }
    }
}
