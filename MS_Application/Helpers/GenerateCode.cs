using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MS_Application.Helpers
{
    public static class GenerateCode
    {
        public static string GenerateRoleCode(string rolePrefix)
        {
            string timePart = DateTime.Now.ToString("yyyyMMddHHmmss");
            return $"{rolePrefix.ToUpper()}-{timePart}";
        }

        public static string GenerateRefCode()
        {
            return Guid.NewGuid().ToString("D").ToUpper();
        }
    }
}
