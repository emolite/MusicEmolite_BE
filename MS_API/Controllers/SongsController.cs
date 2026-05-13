
using Microsoft.AspNetCore.Mvc;
using MS_API.Extensions;
using MS_Application.DataTransferObjects.Base;
using MS_Application.DataTransferObjects.Songs;
using MS_Application.Services;
using MS_Application.Services.Interfaces;

namespace MS_API.Controllers
{
    [Route("api/songs")]
    [ApiController]
    public class SongsController : BaseController
    {
        private readonly ISongsService _songsService;

        public SongsController(ISongsService songsService)
        {
            _songsService = songsService;
        }

        [HttpPost("search")]
        public async Task<IActionResult> GetSongs(BaseSearchDto<SongRequestDto> dto)
        {
            var result = await _songsService.GetSongs(dto);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetDetail(long id)
        {
            var result = await _songsService.GetSongDetail(id, User.GetUserId());
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateSong(SongCreateDto dto)
        {
            var result = await _songsService.CreateSong(dto, User.GetUserId());
            return Ok(result);
        }

        [HttpPost("{id}/view")]
        public async Task<IActionResult> IncrementView(long id)
        {
            var result = await _songsService.IncrementView(id);
            return Ok(result);
        }

        [HttpPost("{id}/like")]
        public async Task<IActionResult> ToggleLike(long id)
        {
            var result = await _songsService.ToggleLike(id, UserId);
            return Ok(result);
        }
    }
}
