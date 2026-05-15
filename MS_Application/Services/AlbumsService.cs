using Microsoft.EntityFrameworkCore;
using MS_Application.Constants;
using MS_Application.DataTransferObjects.Albums;
using MS_Application.DataTransferObjects.Base;
using MS_Application.Helpers;
using MS_Application.Repositories.Interfaces;
using MS_Application.Services.Interfaces;
using MS_Domain.Entities.DISTS;
using MS_Domain.Enums;

namespace MS_Application.Services
{
    public class AlbumsService : IAlbumsService
    {
        private readonly IDistUnitOfWork _distUnitOfWork;

        public AlbumsService(IDistUnitOfWork distUnitOfWork)
        {
            _distUnitOfWork = distUnitOfWork;
        }

        public async Task<BaseTableResponse<AlbumResponseDto>> GetAlbums(BaseSearchDto<AlbumRequestDto> dto, long userId)
        {
            var result = new BaseTableResponse<AlbumResponseDto>();

            var repoAlbum = _distUnitOfWork.GetRepositoryReadOnlyAsync<DistAlbums>().QueryAll();

            var query = repoAlbum.Where(x => !x.IsDeleted && x.CreatedBy == userId);

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
                .Select(x => new AlbumResponseDto
                {
                    Id = x.Id,
                    Title = x.Title,
                    ReleaseDate = x.ReleaseDate,
                    ArtistId = x.ArtistId,
                    AlbumTypeName = EnumHelper.GetDisplayName((MS_Domain.Enums.Type)x.AlbumType),
                    IsActived = x.IsActived,
                    IsDeleted = x.IsDeleted,
                    CreatedAt = x.CreatedAt,
                    CreatedBy = x.CreatedBy
                })
                .ToList();

            result.TotalRecords = totalRecords;
            result.Data = data;
            result.Code = ResponseStatusCode.Status200;

            return result.Success(string.Format(Messages.Action.GetSuccess, "albums"));
        }

        public async Task<BaseTableResponse<AlbumResponseDto>> GetPublicAlbums(BaseSearchDto<AlbumRequestDto> dto)
        {
            var result = new BaseTableResponse<AlbumResponseDto>();

            var repoAlbum = _distUnitOfWork.GetRepositoryReadOnlyAsync<DistAlbums>().QueryAll();

            var query = repoAlbum.Where(x => !x.IsDeleted && x.AlbumType == 1);

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
                .Select(x => new AlbumResponseDto
                {
                    Id = x.Id,
                    Title = x.Title,
                    ReleaseDate = x.ReleaseDate,
                    ArtistId = x.ArtistId,
                    AlbumTypeName = EnumHelper.GetDisplayName((MS_Domain.Enums.Type)x.AlbumType),
                    IsActived = x.IsActived,
                    IsDeleted = x.IsDeleted,
                    CreatedAt = x.CreatedAt,
                    CreatedBy = x.CreatedBy
                })
                .ToList();

            result.TotalRecords = totalRecords;
            result.Data = data;
            result.Code = ResponseStatusCode.Status200;

            return result.Success(string.Format(Messages.Action.GetSuccess, "albums"));
        }

        public async Task<BaseResponse<AlbumResponseDto>> CreateAlbum(AlbumCreateDto dto, long userId)
        {
            var result = new BaseResponse<AlbumResponseDto>();

            var repoAlbum = _distUnitOfWork.GetRepositoryAsync<DistAlbums>();

            var entity = new DistAlbums
            {
                Title = dto.Title,
                ReleaseDate = dto.ReleaseDate,
                ArtistId = dto.ArtistId,
                AlbumType = dto.AlbumType,
                CreatedBy = userId
            };

            await repoAlbum.AddAsync(entity);
            await _distUnitOfWork.SaveChangesAsync();
            result.Code = ResponseStatusCode.Status200;
            return result.Success(string.Format(Messages.Action.CreateSuccess, "album"));
        }

        public async Task<BaseResponse<AlbumResponseDto>> UpdateAlbum(long id, AlbumUpdateDto dto, long userId)
        {
            var result = new BaseResponse<AlbumResponseDto>();

            var repoAlbum = _distUnitOfWork.GetRepositoryAsync<DistAlbums>();

            var album = await repoAlbum
                .QueryAll()
                .FirstOrDefaultAsync(x => x.Id == id);

            if (album == null || album.IsDeleted)
                return result.Fail(string.Format(Messages.Validation.NotFound, "album"));

            album.Title = dto.Title;
            album.ReleaseDate = dto.ReleaseDate;
            album.ArtistId = dto.ArtistId;
            album.IsActived = dto.IsActived;
            album.UpdatedBy = userId;

            repoAlbum.UpdateAsync(album);
            await _distUnitOfWork.SaveChangesAsync();

            result.Code = ResponseStatusCode.Status200;
            return result.Success(string.Format(Messages.Action.UpdateSuccess, "album"));
        }

        public async Task<BaseResponse<bool>> DeleteAlbum(long id, long userId)
        {
            var result = new BaseResponse<bool>();
            var repoAlbum = _distUnitOfWork.GetRepositoryAsync<DistAlbums>();
            var album = await repoAlbum
                .QueryAll()
                .FirstOrDefaultAsync(x => x.Id == id);
            if (album == null || album.IsDeleted)
                return result.Fail(string.Format(Messages.Validation.NotFound, "album"));
            album.IsDeleted = true;
            album.UpdatedBy = userId;
            repoAlbum.UpdateAsync(album);
            await _distUnitOfWork.SaveChangesAsync();
            result.Data = true;
            result.Code = ResponseStatusCode.Status200;
            return result.Success(string.Format(Messages.Action.DeleteSuccess, "album"));
        }
    }
}