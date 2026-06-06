using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MS_API.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static long GetUserId(this ClaimsPrincipal principal)
        {
            var value = principal.FindFirst("UserId")?.Value;
            return long.TryParse(value, out var id) ? id : 0;
        }
    }

}
