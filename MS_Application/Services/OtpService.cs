using MS_Domain.Common;
using MS_Application.Constants;
using MS_Application.DataTransferObjects.Auth;
using MS_Application.DataTransferObjects.Base;
using MS_Application.Helpers;
using MS_Application.Repositories.Interfaces;
using MS_Application.Services.Interfaces;
using MS_Application.Services.Interfaces.External;
using MS_Domain.Entities.CRMS;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MS_Application.Services
{
    public class OtpService : IOtpService
    {
        private const int CodeLength = 6;
        private const int ExpiryMinutes = 5;
        private const int ResendCooldownSeconds = 60;
        private const int MaxRequestsPerHour = 5;
        private const int MaxVerifyAttempts = 5;

        private readonly ICrmUnitOfWork _crmUnitOfWork;
        private readonly IEmailService _emailService;

        public OtpService(ICrmUnitOfWork crmUnitOfWork, IEmailService emailService)
        {
            _crmUnitOfWork = crmUnitOfWork;
            _emailService = emailService;
        }

        public async Task<BaseResponse<bool>> SendOtpAsync(SendOtpRequestDto dto)
        {
            var result = new BaseResponse<bool>();

            if (string.IsNullOrWhiteSpace(dto.Email))
            {
                result.Code = ResponseStatusCode.Status400;
                return result.Fail(false, string.Format(Messages.Validation.Required, "Email"));
            }

            var purpose = OtpPurpose.Normalize(dto.Purpose);
            if (!OtpPurpose.IsValid(purpose))
            {
                result.Code = ResponseStatusCode.Status400;
                return result.Fail(false, Messages.Otp.InvalidPurpose);
            }

            var email = dto.Email.Trim().ToLower();
            var now = DateTimeHelper.VnNow;

            var repoRead = _crmUnitOfWork.GetRepositoryReadOnlyAsync<CrmOtpCode>().QueryAll();
            var repoWrite = _crmUnitOfWork.GetRepositoryAsync<CrmOtpCode>();

            var lastCode = repoRead
                .Where(x => x.Email == email && x.Purpose == purpose)
                .OrderByDescending(x => x.CreatedAt)
                .FirstOrDefault();

            if (lastCode?.CreatedAt != null &&
                (now - lastCode.CreatedAt.Value).TotalSeconds < ResendCooldownSeconds)
            {
                result.Code = ResponseStatusCode.Status429;
                return result.Fail(false, Messages.PasswordReset.ResetWait60Second);
            }

            var oneHourAgo = now.AddHours(-1);
            var requestsLastHour = repoRead.Count(x =>
                x.Email == email &&
                x.Purpose == purpose &&
                x.CreatedAt.HasValue &&
                x.CreatedAt.Value > oneHourAgo);

            if (requestsLastHour >= MaxRequestsPerHour)
            {
                result.Code = ResponseStatusCode.Status429;
                return result.Fail(false, Messages.PasswordReset.TooManyRequests);
            }

            var code = GenerateCode.GenerateOtpCode(CodeLength);

            var otp = new CrmOtpCode
            {
                Email = email,
                Code = code,
                Purpose = purpose,
                ExpiredAt = now.AddMinutes(ExpiryMinutes),
                IsUsed = false,
                AttemptCount = 0,
                CreatedAt = now
            };

            await repoWrite.AddAsync(otp);
            await _crmUnitOfWork.SaveChangesAsync();

            var subject = OtpPurpose.GetEmailSubject(purpose);
            var body = OtpPurpose.GetEmailBody(purpose, code, ExpiryMinutes);

            await _emailService.SendEmailAsync(email, subject, body);

            result.Code = ResponseStatusCode.Status200;
            return result.Success(true, Messages.Otp.SendSuccess);
        }

        public async Task<BaseResponse<bool>> VerifyOtpAsync(VerifyOtpRequestDto dto)
        {
            var result = new BaseResponse<bool>();

            if (string.IsNullOrWhiteSpace(dto.Email) || string.IsNullOrWhiteSpace(dto.Code))
            {
                result.Code = ResponseStatusCode.Status400;
                return result.Fail(false, Messages.Validation.InvalidModel);
            }

            var purpose = OtpPurpose.Normalize(dto.Purpose);
            if (!OtpPurpose.IsValid(purpose))
            {
                result.Code = ResponseStatusCode.Status400;
                return result.Fail(false, Messages.Otp.InvalidPurpose);
            }

            var email = dto.Email.Trim().ToLower();
            var now = DateTimeHelper.VnNow;

            var repoRead = _crmUnitOfWork.GetRepositoryReadOnlyAsync<CrmOtpCode>().QueryAll();
            var repoWrite = _crmUnitOfWork.GetRepositoryAsync<CrmOtpCode>();

            var otp = repoRead
                .Where(x => x.Email == email && x.Purpose == purpose && !x.IsUsed)
                .OrderByDescending(x => x.CreatedAt)
                .FirstOrDefault();

            if (otp == null)
            {
                result.Code = ResponseStatusCode.Status404;
                return result.Fail(false, Messages.Otp.NotFound);
            }

            if (otp.ExpiredAt < now)
            {
                result.Code = ResponseStatusCode.Status400;
                return result.Fail(false, Messages.Otp.Expired);
            }

            if (otp.AttemptCount >= MaxVerifyAttempts)
            {
                result.Code = ResponseStatusCode.Status429;
                return result.Fail(false, Messages.Otp.TooManyAttempts);
            }

            if (otp.Code != dto.Code.Trim())
            {
                otp.AttemptCount += 1;
                otp.UpdatedAt = now;

                await repoWrite.UpdateAsync(otp);
                await _crmUnitOfWork.SaveChangesAsync();

                result.Code = ResponseStatusCode.Status400;
                return result.Fail(false, Messages.Otp.InvalidCode);
            }

            otp.IsUsed = true;
            otp.UpdatedAt = now;

            await repoWrite.UpdateAsync(otp);
            await _crmUnitOfWork.SaveChangesAsync();

            result.Code = ResponseStatusCode.Status200;
            return result.Success(true, Messages.Otp.VerifySuccess);
        }
    }
}
