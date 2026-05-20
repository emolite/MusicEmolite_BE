using MS_Application.DataTransferObjects.Base;
using MS_Application.DataTransferObjects.Songs;
using MS_Application.DataTransferObjects.Youtube;
using MS_Application.Helpers;
using MS_Application.Repositories.Interfaces;
using MS_Application.Services.Interfaces.External;
using MS_Domain.Entities.DISTS;
using YoutubeExplode;
using YoutubeExplode.Videos.Streams;

namespace MS_Application.External
{
    public class YoutubeService : IYoutubeService
    {
        private readonly YoutubeClient _youtube;
        private readonly IDistUnitOfWork _distUnitOfWork;

        public YoutubeService(IDistUnitOfWork distUnitOfWork)
        {
            _youtube = new YoutubeClient();
            _distUnitOfWork = distUnitOfWork;
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
            try
            {
                var manifest = await _youtube.Videos.Streams.GetManifestAsync(videoId);

                var streamInfo = manifest.GetAudioOnlyStreams().GetWithHighestBitrate();

                if (streamInfo == null)
                {
                    return new BaseResponse<YoutubeStreamResponseDto>
                    {
                        Code = "404",
                        Message = $"No audio stream found for videoId: {videoId}"
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
            catch (Exception ex)
            {
                var errorMessage = ex.Message;
                var stackTrace = ex.StackTrace;

                Console.WriteLine("========== YOUTUBE STREAM ERROR ==========");
                Console.WriteLine($"VideoId: {videoId}");
                Console.WriteLine($"Message: {errorMessage}");
                Console.WriteLine($"StackTrace: {stackTrace}");
                Console.WriteLine("==========================================");

                return new BaseResponse<YoutubeStreamResponseDto>
                {
                    Code = "500",
                    Message = $"{errorMessage}"
                };
            }
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

        public async Task<BaseResponse<SongResponseDto>>PlaySongAsync(string videoId, long userId)
        {
            var result = new BaseResponse<SongResponseDto>();
            var repoSong =_distUnitOfWork.GetRepositoryAsync<DistSongs>();

            var song = repoSong
                .QueryCondition(x =>
                    x.YoutubeVideoId == videoId
                    && !x.IsDeleted)
                .FirstOrDefault();

            if (song == null)
            {
                var video = await _youtube.Videos.GetAsync(videoId);

                song = new DistSongs
                {
                    Title = video.Title,

                    Duration =
                        (int)(video.Duration?.TotalSeconds ?? 0),

                    YoutubeVideoId = videoId,

                    ImgUrl = video.Thumbnails
                        .OrderByDescending(x => x.Resolution.Area)
                        .FirstOrDefault()?.Url,

                    SourceType = 1,

                    Type = 1,

                    ReleaseDate = DateOnly.FromDateTime(DateTime.UtcNow),

                    CreatedBy = userId,

                    Views = 0,
                    Likes = 0,
                    PlayCount = 0
                };

                await repoSong.AddAsync(song);

                await _distUnitOfWork.SaveChangesAsync();
            }

            song.PlayCount += 1;
            song.Views += 1;

            await repoSong.UpdateAsync(song);

            await _distUnitOfWork.SaveChangesAsync();

            var stream = await GetAudioStreamUrlAsync(videoId);

            result.Data = new SongResponseDto
            {
                Id = song.Id,
                Title = song.Title,
                Duration = song.Duration,
                ImgUrl = song.ImgUrl,
                FileUrl = stream.Data?.StreamUrl,
                Views = song.Views,
                Likes = song.Likes
            };

            result.Code = "200";

            return result.Success("Play success");
        }
    }
}