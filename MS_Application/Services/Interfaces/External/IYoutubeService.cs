using MS_Application.DataTransferObjects.Base;
using MS_Application.DataTransferObjects.Youtube;

namespace MS_Application.Services.Interfaces.External
{
    public interface IYoutubeService
    {
        Task<BaseTableResponse<YoutubeSongResponseDto>>SearchSongsAsync(string query);

        Task<BaseResponse<YoutubeStreamResponseDto>>GetAudioStreamUrlAsync(string videoId);

        Task<BaseTableResponse<YoutubeSongResponseDto>>GetPlaylistSongsAsync(string playlistId);
    }
}
