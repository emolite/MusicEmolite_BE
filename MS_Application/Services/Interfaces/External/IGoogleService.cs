using Google.Apis.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MS_Application.Services.Interfaces.External
{
    public interface IGoogleService
    {
        Task<GoogleJsonWebSignature.Payload?> VerifyIdTokenAsync(string idToken);
    }
}
