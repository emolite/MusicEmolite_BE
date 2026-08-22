using Microsoft.Extensions.Caching.Memory;
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
    private readonly IMemoryCache _cache;

    /// <summary>
    /// Youtube search only supports cursor pagination (pageToken), not numeric
    /// pages, and each search.list call costs 100 quota units. So instead of
    /// re-querying Youtube on every "load more" (infinite scroll), we fetch up
    /// to this many results once per keyword, cache them, and slice pages out
    /// of the cache - scrolling further within the same search costs 0 extra
    /// quota until the cache entry expires.
    /// </summary>
    private const int MaxCachedRecords = 300;
    private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(30);

    public YoutubeAPIService(
        IConfiguration configuration,
        IHttpClientFactory httpClientFactory,
        IDistUnitOfWork distUnitOfWork,
        IMemoryCache cache)
    {
        _configuration = configuration;
        _httpClient = httpClientFactory.CreateClient();
        _distUnitOfWork = distUnitOfWork;
        _cache = cache;
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

        var pageSize = request.PageSize > 0
            ? Math.Min(request.PageSize, 50)
            : 20;

        var cacheKey = $"youtube_search_{keyword.Trim().ToLowerInvariant()}";

        if (!_cache.TryGetValue(cacheKey, out List<YoutubeVideoDto>? allVideos) || allVideos == null)
        {
            allVideos = await FetchAndEnrichVideosAsync(keyword, apiKey);

            if (allVideos.Count > 0)
            {
                _cache.Set(cacheKey, allVideos, CacheDuration);
            }
        }

        var totalRecords = allVideos.Count;
        var totalPages = totalRecords == 0
            ? 0
            : (int)Math.Ceiling(totalRecords / (double)pageSize);

        var currentPage = Math.Max(request.Page, 1);

        // Clone so per-user like status (set below) never mutates the shared cache entry.
        var pageVideos = allVideos
            .Skip((currentPage - 1) * pageSize)
            .Take(pageSize)
            .Select(CloneVideo)
            .ToList();

        if (pageVideos.Count > 0)
        {
            await AttachLikeStatusAsync(pageVideos, userId);
        }

        response.Code = ResponseStatusCode.Status200;
        response.Type = GlobalConstants.ResponseType.Success;
        response.Message = "Search youtube success";

        response.TotalRecords = totalRecords;
        response.TotalPages = totalPages;
        response.Data = pageVideos;

        return response;
    }

    private async Task<List<YoutubeVideoDto>> FetchAndEnrichVideosAsync(string keyword, string? apiKey)
    {
        var videos = new List<YoutubeVideoDto>();
        string? pageToken = null;

        do
        {
            var searchUrl =
                $"https://www.googleapis.com/youtube/v3/search" +
                $"?part=snippet" +
                $"&type=video" +
                $"&maxResults=50" +
                $"&q={Uri.EscapeDataString(keyword)}" +
                (string.IsNullOrEmpty(pageToken) ? "" : $"&pageToken={pageToken}") +
                $"&key={apiKey}";

            var youtubeResponse = await _httpClient.GetAsync(searchUrl);

            if (!youtubeResponse.IsSuccessStatusCode)
                break;

            var json = await youtubeResponse.Content.ReadAsStringAsync();

            using var doc = JsonDocument.Parse(json);

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

            pageToken = doc.RootElement.TryGetProperty("nextPageToken", out var npt)
                ? npt.GetString()
                : null;
        }
        while (videos.Count < MaxCachedRecords && !string.IsNullOrEmpty(pageToken));

        var videoIds = videos
            .Select(x => x.VideoId)
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Distinct()
            .ToList();

        if (videoIds.Count == 0)
            return videos;

        // ===== Gọi song song videos.list + channels.list, chia batch 50 id/lần =====
        var detailsById = new Dictionary<string, JsonElement>();
        var detailsDocs = new List<JsonDocument>();

        var channelIds = videos
            .Select(x => x.ChannelId)
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Distinct()
            .ToList();

        var channelById = new Dictionary<string, JsonElement>();
        var channelDocs = new List<JsonDocument>();

        try
        {
            foreach (var idBatch in Chunk(videoIds, 50))
            {
                var detailsUrl =
                    $"https://www.googleapis.com/youtube/v3/videos" +
                    $"?part=contentDetails,statistics,status,player" +
                    $"&id={string.Join(",", idBatch)}" +
                    $"&key={apiKey}";

                var detailsResponse = await _httpClient.GetAsync(detailsUrl);

                if (!detailsResponse.IsSuccessStatusCode)
                    continue;

                var detailsJson = await detailsResponse.Content.ReadAsStringAsync();
                var detailsDoc = JsonDocument.Parse(detailsJson);
                detailsDocs.Add(detailsDoc);

                foreach (var item in detailsDoc.RootElement.GetProperty("items").EnumerateArray())
                {
                    var id = item.GetProperty("id").GetString() ?? "";
                    if (!string.IsNullOrEmpty(id))
                        detailsById[id] = item;
                }
            }

            foreach (var idBatch in Chunk(channelIds, 50))
            {
                var channelsUrl =
                    $"https://www.googleapis.com/youtube/v3/channels" +
                    $"?part=snippet" +
                    $"&id={string.Join(",", idBatch)}" +
                    $"&key={apiKey}";

                var channelsResponse = await _httpClient.GetAsync(channelsUrl);

                if (!channelsResponse.IsSuccessStatusCode)
                    continue;

                var channelsJson = await channelsResponse.Content.ReadAsStringAsync();
                var channelsDoc = JsonDocument.Parse(channelsJson);
                channelDocs.Add(channelsDoc);

                foreach (var item in channelsDoc.RootElement.GetProperty("items").EnumerateArray())
                {
                    var id = item.GetProperty("id").GetString() ?? "";
                    if (!string.IsNullOrEmpty(id))
                        channelById[id] = item;
                }
            }

            foreach (var video in videos)
            {
                if (detailsById.TryGetValue(video.VideoId, out var detail))
                {
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

                if (channelById.TryGetValue(video.ChannelId, out var channelDetail)
                    && channelDetail.TryGetProperty("snippet", out var channelSnippet)
                    && channelSnippet.TryGetProperty("thumbnails", out var channelThumbnails))
                {
                    video.ChannelThumbnail = GetThumbnailUrl(channelThumbnails, "high");

                    if (string.IsNullOrEmpty(video.ChannelThumbnail))
                        video.ChannelThumbnail = GetThumbnailUrl(channelThumbnails, "medium");

                    if (string.IsNullOrEmpty(video.ChannelThumbnail))
                        video.ChannelThumbnail = GetThumbnailUrl(channelThumbnails, "default");
                }
            }
        }
        finally
        {
            foreach (var d in detailsDocs) d.Dispose();
            foreach (var d in channelDocs) d.Dispose();
        }

        return videos;
    }

    private async Task AttachLikeStatusAsync(List<YoutubeVideoDto> pageVideos, long userId)
    {
        var videoIds = pageVideos
            .Select(x => x.VideoId)
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Distinct()
            .ToList();

        if (videoIds.Count == 0)
            return;

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

        foreach (var video in pageVideos)
        {
            var song = songByVideoId
                .FirstOrDefault(x =>
                    x.YoutubeVideoId == video.VideoId);
            video.SongId = song?.Id;
            video.IsLiked =
                song != null &&
                likedSongIds.Contains(song.Id);
        }
    }

    private static IEnumerable<List<string>> Chunk(List<string> source, int size)
    {
        for (var i = 0; i < source.Count; i += size)
        {
            yield return source.Skip(i).Take(size).ToList();
        }
    }

    private static YoutubeVideoDto CloneVideo(YoutubeVideoDto v) => new()
    {
        VideoId = v.VideoId,
        Kind = v.Kind,
        Etag = v.Etag,
        Title = v.Title,
        Description = v.Description,
        ChannelId = v.ChannelId,
        Channel = v.Channel,
        ChannelThumbnail = v.ChannelThumbnail,
        PublishedAt = v.PublishedAt,
        ThumbnailDefault = v.ThumbnailDefault,
        ThumbnailMedium = v.ThumbnailMedium,
        ThumbnailHigh = v.ThumbnailHigh,
        ThumbnailStandard = v.ThumbnailStandard,
        ThumbnailMaxres = v.ThumbnailMaxres,
        LiveBroadcastContent = v.LiveBroadcastContent,
        PublishTime = v.PublishTime,
        DurationRaw = v.DurationRaw,
        Duration = v.Duration,
        Dimension = v.Dimension,
        Definition = v.Definition,
        Caption = v.Caption,
        LicensedContent = v.LicensedContent,
        Projection = v.Projection,
        Views = v.Views,
        LikeCount = v.LikeCount,
        CommentCount = v.CommentCount,
        Embeddable = v.Embeddable,
        PublicStatsViewable = v.PublicStatsViewable,
        PrivacyStatus = v.PrivacyStatus,
        UploadStatus = v.UploadStatus,
        EmbedHtml = v.EmbedHtml
    };

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