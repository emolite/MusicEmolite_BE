using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MS_Application.DataTransferObjects.Base;
using MS_Application.DataTransferObjects.Albums;
using MS_Application.Services.Interfaces;

namespace MS_API.Controllers
{
    [ApiController]
    [Route("api/albums")]
    public class AlbumsController : BaseController
    {
        private readonly IAlbumsService _albumsService;

        public AlbumsController(IAlbumsService albumsService)
        {
            _albumsService = albumsService;
        }

        [HttpPost("search")]
        public async Task<IActionResult> GetAlbums([FromBody] BaseSearchDto<AlbumRequestDto> dto)
        {
            var result = await _albumsService.GetAlbums(dto, UserId);
            return Ok(result);
        }
    }
}