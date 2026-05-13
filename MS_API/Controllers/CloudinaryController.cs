using Microsoft.AspNetCore.Mvc;
using MS_Application.External;
using MS_Application.Services.Interfaces.External;

namespace MS_API.Controllers
{
    [ApiController]
    [Route("api/cloudinary")]
    public class CloudinaryController : ControllerBase
    {
        private readonly ICloudinaryService _cloudinaryService;

        public CloudinaryController(ICloudinaryService cloudinaryService)
        {
            _cloudinaryService = cloudinaryService;
        }

        [HttpPost("upload-video")]
        public async Task<IActionResult> UploadVideo(
            IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("File is required");
            }

            var result = await _cloudinaryService.UploadVideoAsync(file);

            return Ok(result);
        }

        [HttpPost("upload-audio")]
        public async Task<IActionResult> UploadAudio(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("File is required");
            }

            var result = await _cloudinaryService.UploadAudioAsync(file);

            return Ok(result);
        }
    }
}