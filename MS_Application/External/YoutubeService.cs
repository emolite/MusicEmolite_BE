using MS_Application.DataTransferObjects.Base;
using MS_Application.DataTransferObjects.Youtube;
using MS_Application.Services.Interfaces.External;
using YoutubeExplode;
using YoutubeExplode.Videos.Streams;

namespace MS_Application.External
{
    public class YoutubeService : IYoutubeService
    {
        private readonly YoutubeClient _youtube;

        public YoutubeService()
        {
            _youtube = new YoutubeClient();
        }

        public async Task<BaseTableResponse<YoutubeSongResponseDto>> SearchSongsAsync(string query)
        {
            var data = new List<YoutubeSongResponseDto>();

            await foreach (var video in _youtube.Search.GetVideosAsync(query))
            {
                data.Add(new YoutubeSongResponseDto
                {
                    Id = video.Id.Value,
                    Title = video.Title,
                    Artist = video.Author.ChannelTitle,
                    Thumbnail = video.Thumbnails
                        .OrderByDescending(x => x.Resolution.Area)
                        .FirstOrDefault()?.Url ?? "",
                    Duration = video.Duration?.TotalSeconds ?? 0
                });

                if (data.Count >= 20)
                    break;
            }

            return new BaseTableResponse<YoutubeSongResponseDto>
            {
                Code = "200",
                Message = "Success",
                TotalRecords = data.Count,
                TotalPages = 1,
                Data = data
            };
        }

        public async Task<BaseResponse<YoutubeStreamResponseDto>> GetAudioStreamUrlAsync(string videoId)
        {
            var manifest = await _youtube
                .Videos
                .Streams
                .GetManifestAsync(videoId);

            var streamInfo = manifest
                .GetAudioOnlyStreams()
                .GetWithHighestBitrate();

            if (streamInfo == null)
            {
                return new BaseResponse<YoutubeStreamResponseDto>
                {
                    Code = "404",
                    Message = "Stream not found"
                };
            }

            return new BaseResponse<YoutubeStreamResponseDto>
            {
                Code = "200",
                Message = "Success",
                Data = new YoutubeStreamResponseDto
                {
                    StreamUrl = streamInfo.Url
                }
            };
        }

        public async Task<BaseTableResponse<YoutubeSongResponseDto>>GetPlaylistSongsAsync(string playlistId)
        {
            var data = new List<YoutubeSongResponseDto>();

            await foreach (var video in _youtube
                .Playlists
                .GetVideosAsync(playlistId))
            {
                data.Add(new YoutubeSongResponseDto
                {
                    Id = video.Id.Value,
                    Title = video.Title,
                    Artist = video.Author.ChannelTitle,
                    Thumbnail = video.Thumbnails
                        .OrderByDescending(x => x.Resolution.Area)
                        .FirstOrDefault()?.Url ?? "",
                    Duration = video.Duration?.TotalSeconds ?? 0
                });

                if (data.Count >= 20)
                    break;
            }

            return new BaseTableResponse<YoutubeSongResponseDto>
            {
                Code = "200",
                Message = "Success",
                TotalRecords = data.Count,
                TotalPages = 1,
                Data = data
            };
        }
    }
}