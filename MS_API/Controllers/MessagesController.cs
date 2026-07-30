using Microsoft.AspNetCore.Mvc;
using MS_Application.DataTransferObjects.Base;
using MS_Application.DataTransferObjects.Message;
using MS_Application.Services.Interfaces;
using MS_Application.Services.Interfaces.External;

namespace MS_API.Controllers
{
    [Route("api/messages")]
    [ApiController]
    public class MessagesController : BaseController
    {
        private readonly IMessageService _messageService;
        private readonly ICloudinaryService _cloudinaryService;

        public MessagesController(IMessageService messageService, ICloudinaryService cloudinaryService)
        {
            _messageService = messageService;
            _cloudinaryService = cloudinaryService;
        }

        [HttpGet("conversations")]
        public async Task<BaseResponse<List<ConversationResponseDto>>> GetConversations()
        {
            return await _messageService.GetConversationsAsync(UserId);
        }

        [HttpGet("{otherUserId}")]
        public async Task<BaseResponse<List<MessageResponseDto>>> GetConversation(long otherUserId, [FromQuery] int page = 1, [FromQuery] int pageSize = 30)
        {
            return await _messageService.GetConversationAsync(UserId, otherUserId, page, pageSize);
        }

        [HttpPost]
        public async Task<BaseResponse<MessageResponseDto>> Send([FromBody] SendMessageRequestDto dto)
        {
            return await _messageService.SendMessageAsync(UserId, dto);
        }

        [HttpPut("{otherUserId}/read")]
        public async Task<BaseResponse<bool>> MarkAsRead(long otherUserId)
        {
            return await _messageService.MarkAsReadAsync(UserId, otherUserId);
        }

        [HttpPost("upload-image")]
        public async Task<IActionResult> UploadImage(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("File is required");
            }

            var result = await _cloudinaryService.UploadChatImageAsync(file);
            return Ok(result);
        }
    }
}
