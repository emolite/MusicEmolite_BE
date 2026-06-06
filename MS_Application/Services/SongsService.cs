using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using MS_Application.Constants;
using MS_Application.DataTransferObjects.Base;
using MS_Application.DataTransferObjects.Lyrics;
using MS_Application.DataTransferObjects.Songs;
using MS_Application.Helpers;
using MS_Application.Repositories.Interfaces;
using MS_Application.Services.Interfaces;
using MS_Application.Services.Interfaces.External;
using MS_Domain.Entities.DISTS;
using System.Text.Json;
using TagLib;

namespace MS_Application.Services
{
    public class SongsService : ISongsService
    {
        private readonly IDistUnitOfWork _distUnitOfWork;
        private readonly ICloudinaryService _cloudinaryService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public SongsService(
            IDistUnitOfWork distUnitOfWork,
            ICloudinaryService cloudinaryService,
            IHttpContextAccessor httpContextAccessor)
        {
            _distUnitOfWork = distUnitOfWork;
            _cloudinaryService = cloudinaryService;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<BaseTableResponse<SongResponseDto>> GetSongs(BaseSearchDto<SongRequestDto> dto, long userId)
        {
            var result = new BaseTableResponse<SongResponseDto>();

            var repoSong = _distUnitOfWork.GetRepositoryReadOnlyAsync<DistSongs>().QueryAll();
            var repoSongAlbum = _distUnitOfWork.GetRepositoryReadOnlyAsync<DistSongAlbums>().QueryAll();

            var query = repoSong.Where(x => !x.IsDeleted && x.CreatedBy == userId);

            if (!string.IsNullOrEmpty(dto.SearchParams.Keyword))
            {
                query = query.Where(x =>
                    x.Title.Contains(dto.SearchParams.Keyword));
            }

            var totalRecords = query.Count();

            var data = query
                .OrderByDescending(x => x.CreatedAt)
                .Skip(dto.Start)
                .Take(dto.PageSize)
                .Select(x => new SongResponseDto
                {
                    Id = x.Id,
                    Title = x.Title,
                    Duration = x.Duration,
                    AlbumId = x.AlbumId,
                    ReleaseDate = x.ReleaseDate,
                    FileUrl = _cloudinaryService.BuildAudioUrl(x.FileUrl),
                    ImgUrl = string.IsNullOrWhiteSpace(x.ImgUrl) ? null : _cloudinaryService.BuildImageUrl(x.ImgUrl),
                    ArtistName = x.Artist.Name,
                    TypeSong = EnumHelper.GetDisplayName((MS_Domain.Enums.Type)x.Type),
                    Views = x.Views,
                    Likes = x.Likes,
                    AlbumIds = repoSongAlbum
                        .Where(sa => sa.SongId == x.Id && !sa.IsDeleted)
                        .Select(sa => sa.AlbumId)
                        .ToList(),
                    IsActived = x.IsActived,
                    IsDeleted = x.IsDeleted,
                    CreatedAt = x.CreatedAt,
                    CreatedBy = x.CreatedBy
                })
                .ToList();

            result.TotalRecords = totalRecords;
            result.TotalPages = (int)Math.Ceiling((double)totalRecords / dto.PageSize);
            result.Data = data;
            result.Code = ResponseStatusCode.Status200;

            return result.Success(string.Format(Messages.Action.GetSuccess, "songs"));
        }

        public async Task<BaseTableResponse<SongResponseDto>> GetPublicSongs(BaseSearchDto<SongRequestDto> dto)
        {
            var result = new BaseTableResponse<SongResponseDto>();

            var repoSong = _distUnitOfWork
                .GetRepositoryReadOnlyAsync<DistSongs>()
                .QueryAll();

            var repoSongAlbum = _distUnitOfWork
                .GetRepositoryReadOnlyAsync<DistSongAlbums>()
                .QueryAll();

            var repoAlbum = _distUnitOfWork
                .GetRepositoryReadOnlyAsync<DistAlbums>()
                .QueryAll();

            var query = repoSong
                .Where(x =>
                    !x.IsDeleted
                    && x.Type == 1);

            if (!string.IsNullOrWhiteSpace(dto.SearchParams.Keyword))
            {
                var keyword = dto.SearchParams.Keyword
                    .Trim()
                    .ToLower();

                query = query.Where(x =>
                    x.Title.ToLower().Contains(keyword)
                    || x.Artist.Name.ToLower().Contains(keyword));
            }

            if (dto.SearchParams.AlbumId.HasValue)
            {
                var albumId = dto.SearchParams.AlbumId.Value;

                query = query.Where(x =>
                    x.AlbumId == albumId);
            }

            if (dto.SearchParams.Type.HasValue)
            {
                query = query.Where(x =>
                    x.Type == dto.SearchParams.Type.Value);
            }

            if (dto.SearchParams.IsActived.HasValue)
            {
                query = query.Where(x =>
                    x.IsActived == dto.SearchParams.IsActived.Value);
            }

            query = dto.SearchParams.SortBy?.ToLower() switch
            {
                "views" => dto.Asc
                    ? query.OrderBy(x => x.Views)
                    : query.OrderByDescending(x => x.Views),

                "likes" => dto.Asc
                    ? query.OrderBy(x => x.Likes)
                    : query.OrderByDescending(x => x.Likes),

                "createdat" => dto.Asc
                    ? query.OrderBy(x => x.CreatedAt)
                    : query.OrderByDescending(x => x.CreatedAt),

                _ => dto.Asc
                    ? query.OrderBy(x => x.Id)
                    : query.OrderByDescending(x => x.Id)
            };

            var totalRecords = query.Count();

            var data = query
                .Skip(dto.Start)
                .Take(dto.PageSize)
                .Select(x => new SongResponseDto
                {
                    Id = x.Id,
                    Title = x.Title,
                    Duration = x.Duration,

                    AlbumId = x.AlbumId,

                    AlbumName = repoAlbum
                        .Where(a =>
                            a.Id == x.AlbumId
                            && !a.IsDeleted)
                        .Select(a => a.Title)
                        .FirstOrDefault(),

                    ReleaseDate = x.ReleaseDate,

                    FileUrl = _cloudinaryService
                        .BuildAudioUrl(x.FileUrl),

                    ImgUrl = string.IsNullOrWhiteSpace(x.ImgUrl)
                        ? null
                        : _cloudinaryService.BuildImageUrl(x.ImgUrl),

                    ArtistName = x.Artist.Name,

                    TypeSong = EnumHelper.GetDisplayName(
                        (MS_Domain.Enums.Type)x.Type),

                    Views = x.Views,

                    Likes = x.Likes,

                    AlbumIds = repoSongAlbum
                        .Where(sa =>
                            sa.SongId == x.Id
                            && !sa.IsDeleted)
                        .Select(sa => sa.AlbumId)
                        .ToList(),

                    IsActived = x.IsActived,

                    IsDeleted = x.IsDeleted,

                    CreatedAt = x.CreatedAt,

                    CreatedBy = x.CreatedBy
                })
                .ToList();

            result.TotalRecords = totalRecords;

            result.TotalPages = (int)Math.Ceiling(
                (double)totalRecords / dto.PageSize);

            result.Data = data;

            result.Code = ResponseStatusCode.Status200;

            return result.Success(
                string.Format(Messages.Action.GetSuccess, "songs"));
        }

        public async Task<BaseTableResponse<SongResponseDto>> GetRecentSongs(BaseSearchDto<SongRequestDto> dto, long userId)
        {
            var result = new BaseTableResponse<SongResponseDto>();

            var repoSong = _distUnitOfWork
                .GetRepositoryReadOnlyAsync<DistSongs>()
                .QueryAll();

            var repoSongHistory = _distUnitOfWork.GetRepositoryReadOnlyAsync<DistSongHistories>().QueryAll();

            var repoSongAlbum = _distUnitOfWork
                .GetRepositoryReadOnlyAsync<DistSongAlbums>()
                .QueryAll();

            var recentSongIds = repoSongHistory
    .Where(x =>
        !x.IsDeleted &&
        x.UserId == userId)
    .OrderByDescending(x => x.PlayedAt)
    .Select(x => x.SongId)
    .Distinct();

            var query = repoSong
                .Where(x =>
                    !x.IsDeleted &&
                    recentSongIds.Contains(x.Id));

            var totalRecords = query.Count();

            var data = query
               .OrderByDescending(x =>
                    repoSongHistory
                        .Where(h =>
                            h.SongId == x.Id &&
                            h.UserId == userId)
                        .Max(h => h.PlayedAt))

                .Skip(dto.Start)
                .Take(dto.PageSize)

                .Select(x => new SongResponseDto
                {
                    Id = x.Id,
                    Title = x.Title,
                    Duration = x.Duration,
                    AlbumId = x.AlbumId,
                    ReleaseDate = x.ReleaseDate,

                    FileUrl = _cloudinaryService
                        .BuildAudioUrl(x.FileUrl),

                    ImgUrl = string.IsNullOrWhiteSpace(x.ImgUrl)
                        ? null
                        : _cloudinaryService.BuildImageUrl(x.ImgUrl),

                    ArtistName = x.Artist.Name,

                    TypeSong = EnumHelper.GetDisplayName(
                        (MS_Domain.Enums.Type)x.Type),

                    Views = x.Views,
                    Likes = x.Likes,

                    AlbumIds = repoSongAlbum
                        .Where(sa =>
                            sa.SongId == x.Id &&
                            !sa.IsDeleted)
                        .Select(sa => sa.AlbumId)
                        .ToList(),

                    IsActived = x.IsActived,
                    IsDeleted = x.IsDeleted,
                    CreatedAt = x.CreatedAt,
                    CreatedBy = x.CreatedBy
                })

                .ToList();

            result.TotalRecords = totalRecords;

            result.TotalPages = (int)Math.Ceiling(
                (double)totalRecords / dto.PageSize);

            result.Data = data;
            result.Code = ResponseStatusCode.Status200;

            return result.Success(string.Format(Messages.Action.GetSuccess, "recent songs"));
        }

        public async Task<BaseTableResponse<SongResponseDto>> GetTrendingSongs(BaseSearchDto<SongRequestDto> dto)
        {
            var result = new BaseTableResponse<SongResponseDto>();

            var repoSong = _distUnitOfWork
                .GetRepositoryReadOnlyAsync<DistSongs>()
                .QueryAll();

            var repoSongAlbum = _distUnitOfWork
                .GetRepositoryReadOnlyAsync<DistSongAlbums>()
                .QueryAll();

            var query = repoSong
                .Where(x =>
                    !x.IsDeleted
                    && x.Type == 1);

            if (!string.IsNullOrEmpty(dto.SearchParams.Keyword))
            {
                query = query.Where(x =>
                    x.Title.Contains(dto.SearchParams.Keyword));
            }

            var totalRecords = query.Count();

            var data = query
               .OrderByDescending(x =>
                    x.Views + (x.Likes * 2))
                .ThenByDescending(x =>
                    x.CreatedAt)
                .Skip(dto.Start)
                .Take(dto.PageSize)
                .Select(x => new SongResponseDto
                {
                    Id = x.Id,
                    Title = x.Title,
                    Duration = x.Duration,
                    AlbumId = x.AlbumId,
                    ReleaseDate = x.ReleaseDate,

                    FileUrl = _cloudinaryService
                        .BuildAudioUrl(x.FileUrl),

                    ImgUrl = string.IsNullOrWhiteSpace(x.ImgUrl)
                        ? null
                        : _cloudinaryService.BuildImageUrl(x.ImgUrl),

                    ArtistName = x.Artist.Name,

                    TypeSong = EnumHelper.GetDisplayName(
                        (MS_Domain.Enums.Type)x.Type),

                    Views = x.Views,

                    Likes = x.Likes,

                    AlbumIds = repoSongAlbum
                        .Where(sa =>
                            sa.SongId == x.Id &&
                            !sa.IsDeleted)
                        .Select(sa => sa.AlbumId)
                        .ToList(),

                    IsActived = x.IsActived,
                    IsDeleted = x.IsDeleted,
                    CreatedAt = x.CreatedAt,
                    CreatedBy = x.CreatedBy
                })

                .ToList();

            result.TotalRecords = totalRecords;

            result.TotalPages = (int)Math.Ceiling(
                (double)totalRecords / dto.PageSize);

            result.Data = data;

            result.Code = ResponseStatusCode.Status200;

            return result.Success(
                string.Format(Messages.Action.GetSuccess, "trending songs"));
        }

        public async Task<BaseTableResponse<SongResponseDto>> GetNewestSongs(BaseSearchDto<SongRequestDto> dto)
        {
            var result = new BaseTableResponse<SongResponseDto>();

            var repoSong = _distUnitOfWork
                .GetRepositoryReadOnlyAsync<DistSongs>()
                .QueryAll();

            var repoSongAlbum = _distUnitOfWork
                .GetRepositoryReadOnlyAsync<DistSongAlbums>()
                .QueryAll();

            var repoAlbum = _distUnitOfWork
                .GetRepositoryReadOnlyAsync<DistAlbums>()
                .QueryAll();

            var query = repoSong
                .Where(x =>
                    !x.IsDeleted
                    && x.IsActived
                    && x.Type == 1);

            if (!string.IsNullOrWhiteSpace(dto.SearchParams.Keyword))
            {
                var keyword = dto.SearchParams.Keyword
                    .Trim()
                    .ToLower();

                query = query.Where(x =>
                    x.Title.ToLower().Contains(keyword)
                    || x.Artist.Name.ToLower().Contains(keyword));
            }

            var totalRecords = query.Count();

            var data = query
                .OrderByDescending(x => x.CreatedAt)
                .Skip(dto.Start)
                .Take(dto.PageSize)
                .Select(x => new SongResponseDto
                {
                    Id = x.Id,

                    Title = x.Title,

                    Duration = x.Duration,

                    AlbumId = x.AlbumId,

                    AlbumName = repoAlbum
                        .Where(a =>
                            a.Id == x.AlbumId
                            && !a.IsDeleted)
                        .Select(a => a.Title)
                        .FirstOrDefault(),

                    ReleaseDate = x.ReleaseDate,

                    FileUrl = _cloudinaryService
                        .BuildAudioUrl(x.FileUrl),

                    ImgUrl = string.IsNullOrWhiteSpace(x.ImgUrl)
                        ? null
                        : _cloudinaryService.BuildImageUrl(x.ImgUrl),

                    ArtistName = x.Artist.Name,

                    TypeSong = EnumHelper.GetDisplayName(
                        (MS_Domain.Enums.Type)x.Type),

                    Views = x.Views,

                    Likes = x.Likes,

                    AlbumIds = repoSongAlbum
                        .Where(sa =>
                            sa.SongId == x.Id
                            && !sa.IsDeleted)
                        .Select(sa => sa.AlbumId)
                        .ToList(),

                    IsActived = x.IsActived,

                    IsDeleted = x.IsDeleted,

                    CreatedAt = x.CreatedAt,

                    CreatedBy = x.CreatedBy
                })
                .ToList();

            result.TotalRecords = totalRecords;

            result.TotalPages = (int)Math.Ceiling(
                (double)totalRecords / dto.PageSize);

            result.Data = data;

            result.Code = ResponseStatusCode.Status200;

            return result.Success(string.Format(Messages.Action.GetSuccess, "newest songs"));
        }

        public async Task<BaseResponse<SongResponseDto>> GetSongDetail(long id, long userId)
        {
            var result = new BaseResponse<SongResponseDto>();

            var repoSong = _distUnitOfWork.GetRepositoryReadOnlyAsync<DistSongs>().QueryAll();
            var repoSongLyric = _distUnitOfWork.GetRepositoryReadOnlyAsync<DistSongLyrics>().QueryAll();
            var repoUserLike = _distUnitOfWork.GetRepositoryReadOnlyAsync<DistUserLikes>().QueryAll();

            var song = repoSong.FirstOrDefault(x => x.Id == id && !x.IsDeleted);
            var lyrics = repoSongLyric.FirstOrDefault(x => x.SongId == id && !x.IsDeleted);
            var isLiked = repoUserLike.Any(x => x.UserId == userId && x.SongId == id);
            if (song == null)
            {
                return result.Fail(string.Format(Messages.Validation.NotFound, "song"));
            }
            var deserializeOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            result.Data = new SongResponseDto
            {
                Id = song.Id,
                Title = song.Title,
                Duration = song.Duration,
                FileUrl = _cloudinaryService.BuildAudioUrl(song.FileUrl),
                ImgUrl = string.IsNullOrWhiteSpace(song.ImgUrl) ? null : _cloudinaryService.BuildImageUrl(song.ImgUrl),
                ReleaseDate = song.ReleaseDate,
                AlbumId = song.AlbumId,
                ArtistName = song.ArtistId.ToString(),
                TypeSong = EnumHelper.GetDisplayName((MS_Domain.Enums.Type)song.Type),
                IsLiked = isLiked,
                Lyrics = lyrics?.Content,
                SyncedLyrics = lyrics?.SyncedJson == null
            ? null
            : JsonSerializer.Deserialize<List<LyricsLineDto>>(lyrics.SyncedJson.ToString(), deserializeOptions),
                IsActived = song.IsActived,
                IsDeleted = song.IsDeleted,
                CreatedAt = song.CreatedAt,
                CreatedBy = song.CreatedBy
            };

            result.Code = ResponseStatusCode.Status200;

            return result.Success(string.Format(Messages.Action.GetSuccess, "song"));
        }

        public async Task<BaseResponse<SongResponseDto>> IncrementView(long id, long userId)
        {
            var result = new BaseResponse<SongResponseDto>();

            var repoSong = _distUnitOfWork
                .GetRepositoryAsync<DistSongs>();

            var repoSongView = _distUnitOfWork
                .GetRepositoryAsync<DistSongViews>();

            var song = await repoSong.FindByIdAsync(id);

            if (song == null || song.IsDeleted)
            {
                return result.Fail(
                    string.Format(Messages.Validation.NotFound, "song"));
            }

            var ipAddress = _httpContextAccessor
                .HttpContext?
                .Request?
                .Headers["X-Forwarded-For"]
                .FirstOrDefault();

            if (string.IsNullOrWhiteSpace(ipAddress))
            {
                ipAddress = _httpContextAccessor
                    .HttpContext?
                    .Connection?
                    .RemoteIpAddress?
                    .ToString();
            }

            if (!string.IsNullOrWhiteSpace(ipAddress)
                && ipAddress.Contains(','))
            {
                ipAddress = ipAddress
                    .Split(',')
                    .First()
                    .Trim();
            }

            var limitTime = DateTime.Now.AddSeconds(-30);

            var recentView = repoSongView
                .QueryAll()
                .Any(x =>
                    x.SongId == id &&
                    (
                        (userId > 0 && x.UserId == userId)
                        || x.IpAddress == ipAddress
                    ) &&
                    x.ViewedAt >= limitTime &&
                    !x.IsDeleted);

            if (!recentView)
            {
                song.Views += 1;
                await repoSong.UpdateAsync(song);
                var songView = new DistSongViews
                {
                    SongId = id,
                    UserId = userId > 0 ? userId : null,
                    IpAddress = ipAddress,
                    ViewedAt = DateTime.Now,
                    IsActived = true,
                    IsDeleted = false,
                    CreatedBy = userId > 0 ? userId : null
                };

                await repoSongView.AddAsync(songView);

                await _distUnitOfWork.SaveChangesAsync();
            }

            return result.Success(
                string.Format(Messages.Action.UpdateSuccess, "view"));
        }

        public async Task<BaseResponse<string>> AddSongHistory(long songId, long userId)
        {
            var result = new BaseResponse<string>();

            var repoSong = _distUnitOfWork
                .GetRepositoryReadOnlyAsync<DistSongs>()
                .QueryAll();

            var repoHistory = _distUnitOfWork
                .GetRepositoryAsync<DistSongHistories>();

            var song = repoSong.FirstOrDefault(x =>
                x.Id == songId &&
                !x.IsDeleted);

            if (song == null)
            {
                return result.Fail(string.Format(Messages.Validation.NotFound, "song"));
            }

            var history = new DistSongHistories
            {
                SongId = songId,
                UserId = userId,
                PlayedAt = DateTime.Now,
                CreatedBy = userId
            };

            await repoHistory.AddAsync(history);

            await _distUnitOfWork.SaveChangesAsync();

            return result.Success(string.Format(Messages.Action.CreateSuccess, "song history"));
        }

        public async Task<BaseResponse<SongResponseDto>> ToggleLike(long id, long userId)
        {
            var result = new BaseResponse<SongResponseDto>();

            var repoSong = _distUnitOfWork.GetRepositoryAsync<DistSongs>();
            var repoLike = _distUnitOfWork.GetRepositoryAsync<DistUserLikes>();

            var song = await repoSong.FindByIdAsync(id);

            if (song == null || song.IsDeleted)
                return result.Fail(string.Format(Messages.Validation.NotFound, "song"));

            var existed = repoLike
                .QueryCondition(x => x.UserId == userId && x.SongId == id && !x.IsDeleted)
                .FirstOrDefault();

            if (existed != null)
            {
                await repoLike.DeleteAsync(existed);
                if (song.Likes > 0)
                {
                    song.Likes--;
                }
            }
            else
            {
                await repoLike.AddAsync(new DistUserLikes
                {
                    UserId = userId,
                    SongId = id,
                    CreatedBy = userId
                });
                song.Likes += 1;
            }

            await repoSong.UpdateAsync(song);
            await _distUnitOfWork.SaveChangesAsync();

            return result.Success(string.Format(Messages.Action.UpdateSuccess, "like"));
        }

        public async Task<BaseResponse<SongResponseDto>> CreateSong(SongCreateDto dto, long userId)
        {
            var result = new BaseResponse<SongResponseDto>();

            var repoSongWrite = _distUnitOfWork.GetRepositoryAsync<DistSongs>();
            var repoLyricsWrite = _distUnitOfWork.GetRepositoryAsync<DistSongLyrics>();
            var repoArtistWrite = _distUnitOfWork.GetRepositoryAsync<DistArtists>();

            var extension = Path.GetExtension(dto.FileUrl.FileName);

            var tempFile = Path.Combine(
                Path.GetTempPath(),
                $"{Guid.NewGuid()}{extension}");

            await using (var stream = dto.FileUrl.OpenReadStream())
            await using (var fs = System.IO.File.Create(tempFile))
            {
                await stream.CopyToAsync(fs);
            }

            int duration = 0;

            try
            {
                var tagFile = TagLib.File.Create(tempFile);

                duration = (int)tagFile
                    .Properties
                    .Duration
                    .TotalSeconds;
            }
            catch{}

            System.IO.File.Delete(tempFile);

            var uploadAudioTask = _cloudinaryService
                .UploadAudioAsync(dto.FileUrl);

            var uploadImageTask = _cloudinaryService
                .UploadMusicImageAsync(dto.ImgUrl);

            await Task.WhenAll(uploadAudioTask, uploadImageTask);

            var uploadAudio = await uploadAudioTask;
            var uploadImage = await uploadImageTask;

            if (string.IsNullOrEmpty(uploadAudio.Data))
            {
                return result.Fail(
                    string.Format(Messages.Action.UploadFail, "audio"));
            }

            if (string.IsNullOrEmpty(uploadImage.Data))
            {
                return result.Fail(
                    string.Format(Messages.Action.UploadFail, "image"));
            }

            long? artistId = null;

            if (!string.IsNullOrWhiteSpace(dto.ArtistName))
            {
                var normalizedName = dto.ArtistName.Trim();

                var artist = await repoArtistWrite
                    .QueryCondition(x =>
                        x.Name.ToLower() == normalizedName.ToLower())
                    .FirstOrDefaultAsync();

                if (artist == null)
                {
                    artist = new DistArtists
                    {
                        Name = normalizedName,
                        StageName = normalizedName,
                        CreatedBy = userId
                    };

                    await repoArtistWrite.AddAsync(artist);
                    await _distUnitOfWork.SaveChangesAsync();
                }

                artistId = artist.Id;
            }

            var entity = new DistSongs
            {
                Title = dto.Title,
                Duration = duration,
                ReleaseDate = dto.ReleaseDate,
                FileUrl = uploadAudio.Data,
                ImgUrl = uploadImage.Data,
                AlbumId = dto.AlbumId,
                Type = dto.Type,
                ArtistId = artistId,
                CreatedBy = userId,
            };

            await repoSongWrite.AddAsync(entity);
            await _distUnitOfWork.SaveChangesAsync();

            if (!string.IsNullOrWhiteSpace(dto.Lyrics))
            {
                var serializeOptions = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                };

                var deserializeOptions = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                var lyricsDto = JsonSerializer.Deserialize<SongLyricsCreateDto>(dto.Lyrics, deserializeOptions);

                var lyrics = new DistSongLyrics
                {
                    SongId = entity.Id,
                    Content = lyricsDto?.Lyrics ?? dto.Lyrics,
                    SyncedJson = lyricsDto?.SyncedLyrics != null
                        ? JsonSerializer.Serialize(lyricsDto.SyncedLyrics, serializeOptions)
                        : null,
                    CreatedBy = userId,
                };

                await repoLyricsWrite.AddAsync(lyrics);
                await _distUnitOfWork.SaveChangesAsync();
            }

            result.Data = new SongResponseDto
            {
                Title = entity.Title,
                Duration = entity.Duration,
                FileUrl = _cloudinaryService
                    .BuildAudioUrl(entity.FileUrl),

                ImgUrl = _cloudinaryService
                    .BuildImageUrl(entity.ImgUrl)
            };

            return result.Success(
                string.Format(Messages.Action.CreateSuccess, "song"));
        }

        public async Task<BaseResponse<SongResponseDto>> AddSongToAlbum(long songId, long albumId, long userId)
        {
            var result = new BaseResponse<SongResponseDto>();

            var repoSong = _distUnitOfWork.GetRepositoryReadOnlyAsync<DistSongs>().QueryAll();
            var repoAlbum = _distUnitOfWork.GetRepositoryReadOnlyAsync<DistAlbums>().QueryAll();
            var repoSongAlbum = _distUnitOfWork.GetRepositoryAsync<DistSongAlbums>();

            var song = repoSong.FirstOrDefault(x => x.Id == songId && !x.IsDeleted);
            if (song == null)
                return result.Fail(string.Format(Messages.Validation.NotFound, "song"));

            var album = repoAlbum.FirstOrDefault(x => x.Id == albumId && !x.IsDeleted);
            if (album == null)
                return result.Fail(string.Format(Messages.Validation.NotFound, "album"));

            var existed = repoSongAlbum
                .QueryCondition(x => x.SongId == songId && x.AlbumId == albumId && !x.IsDeleted)
                .FirstOrDefault();

            if (existed != null)
                return result.Fail("Bài hát đã có trong album này");

            await repoSongAlbum.AddAsync(new DistSongAlbums
            {
                SongId = songId,
                AlbumId = albumId,
                CreatedBy = userId
            });

            await _distUnitOfWork.SaveChangesAsync();

            result.Code = ResponseStatusCode.Status200;
            return result.Success(string.Format(Messages.Action.UpdateSuccess, "album"));
        }
    }
}