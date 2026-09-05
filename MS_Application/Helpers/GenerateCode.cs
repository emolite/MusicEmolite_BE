using MS_Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace MS_Application.Helpers
{
    public static class GenerateCode
    {
        public static string GenerateRoleCode(string rolePrefix)
        {
            string timePart = DateTimeHelper.VnNow.ToString("yyyyMMddHHmmss");
            return $"{rolePrefix.ToUpper()}-{timePart}";
        }

        public static string GenerateRefCode()
        {
            return Guid.NewGuid().ToString("D").ToUpper();
        }

        public static string GenerateOtpCode(int length = 6)
        {
            var maxValue = (uint)Math.Pow(10, length);
            var randomValue = RandomNumberGenerator.GetInt32((int)maxValue);
            return randomValue.ToString().PadLeft(length, '0');
        }
    }
}
