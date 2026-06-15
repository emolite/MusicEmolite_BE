using Microsoft.AspNetCore.Http;
using MS_Application.Constants;
using MS_Application.DataTransferObjects.Auth;
using MS_Application.DataTransferObjects.Base;
using MS_Application.DataTransferObjects.GoogleLogin;
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
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IGoogleService _googleService;

        public AuthService(ICrmUnitOfWork crmUnitOfWork, JwtHelper jwtHelper, ICloudinaryService cloudinaryService, IHttpContextAccessor httpContextAccessor, IGoogleService googleService)
        {
            _crmUnitOfWork = crmUnitOfWork;
            _jwtHelper=jwtHelper;
            _cloudinaryService=cloudinaryService;
            _googleService=googleService;
            _httpContextAccessor=httpContextAccessor;
        }

        public async Task<BaseResponse<LoginResponseDto>> LoginAsync(LoginRequestDto dto)
        {
            var result = new BaseResponse<LoginResponseDto>();

            var repoUser = _crmUnitOfWork
                .GetRepositoryReadOnlyAsync<CrmUser>()
                .QueryAll();

            var repoSessionRead = _crmUnitOfWork
                .GetRepositoryReadOnlyAsync<CrmUserSession>()
                .QueryAll();

            var repoSessionWrite = _crmUnitOfWork
                .GetRepositoryAsync<CrmUserSession>();

            var user = repoUser.FirstOrDefault(u =>
                u.Username == dto.UserName);

            if (user == null)
            {
                result.Code = ResponseStatusCode.Status404;
                return result.Fail("Sai tài khoản hoặc mật khẩu");
            }

            var isValidPassword = HashHelper.VerifyPassword(
                dto.Password,
                user.PasswordHash,
                user.PasswordSalt);

            if (!isValidPassword)
            {
                result.Code = ResponseStatusCode.Status400;
                return result.Fail("Sai tài khoản hoặc mật khẩu");
            }

            var httpContext = _httpContextAccessor.HttpContext;

            var ipAddress =
                httpContext?.Request.Headers["X-Forwarded-For"]
                    .FirstOrDefault();

            if (string.IsNullOrWhiteSpace(ipAddress))
            {
                ipAddress =
                    httpContext?.Connection
                        ?.RemoteIpAddress
                        ?.ToString();
            }

            var userAgent =
                httpContext?.Request.Headers["User-Agent"].ToString();

            var deviceName = userAgent;

            var refreshToken = Guid.NewGuid().ToString();

            var now = DateTime.Now;

            var existingSession =
                repoSessionRead.FirstOrDefault(x =>
                    x.UserId == user.Id &&
                    x.IpAddress == ipAddress &&
                    x.UserAgent == userAgent &&
                    x.ExpiredAt > now &&
                    !x.IsDeleted);

            if (existingSession == null)
            {
                var newSession = new CrmUserSession
                {
                    UserId = user.Id,
                    RefreshToken = refreshToken,
                    IpAddress = ipAddress,
                    UserAgent = userAgent,
                    DeviceName = deviceName,
                    IsVerified = false,
                    LastAccessAt = now,
                    ExpiredAt = now.AddDays(30),

                    IsActived = true,
                    IsDeleted = false,
                    CreatedAt = now,
                    CreatedBy = user.Id
                };

                await repoSessionWrite.AddAsync(newSession);
            }
            else
            {
                existingSession.LastAccessAt = now;
                existingSession.UpdatedAt = now;
                existingSession.UpdatedBy = user.Id;

                await repoSessionWrite.UpdateAsync(existingSession);
            }

            await _crmUnitOfWork.SaveChangesAsync();

            var token = _jwtHelper.GenerateToken(
                user.Id,
                user.RefCode,
                user.Username,
                user.RoleCode,
                user.Email,
                user.UserType,
                expireMinutes: 120);

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

            var roleCode = repoRole.FirstOrDefault(x => x.RoleName == "USER")?.RoleCode;

            HashHelper.CreatePasswordHash(dto.Password, out string passwordHash, out string passwordSalt);

            var newUser = new CrmUser
            {
                RefCode = GenerateCode.GenerateRefCode(),
                Username = dto.UserName,
                Email = dto.Email,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt,
                RoleCode = roleCode,
                UserType = 1,
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

        public async Task<BaseResponse<bool>> CheckEmailExistsAsync(string email)
        {
            var result = new BaseResponse<bool>();

            var repoUser = _crmUnitOfWork
                .GetRepositoryReadOnlyAsync<CrmUser>()
                .QueryAll();

            var exists = repoUser.Any(x =>
                x.Email != null &&
                x.Email.ToLower() == email.ToLower());

            result.Data = exists;

            result.Code = ResponseStatusCode.Status200;

            result.Message = exists
                ? string.Format(Messages.Validation.Exists, "Email")
                : string.Format(Messages.Validation.ValidValue, "Email");

            return result;
        }

        public async Task<BaseResponse<bool>> CheckUserNameExistAsync(string username)
        {
            var result = new BaseResponse<bool>();
            var repoUser = _crmUnitOfWork.GetRepositoryReadOnlyAsync<CrmUser>().QueryAll();

            var exists = repoUser.Any(x => x.Username != null && x.Username.ToLower() == username.ToLower());
            result.Data = exists;
            result.Code = ResponseStatusCode.Status200;
            result.Message = exists
                ? string.Format(Messages.Validation.Exists, "Username")
                : string.Format(Messages.Validation.ValidValue, "Username");

            return result;
        }

        public async Task<BaseResponse<bool>> CheckIpAddressExistsAsync(string ipAddress)
        {
            var result = new BaseResponse<bool>();

            var repoSession = _crmUnitOfWork
                .GetRepositoryReadOnlyAsync<CrmUserSession>()
                .QueryAll();

            var exists = repoSession.Any(x =>
                !x.IsDeleted &&
                x.IpAddress == ipAddress);

            result.Data = exists;

            result.Code = ResponseStatusCode.Status200;

            result.Message = exists
                ? string.Format(Messages.Validation.Exists, "IP Address")
                : string.Format(Messages.Validation.ValidValue, "IP Address");

            return result;
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

        public async Task<BaseResponse<LoginResponseDto>> LoginWithGoogleAsync(GoogleLoginRequestDto dto)
        {
            var result = new BaseResponse<LoginResponseDto>();
            var payload = await _googleService.VerifyIdTokenAsync(dto.IdToken);

            if (payload == null)
            {
                result.Code = ResponseStatusCode.Status400;
                return result.Fail("Google token không hợp lệ");
            }

            var repoUserRead = _crmUnitOfWork.GetRepositoryReadOnlyAsync<CrmUser>().QueryAll();
            var repoUserWrite = _crmUnitOfWork.GetRepositoryAsync<CrmUser>();
            var repoProfileWrite = _crmUnitOfWork.GetRepositoryAsync<CrmUserProfile>();
            var repoRole = _crmUnitOfWork.GetRepositoryReadOnlyAsync<CrmRole>().QueryAll();

            var user = repoUserRead.FirstOrDefault(u => u.Email == payload.Email);
            var isNewUser = user == null;

            if (isNewUser)
            {
                var roleCode = repoRole.FirstOrDefault(x => x.RoleName == "USER")?.RoleCode;

                user = new CrmUser
                {
                    RefCode = GenerateCode.GenerateRefCode(),
                    Username = payload.Email,
                    Email = payload.Email,
                    PasswordHash = string.Empty,
                    PasswordSalt = string.Empty,
                    RoleCode = roleCode,
                    UserType = 2,
                    IsActived = true,
                    IsDeleted = false,
                    CreatedAt = DateTime.Now
                };

                await repoUserWrite.AddAsync(user);
                await _crmUnitOfWork.SaveChangesAsync();

                var profile = new CrmUserProfile
                {
                    UserId = user.Id,
                    RefCode = user.RefCode,
                    IsActived = true,
                    IsDeleted = false,
                    CreatedAt = DateTime.Now
                };

                await repoProfileWrite.AddAsync(profile);
                await _crmUnitOfWork.SaveChangesAsync();
            }

            var token = _jwtHelper.GenerateToken(
                user.Id,
                user.RefCode,
                user.Username,
                user.RoleCode,
                user.Email,
                user.UserType,
                expireMinutes: 120);

            result.Data = new LoginResponseDto
            {
                AccessToken = token,
                RefCode = user.RefCode,
                RoleCode = user.RoleCode,
                Username = user.Username,
                Email = user.Email,
                IsNewUser = isNewUser,
                GoogleName = isNewUser ? payload.Name : null,
                GooglePicture = isNewUser ? payload.Picture : null
            };

            result.Code = ResponseStatusCode.Status200;
            return result.Success(Messages.Login.LoginSuccess);
        }

        public async Task<BaseResponse<bool>> CompleteProfileAsync(CompleteProfileRequestDto dto)
        {
            var result = new BaseResponse<bool>();

            var httpContext = _httpContextAccessor.HttpContext;
            var userIdStr = httpContext?.User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;

            if (string.IsNullOrEmpty(userIdStr) || !long.TryParse(userIdStr, out long userId))
            {
                result.Code = ResponseStatusCode.Status401;
                return result.Fail("Unauthorized");
            }

            var repoProfileRead = _crmUnitOfWork.GetRepositoryReadOnlyAsync<CrmUserProfile>().QueryAll();
            var repoProfileWrite = _crmUnitOfWork.GetRepositoryAsync<CrmUserProfile>();

            var profile = repoProfileRead.FirstOrDefault(p => p.UserId == userId);

            if (profile == null)
            {
                result.Code = ResponseStatusCode.Status404;
                return result.Fail("Profile không tồn tại");
            }

            var now = DateTime.Now;

            if (dto.UseGoogleInfo)
            {
                profile.FullName = dto.GoogleName;
                profile.Uri = dto.GooglePicture;
            }
            else
            {
                profile.FullName = dto.FullName;
                profile.Uri = dto.Uri;
                profile.Phone = dto.PhoneNumber;
                profile.DateOfBirth = dto.DateOfBirth;
            }

            profile.UpdatedAt = now;

            await repoProfileWrite.UpdateAsync(profile);
            await _crmUnitOfWork.SaveChangesAsync();

            result.Data = true;
            result.Code = ResponseStatusCode.Status200;
            return result.Success("Hoàn tất hồ sơ thành công");
        }
    }
}
