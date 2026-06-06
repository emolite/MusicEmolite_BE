using Microsoft.AspNetCore.Http;
using MS_Application.DataTransferObjects.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MS_Application.Services.Interfaces.External
{
    public interface ICloudinaryService
    {
        string BuildAudioUrl(string publicId);
        string BuildImageUrl(string publicId);
        Task<BaseResponse<string>> UploadVideoAsync(IFormFile file);
        Task<BaseResponse<string>> UploadAudioAsync(IFormFile file);
        Task<BaseResponse<string>> UploadProfileImageAsync(IFormFile file);
        Task<BaseResponse<string>> UploadMusicImageAsync(IFormFile file);
    }
}
