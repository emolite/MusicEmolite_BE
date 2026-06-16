using Microsoft.Extensions.Configuration;
using MS_Application.Constants;
using MS_Application.DataTransferObjects.Base;
using MS_Application.DataTransferObjects.Youtube;
using MS_Application.Repositories.Interfaces;
using MS_Application.Services.Interfaces.External;
using MS_Domain.Entities.DISTS;
using System.Text.Json;
using System.Xml;

namespace MS_Application.External;

public class YoutubeAPIService : IYoutubeAPIService
{
    private readonly IConfiguration _configuration;
    private readonly HttpClient _httpClient;
    private readonly IDistUnitOfWork _distUnitOfWork;

    public YoutubeAPIService(
        IConfiguration configuration,
        IHttpClientFactory httpClientFactory,
        IDistUnitOfWork distUnitOfWork)
    {
        _configuration = configuration;
        _httpClient = httpClientFactory.CreateClient();
        _distUnitOfWork = distUnitOfWork;
    }

    public async Task<BaseTableResponse<YoutubeVideoDto>> SearchAsync(BaseSearchDto<YoutubeSearchRequestDto> request, long userId)
    {
        var response = new BaseTableResponse<YoutubeVideoDto>();

        var keyword = request.SearchParams?.Keyword;

        if (string.IsNullOrWhiteSpace(keyword))
        {
            response.Code = ResponseStatusCode.Status400;
            response.Type = GlobalConstants.ResponseType.Error;
            response.Message = "Keyword is required";

            return response;
        }

        var apiKey = _configuration["Youtube:ApiKey"];

        var maxResults = request.PageSize > 0
            ? Math.Min(request.PageSize, 50)
            : 20;

        var searchUrl =
            $"https://www.googleapis.com/youtube/v3/search" +
            $"?part=snippet" +
            $"&type=video" +
            $"&maxResults={maxResults}" +
            $"&q={Uri.EscapeDataString(keyword)}" +
            $"&key={apiKey}";

        var youtubeResponse = await _httpClient.GetAsync(searchUrl);

        if (!youtubeResponse.IsSuccessStatusCode)
        {
            response.Code = ResponseStatusCode.Status400;
            response.Type = GlobalConstants.ResponseType.Error;
            response.Message = "Youtube search failed";

            return response;
        }

        var json = await youtubeResponse.Content.ReadAsStringAsync();

        using var doc = JsonDocument.Parse(json);

        var videos = new List<YoutubeVideoDto>();

        foreach (var item in doc.RootElement
                     .GetProperty("items")
                     .EnumerateArray())
        {
            var snippet = item.GetProperty("snippet");
            var thumbnails = snippet.GetProperty("thumbnails");

            var video = new YoutubeVideoDto
            {
                VideoId = item
                    .GetProperty("id")
                    .GetProperty("videoId")
                    .GetString() ?? "",

                Kind = item.TryGetProperty("kind", out var kind)
                    ? kind.GetString() ?? ""
                    : "",

                Etag = item.TryGetProperty("etag", out var etag)
                    ? etag.GetString() ?? ""
                    : "",

                Title = snippet.TryGetProperty("title", out var title)
                    ? title.GetString() ?? ""
                    : "",

                Description = snippet.TryGetProperty("description", out var desc)
                    ? desc.GetString() ?? ""
                    : "",

                ChannelId = snippet.TryGetProperty("channelId", out var channelId)
                    ? channelId.GetString() ?? ""
                    : "",

                Channel = snippet.TryGetProperty("channelTitle", out var channel)
                    ? channel.GetString() ?? ""
                    : "",

                PublishedAt = snippet.TryGetProperty("publishedAt", out var publishedAt)
                    && publishedAt.TryGetDateTime(out var parsedPublishedAt)
                        ? parsedPublishedAt
                        : null,

                ThumbnailDefault = GetThumbnailUrl(thumbnails, "default"),
                ThumbnailMedium = GetThumbnailUrl(thumbnails, "medium"),
                ThumbnailHigh = GetThumbnailUrl(thumbnails, "high"),
                ThumbnailStandard = GetThumbnailUrl(thumbnails, "standard"),
                ThumbnailMaxres = GetThumbnailUrl(thumbnails, "maxres"),

                LiveBroadcastContent = snippet.TryGetProperty("liveBroadcastContent", out var live)
                    ? live.GetString() ?? ""
                    : "",

                PublishTime = snippet.TryGetProperty("publishTime", out var publishTime)
                    ? publishTime.GetString() ?? ""
                    : ""
            };

            videos.Add(video);
        }

        var videoIds = videos
            .Select(x => x.VideoId)
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Distinct()
            .ToList();

        if (videoIds.Count > 0)
        {
            var repoSong = _distUnitOfWork
                .GetRepositoryReadOnlyAsync<DistSongs>()
                .QueryAll();

            var repoUserLike = _distUnitOfWork
                .GetRepositoryReadOnlyAsync<DistUserLikes>()
                .QueryAll();

            var songByVideoId = repoSong
                .Where(x =>
                    !x.IsDeleted &&
                    x.YoutubeVideoId != null &&
                    videoIds.Contains(x.YoutubeVideoId))
                .Select(x => new
                {
                    x.Id,
                    x.YoutubeVideoId
                })
                .ToList();

            var songIds = songByVideoId
                .Select(x => x.Id)
                .ToList();

            var likedSongIds = repoUserLike
                .Where(x =>
                    !x.IsDeleted &&
                    x.UserId == userId &&
                    songIds.Contains(x.SongId))
                .Select(x => x.SongId)
                .ToList();

            foreach (var video in videos)
            {
                var song = songByVideoId
                    .FirstOrDefault(x =>
                        x.YoutubeVideoId == video.VideoId);

                video.IsLiked =
                    song != null &&
                    likedSongIds.Contains(song.Id);
            }

            var detailsUrl =
                $"https://www.googleapis.com/youtube/v3/videos" +
                $"?part=contentDetails,statistics,status,player" +
                $"&id={string.Join(",", videoIds)}" +
                $"&key={apiKey}";

            var detailsResponse = await _httpClient.GetAsync(detailsUrl);

            if (detailsResponse.IsSuccessStatusCode)
            {
                var detailsJson = await detailsResponse.Content.ReadAsStringAsync();

                using var detailsDoc = JsonDocument.Parse(detailsJson);

                var detailsById = detailsDoc.RootElement
                    .GetProperty("items")
                    .EnumerateArray()
                    .ToDictionary(
                        x => x.GetProperty("id").GetString() ?? "",
                        x => x);

                foreach (var video in videos)
                {
                    if (!detailsById.TryGetValue(video.VideoId, out var detail))
                        continue;

                    if (detail.TryGetProperty("contentDetails", out var contentDetails))
                    {
                        video.DurationRaw = contentDetails.TryGetProperty("duration", out var duration)
                            ? duration.GetString() ?? ""
                            : "";

                        video.Duration = ParseYoutubeDurationToSeconds(video.DurationRaw);

                        video.Dimension = contentDetails.TryGetProperty("dimension", out var dimension)
                            ? dimension.GetString() ?? ""
                            : "";

                        video.Definition = contentDetails.TryGetProperty("definition", out var definition)
                            ? definition.GetString() ?? ""
                            : "";

                        video.Caption = contentDetails.TryGetProperty("caption", out var caption)
                            && caption.GetString() == "true";

                        video.LicensedContent = contentDetails.TryGetProperty("licensedContent", out var licensedContent)
                            && licensedContent.ValueKind == JsonValueKind.True;

                        video.Projection = contentDetails.TryGetProperty("projection", out var projection)
                            ? projection.GetString() ?? ""
                            : "";
                    }

                    if (detail.TryGetProperty("statistics", out var statistics))
                    {
                        video.Views = statistics.TryGetProperty("viewCount", out var viewCount)
                            && long.TryParse(viewCount.GetString(), out var parsedViews)
                                ? parsedViews
                                : 0;

                        video.LikeCount = statistics.TryGetProperty("likeCount", out var likeCount)
                            && long.TryParse(likeCount.GetString(), out var parsedLikes)
                                ? parsedLikes
                                : 0;

                        video.CommentCount = statistics.TryGetProperty("commentCount", out var commentCount)
                            && long.TryParse(commentCount.GetString(), out var parsedComments)
                                ? parsedComments
                                : 0;
                    }

                    if (detail.TryGetProperty("status", out var status))
                    {
                        video.Embeddable = status.TryGetProperty("embeddable", out var embeddable)
                            && embeddable.ValueKind == JsonValueKind.True;

                        video.PublicStatsViewable = status.TryGetProperty("publicStatsViewable", out var publicStats)
                            && publicStats.ValueKind == JsonValueKind.True;

                        video.PrivacyStatus = status.TryGetProperty("privacyStatus", out var privacyStatus)
                            ? privacyStatus.GetString() ?? ""
                            : "";

                        video.UploadStatus = status.TryGetProperty("uploadStatus", out var uploadStatus)
                            ? uploadStatus.GetString() ?? ""
                            : "";
                    }

                    if (detail.TryGetProperty("player", out var player))
                    {
                        video.EmbedHtml = player.TryGetProperty("embedHtml", out var embedHtml)
                            ? embedHtml.GetString() ?? ""
                            : "";
                    }
                }
            }
        }

        response.Code = ResponseStatusCode.Status200;
        response.Type = GlobalConstants.ResponseType.Success;
        response.Message = "Search youtube success";

        response.TotalRecords = videos.Count;
        response.TotalPages = 1;
        response.Data = videos;

        return response;
    }

    private static string GetThumbnailUrl(JsonElement thumbnails, string key)
    {
        if (!thumbnails.TryGetProperty(key, out var thumbnail))
            return "";

        return thumbnail.TryGetProperty("url", out var url)
            ? url.GetString() ?? ""
            : "";
    }

    private static int ParseYoutubeDurationToSeconds(string duration)
    {
        if (string.IsNullOrWhiteSpace(duration))
            return 0;

        try
        {
            return (int)XmlConvert.ToTimeSpan(duration).TotalSeconds;
        }
        catch
        {
            return 0;
        }
    }
}