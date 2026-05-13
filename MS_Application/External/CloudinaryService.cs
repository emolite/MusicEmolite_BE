using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using MS_Application.Constants;
using MS_Application.DataTransferObjects.Base;
using MS_Application.Helpers;
using MS_Application.Services.Interfaces.External;

namespace MS_Application.External
{
    public class CloudinaryService : ICloudinaryService
    {
        private readonly Cloudinary _cloudinary;

        public CloudinaryService(Cloudinary cloudinary)
        {
            _cloudinary = cloudinary;
        }
        public string BuildAudioUrl(string publicId)
        {
            return _cloudinary.Api.UrlVideoUp
                .Transform(new Transformation()
                    .AudioCodec("mp3")
                    .BitRate("128k")
                    .Quality("auto:low"))
                .BuildUrl(publicId + ".mp3");
        }

        public string BuildImageUrl(string publicId)
        {
            return _cloudinary.Api.UrlImgUp
                .Transform(new Transformation()
                    .Quality("auto")
                    .FetchFormat("auto"))
                .BuildUrl(publicId);
        }

        public async Task<BaseResponse<string>> UploadVideoAsync(IFormFile file)
        {
            var response = new BaseResponse<string>();

            await using var stream = file.OpenReadStream();

            var uploadParams = new VideoUploadParams
            {
                File = new FileDescription(file.FileName, stream),
                Folder = "MUSIC_PROJECT/VIDEOS"
            };

            var result = await _cloudinary.UploadAsync(uploadParams);

            if (result.Error != null)
            {
                throw new Exception(result.Error.Message);
            }

            if (result.SecureUrl == null)
            {
                throw new Exception("Upload failed. SecureUrl is null.");
            }

            response.Data = result.PublicId;

            return response.Success(string.Format(Messages.Action.CreateSuccess, "file"));
        }

        public async Task<BaseResponse<string>> UploadAudioAsync(IFormFile file)
        {
            var response = new BaseResponse<string>();

            await using var stream = file.OpenReadStream();

            var uploadParams = new VideoUploadParams
            {
                File = new FileDescription(file.FileName, stream),
                Folder = "MUSIC_PROJECT/AUDIOS"
            };

            var result = await _cloudinary.UploadAsync(uploadParams);

            if (result.Error != null)
            {
                throw new Exception(result.Error.Message);
            }

            if (result.SecureUrl == null)
            {
                throw new Exception("Upload failed. SecureUrl is null.");
            }

            response.Data = result.PublicId;
            return response.Success(string.Format(Messages.Action.CreateSuccess, "file"));
        }

        public async Task<BaseResponse<string>> UploadProfileImageAsync(IFormFile file)
        {
            var response = new BaseResponse<string>();

            await using var stream = file.OpenReadStream();

            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(file.FileName, stream),
                Folder = "MUSIC_PROJECT/IMAGES/PROFILES"
            };

            var result = await _cloudinary.UploadAsync(uploadParams);

            if (result.Error != null)
            {
                throw new Exception(result.Error.Message);
            }

            if (result.SecureUrl == null)
            {
                throw new Exception("Upload failed. SecureUrl is null.");
            }

            response.Data = result.PublicId;

            return response.Success(string.Format(Messages.Action.CreateSuccess, "image"));
        }

        public async Task<BaseResponse<string>> UploadMusicImageAsync(IFormFile file)
        {
            var response = new BaseResponse<string>();

            await using var stream = file.OpenReadStream();

            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(file.FileName, stream),
                Folder = "MUSIC_PROJECT/IMAGES/MUSICS"
            };

            var result = await _cloudinary.UploadAsync(uploadParams);

            if (result.Error != null)
            {
                throw new Exception(result.Error.Message);
            }

            if (result.SecureUrl == null)
            {
                throw new Exception("Upload failed. SecureUrl is null.");
            }

            response.Data = result.PublicId;

            return response.Success(string.Format(Messages.Action.CreateSuccess, "image"));
        }
    }
}
