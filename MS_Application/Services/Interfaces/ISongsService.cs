using Microsoft.AspNetCore.Mvc;
using MS_Application.DataTransferObjects.Base;
using MS_Application.DataTransferObjects.Songs;

namespace MS_Application.Services.Interfaces
{
    public interface ISongsService
    {
        Task<BaseTableResponse<SongResponseDto>> GetSongs(BaseSearchDto<SongRequestDto> dto);
        Task<BaseResponse<SongResponseDto>> GetSongDetail(long id, long userId);
        Task<BaseResponse<SongResponseDto>> IncrementView(long id);
        Task<BaseResponse<SongResponseDto>> ToggleLike(long id, long userId);
        Task<BaseResponse<SongResponseDto>> CreateSong(SongCreateDto dto, long userId);
    }
}
