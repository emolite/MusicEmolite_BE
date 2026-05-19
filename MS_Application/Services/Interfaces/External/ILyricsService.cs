using MS_Application.DataTransferObjects.Base;
using MS_Application.DataTransferObjects.Lyrics;

namespace MS_Application.Services.Interfaces.External
{
    public interface ILyricsService
    {
        Task<BaseResponse<LyricsResponseDto?>> GetLyricsAsync(LyricsRequestDto request);
        Task<BaseResponse<LyricsResponseDto?>> GetLyricsByIdAsync(int id);
        Task<BaseTableResponse<LyricsResponseDto>> SearchLyricsAsync(BaseSearchDto<LyricsSearchRequestDto> request);
    }
}
