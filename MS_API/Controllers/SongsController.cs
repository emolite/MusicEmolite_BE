
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MS_API.Extensions;
using MS_Application.Constants;
using MS_Application.DataTransferObjects.Base;
using MS_Application.DataTransferObjects.Lyrics;
using MS_Application.DataTransferObjects.Songs;
using MS_Application.External;
using MS_Application.Services;
using MS_Application.Services.Interfaces;
using MS_Application.Services.Interfaces.External;

namespace MS_API.Controllers
{
    [Route("api/songs")]
    [ApiController]
    public class SongsController : BaseController
    {
        private readonly ILyricsService _lyricsService;
        private readonly ISongsService _songsService;
        private readonly IYoutubeService _youtubeService;

        public SongsController(ISongsService songsService, ILyricsService lyricsService, IYoutubeService youtubeService)
        {
            _songsService = songsService;
            _lyricsService = lyricsService;
            _youtubeService = youtubeService;
        }

        [HttpPost("search")]
        public async Task<IActionResult> GetSongs(BaseSearchDto<SongRequestDto> dto)
        {
            var result = await _songsService.GetSongs(dto, UserId);
            return Ok(result);
        }

        [AllowAnonymous]
        [HttpPost("public/search")]
        public async Task<IActionResult> GetPublicSongs([FromBody] BaseSearchDto<SongRequestDto> dto)
        {
            var result = await _songsService.GetPublicSongs(dto);

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

        [HttpPost("albums")]
        public async Task<IActionResult> AddSongToAlbum(long songId, long albumId)
        {
            var result = await _songsService.AddSongToAlbum(songId, albumId, User.GetUserId());
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

        [AllowAnonymous]
        [HttpGet("lyrics")]
        public async Task<IActionResult> GetLyrics([FromQuery] LyricsRequestDto request)
        {
            var result = await _lyricsService.GetLyricsAsync(request);
            if (result.Code == ResponseStatusCode.Status404)
                return NotFound("Lyrics not found");

            if (result.Code == ResponseStatusCode.Status400)
                return BadRequest(result.Message);

            return Ok(result);
        }

        [AllowAnonymous]
        [HttpGet("lyrics/{id}")]
        public async Task<IActionResult> GetLyricsById(int id)
        {
            var response = await _lyricsService.GetLyricsByIdAsync(id);
            return Ok(response);
        }

        [AllowAnonymous]
        [HttpPost("lyrics/search")]
        public async Task<IActionResult> SearchLyrics([FromBody] BaseSearchDto<LyricsSearchRequestDto> request)
        {
            var response = await _lyricsService.SearchLyricsAsync(request);
            return Ok(response);
        }

        [AllowAnonymous]
        [HttpGet("youtube/search")]
        public async Task<IActionResult> SearchYoutube([FromQuery] string q)
        {
            var result = await _youtubeService.SearchSongsAsync(q);

            return Ok(result);
        }

        [AllowAnonymous]
        [HttpGet("youtube/stream/{videoId}")]
        public async Task<IActionResult> GetYoutubeStream(string videoId)
        {
            var result = await _youtubeService.GetAudioStreamUrlAsync(videoId);

            return Ok(result);
        }

        [AllowAnonymous]
        [HttpGet("youtube/playlist/{playlistId}")]
        public async Task<IActionResult> GetPlaylistSongs(string playlistId)
        {
            var result = await _youtubeService.GetPlaylistSongsAsync(playlistId);

            return Ok(result);
        }

        [HttpPost("play/{videoId}")]
        public async Task<IActionResult> Play(string videoId)
        {
            var result = await _youtubeService.PlaySongAsync(videoId, UserId);

            return Ok(result);
        }
    }
}
