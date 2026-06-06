using Microsoft.AspNetCore.Mvc;
using MS_Application.DataTransferObjects.Base;
using MS_Application.DataTransferObjects.Songs;

namespace MS_Application.Services.Interfaces
{
    public interface ISongsService
    {
        Task<BaseTableResponse<SongResponseDto>> GetSongs(BaseSearchDto<SongRequestDto> dto, long userId);
        Task<BaseTableResponse<SongResponseDto>> GetPublicSongs(BaseSearchDto<SongRequestDto> dto);
        Task<BaseTableResponse<SongResponseDto>> GetRecentSongs(BaseSearchDto<SongRequestDto> dto, long userId);
        Task<BaseTableResponse<SongResponseDto>> GetTrendingSongs(BaseSearchDto<SongRequestDto> dto);
        Task<BaseTableResponse<SongResponseDto>> GetNewestSongs(BaseSearchDto<SongRequestDto> dto);
        Task<BaseResponse<SongResponseDto>> GetSongDetail(long id, long userId);
        Task<BaseResponse<SongResponseDto>> IncrementView(long id, long userId);
        Task<BaseResponse<string>> AddSongHistory(long songId, long userId);
        Task<BaseResponse<SongResponseDto>> ToggleLike(long id, long userId);
        Task<BaseResponse<SongResponseDto>> CreateSong(SongCreateDto dto, long userId);
        Task<BaseResponse<SongResponseDto>> AddSongToAlbum(long songId, long albumId, long userId);
    }
}
