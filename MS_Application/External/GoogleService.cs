using Google.Apis.Auth;
using Microsoft.Extensions.Configuration;
using MS_Application.Services.Interfaces.External;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MS_Application.External
{
    public class GoogleService : IGoogleService
    {
        private readonly string _googleClientId;

        public GoogleService(IConfiguration configuration)
        {
            _googleClientId = configuration["Authentication:Google:ClientId"]!;
        }

        public async Task<GoogleJsonWebSignature.Payload?> VerifyIdTokenAsync(string idToken)
        {
            try
            {
                var settings = new GoogleJsonWebSignature.ValidationSettings
                {
                    Audience = new[] { _googleClientId }
                };
                return await GoogleJsonWebSignature.ValidateAsync(idToken, settings);
            }
            catch
            {
                return null;
            }
        }
    }
}
